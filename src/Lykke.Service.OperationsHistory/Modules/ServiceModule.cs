using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsHistory.Mappers;
using Lykke.Service.OperationsHistory.RabbitSubscribers;
using Lykke.Service.OperationsHistory.Services;
using Lykke.Service.OperationsHistory.Services.InMemoryCache;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

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

            Mapper.Initialize(cfg => cfg.AddProfile(typeof(HistoryLogMapperProfile)));

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
                .As<IHistoryCache>()
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
    }
}
