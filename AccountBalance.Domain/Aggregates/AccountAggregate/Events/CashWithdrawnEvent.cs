﻿using System;
using System.Collections.Generic;
using System.Text;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace AccountBalance.Domain.Aggregates.AccountAggregate.Events
{
    [EventVersion("WithdrawCash", 1)]
    public class CashWithdrawnEvent : IAggregateEvent<AccountAggregate, AccountId>
    {
        public CashWithdrawnEvent(AccountId id, float amount)
        {
            AccountId = id;
            Amount = amount;
        }
        public AccountId AccountId { get; set; }
        public float Amount { get; set; }
    }
}
