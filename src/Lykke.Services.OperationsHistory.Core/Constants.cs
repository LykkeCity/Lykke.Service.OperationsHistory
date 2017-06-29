using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Services.OperationsHistory.Core
{
    public class Constants
    {
        public const string InQueueName = "historyqueue";
        public const string OutTableName = "historylog";
        public const string JobPoisonQueueName = "historyjob-poisonqueue";
    }
}
