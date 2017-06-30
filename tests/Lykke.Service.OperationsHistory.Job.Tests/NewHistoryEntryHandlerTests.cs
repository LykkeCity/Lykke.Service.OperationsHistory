using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.JobTriggers.Triggers.Bindings;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Job.Handlers;
using Lykke.Service.OperationsHistory.Job.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.OperationsHistory.Job.Tests
{
    [TestClass]
    public class NewHistoryEntryHandlerTests
    {
        public static HistoryQueueEntry GetValidMessage()
        {
            return new HistoryQueueEntry
            {
                Amount = 10m,
                ClientId = "any client id",
                Currency = "USD",
                CustomData = "any custom data",
                DateTime = DateTime.Now,
                OpType = "any operation type"
            };
        }
        [TestMethod]
        public void Validate_CurrencyIsNullOrEmpty_False()
        {
            var message = GetValidMessage().EmptyCurrency();

            Assert.IsFalse(NewHistoryEntryHandler.Validate(message));
        }
        [TestMethod]
        public void Validate_ClientIdIsNullOrEmpty_False()
        {
            var message = GetValidMessage().EmptyClientId();

            Assert.IsFalse(NewHistoryEntryHandler.Validate(message));
        }
        [TestMethod]
        public void Validate_OperationTypeIsNullOrEmpty_False()
        {
            var message = GetValidMessage().EmptyOperationType();

            Assert.IsFalse(NewHistoryEntryHandler.Validate(message));
        }
        [TestMethod]
        public void Validate_NoNullOrEmptyFields_True()
        {
            var message = GetValidMessage();

            Assert.IsTrue(NewHistoryEntryHandler.Validate(message));
        }
        [TestMethod]
        public async Task ProcessNewEntry_InvalidMessage_ToPoison()
        {
            var logger = new Mock<ILog>();
            var repo = new Mock<IHistoryLogEntryRepository>();
            var invalidMessage = GetValidMessage()
                .EmptyClientId()
                .EmptyCurrency()
                .EmptyOperationType();

            var messagePoisoned = false;
            var mock = new Mock<NewHistoryEntryHandler>(logger.Object, repo.Object) {CallBase = true};
            mock.Setup(m => m.ToPoison(It.IsAny<QueueTriggeringContext>()))
                .Callback(() => messagePoisoned = true);

            await mock.Object.ProcessNewEntry(invalidMessage, It.IsAny<QueueTriggeringContext>());

            Assert.IsTrue(messagePoisoned);
        }
        [TestMethod]
        public async Task ProcessNewEntry_ValidMessage_ToRepository()
        {
            var logger = new Mock<ILog>();
            var repo = new Mock<IHistoryLogEntryRepository>();
            var message = GetValidMessage();

            var messagePoisoned = false;
            var messageAdded = false;
            var mock = new Mock<NewHistoryEntryHandler>(logger.Object, repo.Object) {CallBase = true};
            mock.Setup(m => m.ToPoison(It.IsAny<QueueTriggeringContext>()))
                .Callback(() => messagePoisoned = true);
            mock.Setup(m => m.ToRepository(message))
                .Callback(() => messageAdded = true)
                .Returns(Task.CompletedTask);

            await mock.Object.ProcessNewEntry(message, It.IsAny<QueueTriggeringContext>());

            Assert.IsFalse(messagePoisoned);
            Assert.IsTrue(messageAdded);
        }
    }
}