using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.OperationsHistory.Core.Settings.Job;
using System;
using System.Threading.Tasks;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsRepository.Contract.History;

namespace Lykke.Service.OperationsHistory.Job.RabbitSubscribers
{
    public class OperationsHistorySubscriber : IStartable, IStopable
    {
        private readonly ILog _log;
        private RabbitMqSubscriber<OperationsHistoryMessage> _subscriber;
        private readonly RabbitMqSettings _rabbitSettings;
        private readonly IHistoryWriter _historyWriter;

        public OperationsHistorySubscriber(ILog log, RabbitMqSettings rabbitSettings, IHistoryWriter historyWriter)
        {
            _log = log;
            _rabbitSettings = rabbitSettings;
            _historyWriter = historyWriter;
        }

        public void Start()
        {
            // NOTE: Read https://github.com/LykkeCity/Lykke.RabbitMqDotNetBroker/blob/master/README.md to learn
            // about RabbitMq subscriber configuration

            var settings = RabbitMqSubscriptionSettings.CreateForSubscriber(_rabbitSettings.ConnectionString,
                _rabbitSettings.ExchangeOperationsHistory, _rabbitSettings.QueueOperationsLogUpdater);

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
            await _historyWriter.SaveAsync(arg);
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