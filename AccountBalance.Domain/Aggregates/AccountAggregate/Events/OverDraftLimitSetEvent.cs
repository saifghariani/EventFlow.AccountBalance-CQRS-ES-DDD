using System;
using System.Collections.Generic;
using System.Text;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace AccountBalance.Domain.Aggregates.AccountAggregate.Events
{
    [EventVersion("OverDraftLimitSet", 1)]
    public class OverDraftLimitSetEvent : IAggregateEvent<AccountAggregate, AccountId>
    {
        public OverDraftLimitSetEvent(AccountId accountId, float overDraftLimit)
        {
            AccountId = accountId;
            OverDraftLimit = overDraftLimit;
        }
        public AccountId AccountId { get; set; }
        public float OverDraftLimit { get; set; }
    }
}
