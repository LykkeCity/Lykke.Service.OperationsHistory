using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lykke.Service.OperationsHistory.Controllers;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Models;
using Lykke.Service.OperationsHistory.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Lykke.Service.OperationsHistory.Tests
{
    [TestClass]
    public class OperationsHistoryControllerTests
    {
        private IHistoryCache _cache;
        private IHistoryLogEntryRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _cache = new Mock<IHistoryCache>().Object;
            _repo = new Mock<IHistoryLogEntryRepository>().Object;
        }

        [TestMethod]
        public async Task GetOperationsHistory_ClientIsNullOrEmpty_BadRequest()
        {
            var controller = new OperationsHistoryController(_cache, _repo);
            var response = await controller.GetOperationsHistory("");

            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));

            var messages = GetErrorMessages(response, "clientId");
            var contains = messages?.Contains(OperationsHistoryController.ClientRequiredMsg);
            Assert.IsTrue(contains ?? false);
        }

        [TestMethod]
        public async Task GetOperationsHistory_OpTypeIsNullOrEmpty_BadRequest()
        {
            var controller = new OperationsHistoryController(_cache, _repo);
            var response = await controller.GetOperationsHistory("clientId", "", "assetId");

            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));

            var messages = GetErrorMessages(response, "operationType");
            var contains = messages?.Contains(OperationsHistoryController.OpTypeRequired);
            Assert.IsTrue(contains ?? false);
        }

        [TestMethod]
        public async Task GetOperationsHistory_AssetIsNullOrEmpty_BadRequest()
        {
            var controller = new OperationsHistoryController(_cache, _repo);
            var response = await controller.GetOperationsHistory("clientId", "opType", "");

            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));

            var messages = GetErrorMessages(response, "assetId");
            var contains = messages?.Contains(OperationsHistoryController.AssetRequired);
            Assert.IsTrue(contains ?? false);
        }

        [TestMethod]
        public async Task GetOperationsHistoryByOpType_ClientIsNullOrEmpty_BadRequest()
        {
            var controller = new OperationsHistoryController(_cache, _repo);
            var response = await controller.GetOperationsHistoryByOpType("", "opType");

            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));

            var messages = GetErrorMessages(response, "clientId");
            var contains = messages?.Contains(OperationsHistoryController.ClientRequiredMsg);
            Assert.IsTrue(contains ?? false);
        }

        [TestMethod]
        public async Task GetOperationsHistoryByOpType_OpTypeIsNullOrEmpty_BadRequest()
        {
            var controller = new OperationsHistoryController(_cache, _repo);
            var response = await controller.GetOperationsHistoryByOpType("clientId", "");

            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));

            var messages = GetErrorMessages(response, "operationType");
            var contains = messages?.Contains(OperationsHistoryController.OpTypeRequired);
            Assert.IsTrue(contains ?? false);
        }

        [TestMethod]
        public async Task GetOperationsHistoryByAsset_ClientIsNullOrEmpty_BadRequest()
        {
            var controller = new OperationsHistoryController(_cache, _repo);
            var response = await controller.GetOperationsHistoryByAsset("", "opType");

            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));

            var messages = GetErrorMessages(response, "clientId");
            var contains = messages?.Contains(OperationsHistoryController.ClientRequiredMsg);
            Assert.IsTrue(contains ?? false);
        }

        [TestMethod]
        public async Task GetOperationsHistoryByAsset_AssetIsNullOrEmpty_BadRequest()
        {
            var controller = new OperationsHistoryController(_cache, _repo);
            var response = await controller.GetOperationsHistoryByAsset("clientId", "");

            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));

            var messages = GetErrorMessages(response, "assetId");
            var contains = messages?.Contains(OperationsHistoryController.AssetRequired);
            Assert.IsTrue(contains ?? false);
        }

        private static IEnumerable<string> GetErrorMessages(IActionResult response, string key)
        {
            var badRequest = response as BadRequestObjectResult;
            var errorResponse = badRequest?.Value as ErrorResponse;

            return errorResponse?.ErrorMessages[key];
        }
    }
}
