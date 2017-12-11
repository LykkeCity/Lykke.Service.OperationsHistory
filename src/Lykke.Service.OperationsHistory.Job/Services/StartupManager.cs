using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.OperationsHistory.Core.Services;

namespace Lykke.Service.OperationsHistory.Job.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;

        public StartupManager(ILog log)
        {
            _log = log;
        }

        public async Task StartAsync()
        {
            // TODO: Implement your startup logic here. Good idea is to log every step

            await Task.CompletedTask;
        }
    }
}