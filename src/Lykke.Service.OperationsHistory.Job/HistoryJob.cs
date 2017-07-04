using System.IO;
using System.Runtime.Loader;
using System.Threading;
using Autofac.Extensions.DependencyInjection;
using Lykke.JobTriggers.Triggers;
using Lykke.Service.OperationsHistory.Core.Settings.Job;
using Lykke.Service.OperationsHistory.Core.Settings.Repository;
using Microsoft.Extensions.Configuration;

namespace Lykke.Service.OperationsHistory.Job
{
    public class HistoryJob
    {
        public IConfigurationRoot Configuration { get; }
        public HistoryJob()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        private static JobSettingsRoot ReadConfiguration(string url)
        {
            return new SettingsRepositoryRemote<JobSettingsRoot>(url).Get();
        }

        public void Run()
        {
            var settings = ReadConfiguration(Configuration.GetConnectionString("Settings"));
            var container = new ServiceBinder()
                .ConfigureContainer(settings)
                .Build();

            var triggers = new TriggerHost(new AutofacServiceProvider(container));

            var end = new ManualResetEvent(false);

            AssemblyLoadContext.Default.Unloading += ctx =>
            {
                triggers.Cancel();
                end.WaitOne();
            };

            triggers.Start().Wait();
            end.Set();
        }
    }
}

// test cnages to check nuget package publication
