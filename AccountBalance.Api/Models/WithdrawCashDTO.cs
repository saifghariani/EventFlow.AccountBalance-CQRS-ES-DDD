﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountBalance.Api.Models
{
    public class WithdrawCashDTO
    {
        public string AccountId { get; set; }
        public float Amount { get; set; }
    }
}
