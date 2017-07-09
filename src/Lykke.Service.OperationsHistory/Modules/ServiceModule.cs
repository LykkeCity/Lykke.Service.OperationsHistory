using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsHistory.Services;
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

            builder.RegisterType<InMemoryCache>().As<IHistoryCache>();

            builder.Populate(_services);
        }
    }
}
