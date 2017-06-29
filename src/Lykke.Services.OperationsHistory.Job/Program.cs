using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Services.OperationsHistory.Job
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var job = new HistoryJob();
            job.Run();
        }
    }
}
