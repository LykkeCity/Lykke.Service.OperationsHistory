using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsHistory.Mappers;
using Lykke.Service.OperationsHistory.Models;
using Lykke.Service.OperationsHistory.Services;
using Lykke.Service.OperationsHistory.Services.InMemoryCache;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.OperationsHistory.Modules
{
    public class ServiceModule : Module
    {
        private readonly OperationsHistorySettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(OperationsHistorySettings settings, IReloadingManager<DbSettings> dbSettings, ILog log)
        {
            _settings = settings;
            _dbSettings = dbSettings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterType<InMemoryCache>()
                .As<IHistoryCache>()
                .SingleInstance();

            Mapper.Initialize(cfg => cfg.AddProfile(typeof(HistoryLogMapperProfile)));

            builder.RegisterInstance(new HistoryLogEntryRepository(AzureTableStorage<HistoryLogEntryEntity>.Create(
                    _dbSettings.ConnectionString(x => x.LogsConnString), "OperationsHistory", _log)))
                .As<IHistoryLogEntryRepository>();

            builder.Populate(_services);
        }
    }
}
