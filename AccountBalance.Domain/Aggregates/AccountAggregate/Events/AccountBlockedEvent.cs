using System;
using System.Collections.Generic;
using System.Text;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace AccountBalance.Domain.Aggregates.AccountAggregate.Events
{
    [EventVersion("AccountBlocked", 1)]
    public class AccountBlockedEvent : AggregateEvent<AccountAggregate, AccountId>
    {
        public AccountBlockedEvent(AccountId accountId)
        {
            AccountId = accountId;
        }
        public AccountId AccountId { get; set; }
    }
}
