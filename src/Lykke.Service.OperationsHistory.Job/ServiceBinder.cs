using System;
using Autofac;
using AzureStorage.Queue;
using AzureStorage.Tables;
using Common.Log;
using Lykke.JobTriggers.Abstractions;
using Lykke.JobTriggers.Extenstions;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Job.Notifiers;
using Lykke.Service.OperationsHistory.Core.Settings.Job;
using Lykke.Service.OperationsHistory.Job.Handlers;

namespace Lykke.Service.OperationsHistory.Job
{
    public class ServiceBinder
    {
        public ContainerBuilder ConfigureContainer(JobSettingsRoot settings)
        {
            var cBuilder = new ContainerBuilder();
            var log = new LogToConsole();
            cBuilder.RegisterInstance(settings);
            cBuilder.RegisterInstance(log).As<ILog>();
            cBuilder.RegisterInstance<Func<string, IQueueExt>>(qName =>
                new AzureQueueExt(settings.HistoryServiceJob.Db.LogsConnString, qName));
            cBuilder.RegisterInstance(new HistoryLogEntryRepository(new AzureTableStorage<HistoryLogEntryEntity>(
                    settings.HistoryServiceJob.Db.LogsConnString,
                    Constants.OutTableName,
                    log)))
                .As<IHistoryLogEntryRepository>();
            cBuilder.RegisterType<SlackNotifier>().As<IPoisionQueueNotifier>();
            cBuilder.RegisterType<NewHistoryEntryHandler>();
            
            cBuilder.AddTriggers(pool =>
            {
                pool.AddDefaultConnection(settings.HistoryServiceJob.Db.LogsConnString);
            });

            return cBuilder;
        }
    }
}
