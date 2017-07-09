using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsHistory.Models;
using Lykke.Service.OperationsHistory.Services;
using Lykke.Service.OperationsHistory.Services.InMemoryCache;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.OperationsHistory.Modules
{
    public class ServiceModule : Module
    {
        private readonly OperationsHistorySettings _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(OperationsHistorySettings settings, ILog log)
        {
            _settings = settings;
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

            builder.RegisterInstance(_settings).SingleInstance();

            builder.RegisterType<InMemoryCache>().As<IHistoryCache>().SingleInstance();

            builder.RegisterInstance(new HistoryLogEntryRepository(new AzureTableStorage<HistoryLogEntryEntity>(
                    _settings.Db.LogsConnString,
                    Constants.OutTableName,
                    _log)))
                .As<IHistoryLogEntryRepository>();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<IHistoryLogEntryEntity, HistoryEntryResponse>();
            });

            builder.Populate(_services);
        }
    }
}
