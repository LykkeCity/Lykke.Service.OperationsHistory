using System;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common;
using Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsHistory.Core.Settings.Job;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Lykke.Service.OperationsHistory.Job.RabbitSubscribers;
using Lykke.Service.OperationsHistory.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace Lykke.Service.OperationsHistory.Job.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettingsManager;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(AppSettings settings, IReloadingManager<DbSettings> dbSettingsManager, ILog log)
        {
            _settings = settings;
            _log = log;
            _dbSettingsManager = dbSettingsManager;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterAzureRepositories(builder);

            RegisterApplicationServices(builder);

            RegisterRabbitMqSubscribers(builder);

            RegisterServiceClients(builder);

            RegisterDictionaryEntities(builder);
            
            RegisterCache(builder);

            builder.Populate(_services);
        }

        private void RegisterRabbitMqSubscribers(ContainerBuilder builder)
        {
            // TODO: You should register each subscriber in DI container as IStartable singleton and autoactivate it

            builder.RegisterType<OperationsHistorySubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.OperationsHistoryJob.Rabbit));
            
            builder.RegisterType<MongoDbSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.OperationsHistoryJob.Rabbit));
        }

        private void RegisterApplicationServices(ContainerBuilder builder)
        {
            builder.RegisterType<HistoryWriter>()
                .As<IHistoryWriter>()
                .SingleInstance();
            
            builder.RegisterType<HistoryOperationAdapter>()
                .As<IHistoryOperationAdapter>()
                .SingleInstance();

            builder.RegisterType<HistoryMessageAdapter>()
                .As<IHistoryMessageAdapter>()
                .SingleInstance();
        }
        
        private void RegisterServiceClients(ContainerBuilder builder)
        {
            builder.RegisterLykkeServiceClient(_settings.ClientAccountServiceClient.ServiceUrl);
            
            _services.RegisterAssetsClient(AssetServiceSettings.Create(new Uri(_settings.AssetsServiceClient.ServiceUrl), TimeSpan.FromMinutes(3)), _log);
        }

        private void RegisterAzureRepositories(ContainerBuilder builder)
        {
            builder.RegisterInstance(new HistoryLogEntryRepository(
                    AzureTableStorage<HistoryLogEntryEntity>.Create(
                        _dbSettingsManager.ConnectionString(x => x.DataConnString), "OperationsHistory", _log)))
                .As<IHistoryLogEntryRepository>();
        }
        
        private void RegisterDictionaryEntities(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var ctx = c.Resolve<IComponentContext>();
                return new CachedDataDictionary<string, Asset>(
                    async () =>
                        (await ctx.Resolve<IAssetsService>().AssetGetAllAsync()).ToDictionary(itm => itm.Id));
            }).SingleInstance();

            builder.Register(c =>
            {
                var ctx = c.Resolve<IComponentContext>();
                return new CachedDataDictionary<string, AssetPair>(
                    async () =>
                        (await ctx.Resolve<IAssetsService>().AssetPairGetAllAsync())
                        .ToDictionary(itm => itm.Id));
            }).SingleInstance();
        }

        private void RegisterCache(ContainerBuilder builder)
        {
            var cache = new RedisCache(new RedisCacheOptions
            {
                Configuration = _settings.RedisSettings.Configuration,
                InstanceName = "OperationsHistoryCache"
            });
            
            builder.RegisterInstance(cache)
                .As<IDistributedCache>()
                .Keyed<IDistributedCache>("operationsHistoryCache")
                .SingleInstance();
        }
    }
}