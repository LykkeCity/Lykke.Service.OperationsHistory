using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsHistory.RabbitSubscribers.Contract;
using Lykke.Service.OperationsHistory.Services;
using System;
using System.Threading.Tasks;

namespace Lykke.Service.OperationsHistory.RabbitSubscribers
{
    public class AuthSubscriber : IStartable, IStopable
    {
        private readonly ILog _log;
        private RabbitMqSubscriber<ClientAuthInfo> _subscriber;
        private readonly RabbitMqSettings _rabbitSettings;
        private readonly IHistoryCache _historyCache;

        public AuthSubscriber(ILog log, RabbitMqSettings rabbitSettings, IHistoryCache historyCache)
        {
            _log = log;
            _rabbitSettings = rabbitSettings;
            _historyCache = historyCache;
        }

        public void Start()
        {
            // NOTE: Read https://github.com/LykkeCity/Lykke.RabbitMqDotNetBroker/blob/master/README.md to learn
            // about RabbitMq subscriber configuration

            var settings = RabbitMqSubscriptionSettings.CreateForSubscriber(
                _rabbitSettings.ConnectionString,
                _rabbitSettings.Exchange, 
                _rabbitSettings.Queue);

            settings.MakeDurable();

            _subscriber = new RabbitMqSubscriber<ClientAuthInfo>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                .SetMessageDeserializer(new JsonMessageDeserializer<ClientAuthInfo>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        private async Task ProcessMessageAsync(ClientAuthInfo arg)
        {
            await _historyCache.WarmUp(arg.ClientId);
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber.Stop();
        }
    }
}