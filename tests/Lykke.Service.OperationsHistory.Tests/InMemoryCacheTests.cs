using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Core.Settings.Api;
using Lykke.Service.OperationsHistory.Models;
using Lykke.Service.OperationsHistory.Services.InMemoryCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.OperationsHistory.Tests
{
    [TestClass]
    public class InMemoryCacheTests
    {
        private OperationsHistorySettings _settings;

        [TestInitialize]
        public void Initialize()
        {
            _settings = new OperationsHistorySettings
            {
                Db = new DbSettings
                {
                    LogsConnString = string.Empty
                },
                ValuesPerPage = 3,
                CacheExpiration = 10
            };

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<IHistoryLogEntryEntity, HistoryEntryResponse>();
            });
        }

        [TestMethod]
        public async Task GetRecordsByClient_CachedFullRepository()
        {
            var mockedRepo = new Mock<IHistoryLogEntryRepository>();
            mockedRepo.Setup(m => m.GetAllAsync(It.IsAny<string>())).Returns(GetFakeRepositoryV1);
            var cache = new InMemoryCache(mockedRepo.Object, _settings);

            var recordsFromCache = await cache.GetRecordsByClient("any");
            var recordsFromRepo = await GetFakeRepositoryV1();

            Assert.AreEqual(recordsFromRepo.Count(), recordsFromCache.Count());
        }

        [TestMethod]
        public async Task GetRecordsByClient_CacheExpiration()
        {
            var pause = new ManualResetEvent(false);

            // getting values from repository into cache
            var mockedRepo = new Mock<IHistoryLogEntryRepository>();
            mockedRepo.Setup(m => m.GetAllAsync(It.IsAny<string>())).Returns(GetFakeRepositoryV1);
            var cache = new InMemoryCache(mockedRepo.Object, _settings);
            var recordsFromCacheV1 = await cache.GetRecordsByClient("any");

            Assert.AreEqual(3, recordsFromCacheV1.Count());

            pause.WaitOne(_settings.CacheExpiration / 2 * 1000);

            // still same values in the cache because cache not expired
            recordsFromCacheV1 = await cache.GetRecordsByClient("any");
            Assert.AreEqual(3, recordsFromCacheV1.Count());

            pause.WaitOne(_settings.CacheExpiration / 2 * 1000 + 3000);

            // cache expired so the new values must be fetched from the repository
            mockedRepo.Setup(m => m.GetAllAsync(It.IsAny<string>())).Returns(GetFakeRepositoryV2);
            var recordsFromCacheV2 = await cache.GetRecordsByClient("any");
            Assert.AreEqual(4, recordsFromCacheV2.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_CheckPagination_SinglePage()
        {
            var mockedRepo = new Mock<IHistoryLogEntryRepository>();
            mockedRepo.Setup(m => m.GetAllAsync(It.IsAny<string>())).Returns(GetFakeRepositoryV1);
            var cache = new InMemoryCache(mockedRepo.Object, _settings);

            var pageOne = await cache.GetAllPagedAsync("any", 1);
            var pageTwo = await cache.GetAllPagedAsync("any", 2);

            Assert.AreEqual(3, pageOne.Count());
            Assert.AreEqual(0, pageTwo.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_CheckPagination_MultiplePages()
        {
            var mockedRepo = new Mock<IHistoryLogEntryRepository>();
            mockedRepo.Setup(m => m.GetAllAsync(It.IsAny<string>())).Returns(GetFakeRepositoryV2);
            var cache = new InMemoryCache(mockedRepo.Object, _settings);

            var pageOne = await cache.GetAllPagedAsync("any", 1);
            var pageTwo = await cache.GetAllPagedAsync("any", 2);
            var pageThree = await cache.GetAllPagedAsync("any", 3);

            Assert.AreEqual(3, pageOne.Count());
            Assert.AreEqual(1, pageTwo.Count());
            Assert.AreEqual(0, pageThree.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_FilterByAssetAndOperation_Success()
        {
            var mockedRepo = new Mock<IHistoryLogEntryRepository>();
            mockedRepo.Setup(m => m.GetAllAsync(It.IsAny<string>())).Returns(GetFakeRepositoryV1());
            var cache = new InMemoryCache(mockedRepo.Object, _settings);

            var filtered = await cache.GetAllPagedAsync("any", "CHF", "OpType2", 1);

            Assert.AreEqual(1, filtered.Count());
        }

        [TestMethod]
        public async Task GetAllByOpTypeAsync_ValidOpType_Success()
        {
            var mockedRepo = new Mock<IHistoryLogEntryRepository>();
            mockedRepo.Setup(m => m.GetAllAsync(It.IsAny<string>())).Returns(GetFakeRepositoryV2);
            var cache = new InMemoryCache(mockedRepo.Object, _settings);

            var filtered = await cache.GetAllByOpTypePagedAsync("any", "OpType3", 1);

            Assert.AreEqual(2, filtered.Count());
        }

        [TestMethod]
        public async Task GetAllByAssetAsync_ValidAsset_Success()
        {
            var mockedRepo = new Mock<IHistoryLogEntryRepository>();
            mockedRepo.Setup(m => m.GetAllAsync(It.IsAny<string>())).Returns(GetFakeRepositoryV2);
            var cache = new InMemoryCache(mockedRepo.Object, _settings);

            var filtered = await cache.GetAllByAssetPagedAsync("any", "RUB", 1);

            Assert.AreEqual(1, filtered.Count());
        }

        private static Task<IList<HistoryLogEntryEntity>> GetFakeRepositoryV1()
        {
            IList<HistoryLogEntryEntity> records = new List<HistoryLogEntryEntity>
            {
                new HistoryLogEntryEntity
                {
                    DateTime = DateTime.Now.AddDays(1),
                    Amount = 1,
                    ClientId = "1",
                    Currency = "USD",
                    OpType = "OpType1",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    DateTime = DateTime.Now.AddDays(2),
                    Amount = 2,
                    ClientId = "1",
                    Currency = "CHF",
                    OpType = "OpType2",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
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
                    DateTime = DateTime.Now.AddDays(1),
                    Amount = 1,
                    ClientId = "1",
                    Currency = "USD",
                    OpType = "OpType1",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    DateTime = DateTime.Now.AddDays(2),
                    Amount = 2,
                    ClientId = "1",
                    Currency = "CHF",
                    OpType = "OpType2",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
                    DateTime = DateTime.Now.AddDays(3),
                    Amount = 3,
                    ClientId = "1",
                    Currency = "EUR",
                    OpType = "OpType3",
                    CustomData = string.Empty
                },
                new HistoryLogEntryEntity
                {
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