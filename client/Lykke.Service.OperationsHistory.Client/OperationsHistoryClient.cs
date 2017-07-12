using System;
using Common.Log;

namespace Lykke.Service.OperationsHistory.Client
{
    public class OperationsHistoryClient : IOperationsHistoryClient, IDisposable
    {
        private readonly ILog _log;

        public OperationsHistoryClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
