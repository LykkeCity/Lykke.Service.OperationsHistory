using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.OperationsHistory.Models
{
    public class EditHistoryEntryModel
    {
        public string Id { get; set; }
        public string BlockChainHash { get; set; }
        public int State { get; set; }
    }
}
