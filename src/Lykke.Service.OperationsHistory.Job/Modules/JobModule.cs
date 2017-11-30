using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Queue;
using AzureStorage.Tables;
using Common.Log;
using Lykke.JobTriggers.Abstractions;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsHistory.Core.Settings.Job;
using Lykke.Service.OperationsHistory.Job.Handlers;
using Lykke.Service.OperationsHistory.Job.Notifiers;
using Lykke.Service.OperationsHistory.Job.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Lykke.JobTriggers.Extenstions;

namespace Lykke.Service.OperationsHistory.Job.Modules
{
    public class JobModule : Module
    {
        private readonly OperationsHistoryJobSettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettingsManager;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(OperationsHistoryJobSettings settings, IReloadingManager<DbSettings> dbSettingsManager, ILog log)
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

            builder.RegisterInstance<Func<string, IQueueExt>>(qName =>
                AzureQueueExt.Create(_dbSettingsManager.ConnectionString(x => x.LogsConnString), qName));

            builder.RegisterInstance(new HistoryLogEntryRepository(
                    AzureTableStorage<HistoryLogEntryEntity>.Create(
                        _dbSettingsManager.ConnectionString(x => x.LogsConnString), Constants.OutTableName, _log)))
                .As<IHistoryLogEntryRepository>();

            builder.RegisterType<SlackNotifier>().As<IPoisionQueueNotifier>();

            builder.RegisterType<NewHistoryEntryHandler>();

            builder.AddTriggers(pool =>
            {
                pool.AddDefaultConnection(_dbSettingsManager.CurrentValue.LogsConnString);
            });

            builder.Populate(_services);
        }
    }
}