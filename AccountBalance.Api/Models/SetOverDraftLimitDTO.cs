using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountBalance.Api.Models
{
    public class SetOverDraftLimitDTO
    {
        public string AccountId { get; set; }
        public float OverDraftLimit { get; set; }
    }
}
