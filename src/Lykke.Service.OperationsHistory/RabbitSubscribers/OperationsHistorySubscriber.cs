using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsRepository.Contract;
using System;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Services;

namespace Lykke.Service.OperationsHistory.RabbitSubscribers
{
    public class OperationsHistorySubscriber : IStartable, IStopable
    {
        private readonly ILog _log;
        private RabbitMqSubscriber<OperationsHistoryMessage> _subscriber;
        private readonly RabbitMqSettings _rabbitSettings;
        private readonly IHistoryCache _historyCache;

        public OperationsHistorySubscriber(ILog log, RabbitMqSettings rabbitSettings, IHistoryCache historyCache)
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
            var newCacheEntry = new HistoryLogEntryEntity
            {
                Id = arg.Id,
                ClientId = arg.ClientId,
                CustomData = arg.Data,
                DateTime = arg.DateTime,
                OpType = arg.OpType,
                Amount = arg.Amount,
                Currency = arg.Currency
            };

            await _historyCache.AddOrUpdate(newCacheEntry);
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