using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.OperationsHistory.Tests
{
    [TestClass]
    public class TestsConfig
    {
        public static IServiceProvider Services { get; set; }
        public static ILog Logger => Services.GetService<ILog>();
        [AssemblyInitialize]
        public static void Init(TestContext context)
        {
            var log = new LogToConsole();
            var cBuilder = new ContainerBuilder();

            cBuilder.RegisterInstance(log).As<ILog>();

            Services = new AutofacServiceProvider(cBuilder.Build());
        }
    }
}
