using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.JobTriggers.Abstractions;
using Lykke.Services.OperationsHistory.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Services.OperationsHistory.Job.Tests
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
