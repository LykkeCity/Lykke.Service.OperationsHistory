using System;
using Autofac;
using Common.Log;
using JetBrains.Annotations;

namespace Lykke.Service.OperationsHistory.Client
{
    [PublicAPI]
    public static class AutofacExtension
    {
        [Obsolete("Registration with ILog is obsolete.")]
        public static void RegisterOperationsHistoryClient(this ContainerBuilder builder, string serviceUrl, ILog log)
            => builder.RegisterOperationsHistoryClient(serviceUrl);

        [Obsolete("Registration with ILog is obsolete.")]
        public static void RegisterOperationsHistoryClient(this ContainerBuilder builder, OperationsHistoryServiceClientSettings settings, ILog log)
            => builder.RegisterOperationsHistoryClient(settings);

        public static void RegisterOperationsHistoryClient(this ContainerBuilder builder, OperationsHistoryServiceClientSettings settings)
            => builder.RegisterOperationsHistoryClient(settings?.ServiceUrl);

        public static void RegisterOperationsHistoryClient(this ContainerBuilder builder, string serviceUrl)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.Register(x => new OperationsHistoryClient(serviceUrl))
                .As<IOperationsHistoryClient>()
                .SingleInstance();
        }
    }
}
