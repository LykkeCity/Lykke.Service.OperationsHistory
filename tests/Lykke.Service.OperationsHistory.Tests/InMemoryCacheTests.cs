using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsHistory.Services.InMemoryCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.OperationsHistory.Tests
{
    [TestClass]
    public class InMemoryCacheTests
    {
        private static OperationsHistorySettings _settings;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _settings = new OperationsHistorySettings
            {
                Db = new DbSettings
                {
                    LogsConnString = string.Empty
                },
                ValuesPerPage = 3
            };

            Mapper.Initialize(cfg => cfg.AddProfile(typeof(TestMappingProfile)));
        }

        [TestMethod]
        public async Task GetRecordsByClient_CachedFullRepository()
        {
            var mockedRepo = new Mock<IHistoryLogEntryRepository>();
            mockedRepo.Setup(m => m.GetByClientIdAsync(It.IsAny<string>())).Returns(GetFakeRepositoryV1);
            var cache = new InMemoryCache(mockedRepo.Object, _settings, null);

            var recordsFromCache = await cache.GetRecordsByClient("any");
            var recordsFromRepo = await GetFakeRepositoryV1();

            Assert.AreEqual(recordsFromRepo.Count(), recordsFromCache.Count());
        }

        private static Task<IList<HistoryLogEntryEntity>> GetFakeRepositoryV1()
        {
            IList<HistoryLogEntryEntity> records = new List<HistoryLogEntryEntity>
            {
                new HistoryLogEntryEntity
                {
                    Id = "216e9bce-34da-438d-ba36-97eafd8e54fe",
                    DateTime = DateTime.Now.AddDays(1),
                    Amount = 1,
                    ClientId = "1",
                    Currency = "USD",
                    OpType = "OpType1",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    Id = "f25b11c5-4126-40dc-8ec6-6b63bc9fdbdf",
                    DateTime = DateTime.Now.AddDays(2),
                    Amount = 2,
                    ClientId = "1",
                    Currency = "CHF",
                    OpType = "OpType2",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    Id = "8d41fdb3-748c-4a9a-9897-5735cc31add8",
                    DateTime = DateTime.Now.AddDays(3),
                    Amount = 3,
                    ClientId = "1",
                    Currency = "EUR",
                    OpType = "OpType3",
                    CustomData = string.Empty
                }
            };

            return Task.FromResult(records);
        }

        private static Task<IList<HistoryLogEntryEntity>> GetFakeRepositoryV2()
        {
            IList<HistoryLogEntryEntity> records = new List<HistoryLogEntryEntity>()
            {
                new HistoryLogEntryEntity
                {
                    Id = "7e719b22-546d-459d-9290-7bbbd655aaad",
                    DateTime = DateTime.Now.AddDays(1),
                    Amount = 1,
                    ClientId = "1",
                    Currency = "USD",
                    OpType = "OpType1",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    Id = "d8a04125-594b-43bb-86df-aeaa0386d794",
                    DateTime = DateTime.Now.AddDays(2),
                    Amount = 2,
                    ClientId = "1",
                    Currency = "CHF",
                    OpType = "OpType2",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    Id = "fb79ead0-d026-48d8-8ae2-863c08910821",
                    DateTime = DateTime.Now.AddDays(3),
                    Amount = 3,
                    ClientId = "1",
                    Currency = "EUR",
                    OpType = "OpType3",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    Id = "455b0957-58f6-441c-8791-51829007116c",
                    DateTime = DateTime.Now.AddDays(4),
                    Amount = 4,
                    ClientId = "1",
                    Currency = "RUB",
                    OpType = "OpType3",
                    CustomData = string.Empty
                }
            };

            return Task.FromResult(records);
        }
    }
}