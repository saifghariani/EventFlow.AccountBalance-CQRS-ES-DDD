using System;
using System.Collections.Generic;
using System.Text;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace AccountBalance.Domain.Aggregates.AccountAggregate.Events
{
    [EventVersion("AccountUnblocked", 1)]
    public class AccountUnblockedEvent : AggregateEvent<AccountAggregate, AccountId>
    {
        public AccountUnblockedEvent(AccountId accountId)
        {
            AccountId = accountId;
        }
        public AccountId AccountId { get; set; }
    }
}
