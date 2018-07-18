using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.AutorestClient;
using Lykke.Service.ClientAccount.Client.AutorestClient.Models;
using Lykke.Service.OperationsHistory.Core;
using Lykke.Service.OperationsHistory.Core.Services;
using Lykke.Service.OperationsHistory.Core.Settings.Job;
using Lykke.Service.OperationsHistory.Mongo;
using Lykke.Service.OperationsRepository.Contract.History;
using Microsoft.Extensions.Caching.Distributed;
using Lykke.Service.OperationsRepository.Contract;

namespace Lykke.Service.OperationsHistory.Job.RabbitSubscribers
{
    public class MongoDbSubscriber : IStartable, IStopable
    {
        private readonly ILog _log;
        private RabbitMqSubscriber<OperationsHistoryMessage> _subscriber;
        private readonly RabbitMqSettings _rabbitSettings;
        private readonly IOperationsHistoryRepository _operationsHistoryRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IClientAccountClient _clientAccountClient;
        private readonly IHistoryMessageAdapter _historyMessageAdapter;

        public MongoDbSubscriber(
            ILog log,
            RabbitMqSettings rabbitSettings,
            IOperationsHistoryRepository operationsHistoryRepository,
            IDistributedCache distributedCache,
            IClientAccountClient clientAccountClient,
            IHistoryMessageAdapter historyMessageAdapter)
        {
            _log = log;
            _rabbitSettings = rabbitSettings;
            _operationsHistoryRepository = operationsHistoryRepository;
            _distributedCache = distributedCache;
            _clientAccountClient = clientAccountClient;
            _historyMessageAdapter = historyMessageAdapter;
        }
        
        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings.CreateForSubscriber(_rabbitSettings.ConnectionString,
                _rabbitSettings.ExchangeOperationsHistory, "mongodbupdater");

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
            var clientId = await GetClientByWalletAsync(arg.ClientId);

            var walletId =
                clientId != arg.ClientId
                    ? arg.ClientId
                    : await GetClientTradingWalletIdAsync(clientId);

            var operation = await _historyMessageAdapter.ExecuteAsync(arg);

            var validId = IsValidId(operation.Id)
                ? operation.Id
                : MakeGuidFromPair(walletId, operation.Id).ToString();

            var existingEntry = 
                (await Task.WhenAll(
                    _operationsHistoryRepository.GetByIdAsync(clientId, operation.Id),
                    _operationsHistoryRepository.GetByIdAsync(clientId, validId)))
                .FirstOrDefault(x => x != null);
            
            if (existingEntry != null)
            {
                if (IsValidId(existingEntry.Id)) // entity was either migrated or had acceptable id in the first place
                    operation.Id = validId;
            }
            else
            {
                operation.Id = validId; // operation is a new entry
            }

            await _operationsHistoryRepository.AddOrUpdateAsync(clientId, walletId, operation, arg.Data);
        }
        
        private static bool IsValidId(string s)
        {
            return Guid.TryParse(s, out var parsedString) && parsedString.ToString() == s;
        }
        
        private static Guid MakeGuidFromPair(string s1, string s2)
        {
            var arr = new byte[16];

            Array.Copy(new SHA256Managed()
                .ComputeHash(
                    Encoding.ASCII.GetBytes(
                        string.Concat(s1, s2))), 0, arr, 0, 16);
            
            return new Guid(arr);
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber.Stop();
        }

        private async Task<string> GetClientByWalletAsync(string walletId)
        {
            var key = $"OperationsHistory:ClientIdByWalletId:{walletId}";

            var clientId = await _distributedCache.GetStringAsync(key);

            if (clientId != null)
                return clientId;
            
            clientId = await _clientAccountClient.GetClientByWalletAsync(walletId);

            await _distributedCache.SetStringAsync(key, clientId);

            return clientId;
        }

        private async Task<string> GetClientTradingWalletIdAsync(string clientId)
        {
            var key = $"OperationsHistory:ClientTradingWalletId:{clientId}";

            var walletId = await _distributedCache.GetStringAsync(key);

            if (walletId != null)
                return walletId;

            walletId = (await _clientAccountClient.GetClientWalletsFiltered(clientId, WalletType.Trading, OwnerType.Spot))
                .Single()
                .Id;
            
            await _distributedCache.SetStringAsync(key, walletId);

            return walletId;
        }
    }
}