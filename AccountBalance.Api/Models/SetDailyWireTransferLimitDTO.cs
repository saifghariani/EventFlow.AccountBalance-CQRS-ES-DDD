using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountBalance.Api.Models
{
    public class SetDailyWireTransferLimitDTO
    {
        public string AccountId { get; set; }
        public float DailyWireTransfer { get; set; }
    }
}
