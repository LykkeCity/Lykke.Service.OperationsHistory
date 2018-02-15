using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common;
using Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsHistory.RabbitSubscribers;
using Lykke.Service.OperationsHistory.Services;
using Lykke.Service.OperationsHistory.Services.InMemoryCache;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Lykke.Service.OperationsHistory.Modules
{
    public class ServiceModule : Module
    {
        private readonly ApiSettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(ApiSettings settings, IReloadingManager<DbSettings> dbSettings, ILog log)
        {
            _settings = settings;
            _dbSettings = dbSettings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
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
            
            _services.RegisterAssetsClient(AssetServiceSettings.Create(new Uri(_settings.AssetsServiceClient.ServiceUrl), TimeSpan.FromMinutes(3)));

            RegisterDictionaryEntities(builder);

            builder.Populate(_services);
        }

        private void RegisterRabbitMqSubscribers(ContainerBuilder builder)
        {
            // TODO: You should register each subscriber in DI container as IStartable singleton and autoactivate it

            builder.RegisterType<OperationsHistorySubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.OperationsHistoryService.RabbitOperations));

            builder.RegisterType<AuthSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.OperationsHistoryService.RabbitRegistration));
        }

        private void RegisterApplicationServices(ContainerBuilder builder)
        {
            builder.RegisterType<InMemoryCache>()
                .WithParameter(TypedParameter.From(_settings.OperationsHistoryService))
                .As<IHistoryOperationsCache>()
                .SingleInstance();

            builder.RegisterType<HistoryOperationAdapter>()
                .As<IHistoryOperationAdapter>()
                .SingleInstance();

            builder.RegisterType<HistoryMessageAdapter>()
                .As<IHistoryMessageAdapter>()
                .SingleInstance();
        }

        private void RegisterAzureRepositories(ContainerBuilder builder)
        {
            builder.RegisterInstance(new HistoryLogEntryRepository(AzureTableStorage<HistoryLogEntryEntity>.Create(
                    _dbSettings.ConnectionString(x => x.LogsConnString), "OperationsHistory", _log)))
                .As<IHistoryLogEntryRepository>();
        }

        private void RegisterServiceClients(ContainerBuilder builder)
        {
            builder.RegisterLykkeServiceClient(_settings.ClientAccountServiceClient.ServiceUrl);
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
    }
}
