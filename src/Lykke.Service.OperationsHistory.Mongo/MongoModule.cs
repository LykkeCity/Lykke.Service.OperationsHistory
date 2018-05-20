using System;
using Autofac;
using Common.Log;
using Lykke.Service.OperationsHistory.Core.Settings;
using Lykke.Service.OperationsHistory.Core.Settings.Job;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Lykke.Service.OperationsHistory.Mongo
{
    public class MongoModule : Module
    {
        private const string DatabaseName = "OperationsHistory";
        private const string CollectionName = "operations";
        
        private readonly MongoSettings _settings;
        
        public MongoModule(MongoSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterOperationsCollection(builder);
        }

        private void RegisterOperationsCollection(ContainerBuilder builder)
        {
            var client = new MongoClient(_settings.ConnectionString);
            var db = client.GetDatabase(DatabaseName);
            var coll = db.GetCollection<OperationsHistoryEntity>(CollectionName);
            
            coll.Indexes.CreateOne(Builders<OperationsHistoryEntity>.IndexKeys.Ascending(x => x.ClientId));
            coll.Indexes.CreateOne(Builders<OperationsHistoryEntity>.IndexKeys.Ascending(x => x.WalletId));
            coll.Indexes.CreateOne(Builders<OperationsHistoryEntity>.IndexKeys.Ascending(x => x.DateTime));
            coll.Indexes.CreateOne(Builders<OperationsHistoryEntity>.IndexKeys.Ascending(x => x.Type));
            coll.Indexes.CreateOne(Builders<OperationsHistoryEntity>.IndexKeys.Ascending(x => x.AssetId));
            coll.Indexes.CreateOne(Builders<OperationsHistoryEntity>.IndexKeys.Ascending(x => x.AssetPairId));

            builder
                .RegisterInstance(coll)
                .SingleInstance();

            builder
                .RegisterType<OperationsHistoryRepository>()
                .As<IOperationsHistoryRepository>()
                .SingleInstance();
        }
    }
}