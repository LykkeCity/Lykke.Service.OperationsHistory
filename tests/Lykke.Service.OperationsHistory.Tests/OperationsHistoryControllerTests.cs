using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.ClientAccount.Client;
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
        private IClientAccountClient _clientAccountService;

        [TestInitialize]
        public void Initialize()
        {
            _cache = new Mock<IHistoryCache>().Object;
            _repo = new Mock<IHistoryLogEntryRepository>().Object;
            _clientAccountService = new Mock<IClientAccountClient>().Object;
        }

        [TestMethod]
        public async Task GetOperationsHistory_ClientIsNullOrEmpty_BadRequest()
        {
            var controller = new OperationsHistoryController(_cache, _repo, _clientAccountService);
            var response = await controller.GetByClientId("", null, null, 1, 1);

            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));

            var messages = GetErrorMessages(response, "clientId");
            var contains = messages?.Contains(OperationsHistoryController.ClientRequiredMsg);
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
