using System;
using System.Threading.Tasks;
using AzureStorage.Queue;
using Lykke.JobTriggers.Abstractions;
using Newtonsoft.Json;

namespace Lykke.Services.OperationsHistory.Core.Notifiers
{
    public class SlackNotifier: ISlackNotifier, IPoisionQueueNotifier
    {
        private readonly IQueueExt _queue;
        public SlackNotifier(Func<string, IQueueExt> queueFactory)
        {
            _queue = queueFactory(Constants.JobPoisonQueueName);
        }
        public async Task SendAsync(string type, string sender, string message)
        {
            var msg = new
            {
                Type = type,
                Sender = sender,
                Message = message
            };

            await _queue.PutRawMessageAsync(JsonConvert.SerializeObject(msg));
        }

        public Task NotifyAsync(string message)
        {
            return this.SendErrorAsync(message);
        }
    }
}
