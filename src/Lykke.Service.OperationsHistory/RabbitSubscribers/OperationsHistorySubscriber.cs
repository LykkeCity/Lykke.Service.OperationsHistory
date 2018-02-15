using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using System;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsRepository.Contract.History;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsRepository.Contract;

namespace Lykke.Service.OperationsHistory.RabbitSubscribers
{
    public class OperationsHistorySubscriber : IStartable, IStopable
    {
        private readonly ILog _log;
        private RabbitMqSubscriber<OperationsHistoryMessage> _subscriber;
        private readonly RabbitMqSettings _rabbitSettings;
        private readonly IHistoryOperationsCache _historyCache;
        private readonly IHistoryMessageAdapter _adapter;

        public OperationsHistorySubscriber(
            RabbitMqSettings rabbitSettings, 
            IHistoryOperationsCache historyCache,
            IHistoryMessageAdapter adapter,
            ILog log)
        {
            _log = log;
            _rabbitSettings = rabbitSettings;
            _historyCache = historyCache;
            _adapter = adapter;
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

            _subscriber = new RabbitMqSubscriber<OperationsHistoryMessage>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                .SetMessageDeserializer(new JsonMessageDeserializer<OperationsHistoryMessage>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        private async Task ProcessMessageAsync(OperationsHistoryMessage arg)
        {
            var operation = await _adapter.ExecuteAsync(arg);

            await _historyCache.AddOrUpdate(arg.ClientId, operation);
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