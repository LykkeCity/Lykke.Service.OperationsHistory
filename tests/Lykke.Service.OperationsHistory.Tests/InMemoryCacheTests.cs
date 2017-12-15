using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Services.InMemoryCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.OperationsHistory.Tests
{
    [TestClass]
    public class InMemoryCacheTests
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(typeof(TestMappingProfile)));
        }

        [TestMethod]
        public async Task GetRecordsByClient_CachedFullRepository()
        {
            var mockedRepo = new Mock<IHistoryLogEntryRepository>();
            mockedRepo.Setup(m => m.GetByWalletIdAsync(It.IsAny<string>())).Returns(GetFakeRepositoryV1);
            var cache = new InMemoryCache(mockedRepo.Object, null);

            var recordsFromCache = await cache.GetRecordsByWalletId("any");
            var recordsFromRepo = await GetFakeRepositoryV1();

            Assert.AreEqual(recordsFromRepo.Count(), recordsFromCache.Count());
        }

        private static Task<IList<HistoryLogEntryEntity>> GetFakeRepositoryV1()
        {
            IList<HistoryLogEntryEntity> records = new List<HistoryLogEntryEntity>
            {
                new HistoryLogEntryEntity
                {
                    Id = GetNewGuid(),
                    DateTime = DateTime.Now.AddDays(1),
                    Amount = 1,
                    ClientId = "1",
                    Currency = "USD",
                    OpType = "OpType1",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    Id = GetNewGuid(),
                    DateTime = DateTime.Now.AddDays(2),
                    Amount = 2,
                    ClientId = "1",
                    Currency = "CHF",
                    OpType = "OpType2",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    Id = GetNewGuid(),
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
                    Id = GetNewGuid(),
                    DateTime = DateTime.Now.AddDays(1),
                    Amount = 1,
                    ClientId = "1",
                    Currency = "USD",
                    OpType = "OpType1",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    Id = GetNewGuid(),
                    DateTime = DateTime.Now.AddDays(2),
                    Amount = 2,
                    ClientId = "1",
                    Currency = "CHF",
                    OpType = "OpType2",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    Id = GetNewGuid(),
                    DateTime = DateTime.Now.AddDays(3),
                    Amount = 3,
                    ClientId = "1",
                    Currency = "EUR",
                    OpType = "OpType3",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    Id = GetNewGuid(),
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

        private static string GetNewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}