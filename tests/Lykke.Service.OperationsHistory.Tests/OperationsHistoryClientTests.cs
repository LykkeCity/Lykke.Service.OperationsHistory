using Lykke.Service.OperationsHistory.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.OperationsHistory.Tests
{
    [TestClass]
    public class OperationsHistoryClientTests
    {
        [TestMethod]
        public void CreateMapper_ValidConfiguration()
        {
            var mapper = OperationsHistoryClient.CreateMapper();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
