using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Queue;
using AzureStorage.Tables;
using Common.Log;
using Lykke.JobTriggers.Abstractions;
using Lykke.JobTriggers.Extenstions;
using Lykke.Services.OperationsHistory.Core;
using Lykke.Services.OperationsHistory.Core.Entities;
using Lykke.Services.OperationsHistory.Core.Notifiers;
using Lykke.Services.OperationsHistory.Core.Settings.Job;
using Lykke.Services.OperationsHistory.Job.Handlers;

namespace Lykke.Services.OperationsHistory.Job
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
