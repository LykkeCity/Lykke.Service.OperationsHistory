using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.OperationsHistory.Core.Services;

namespace Lykke.Service.OperationsHistory.Job.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;

        public ShutdownManager(ILog log)
        {
            _log = log;
        }

        public async Task StopAsync()
        {
            // TODO: Implement your shutdown logic here. Good idea is to log every step

            await Task.CompletedTask;
        }
    }
}