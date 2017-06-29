using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Services.OperationsHistory.Job.Tests
{
    [TestClass]
    public class HistoryJobTests
    {
        public static IServiceProvider Services { get; set; }
        //public static ILog Logger = Services.GetService<ILog>();
        [AssemblyInitialize]
        public static void Init(TestContext context)
        {
            //var cBuilder = new ContainerBuilder();
        }
        [TestMethod]
        public void TestValidate()
        {
        }
    }
}
