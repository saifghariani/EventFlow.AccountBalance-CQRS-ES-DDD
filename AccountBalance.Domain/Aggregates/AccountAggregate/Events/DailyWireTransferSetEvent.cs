using System;
using System.Collections.Generic;
using System.Text;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace AccountBalance.Domain.Aggregates.AccountAggregate.Events
{
    [EventVersion("DailyWireTransferSet", 1)]
    public class DailyWireTransferSetEvent : IAggregateEvent<AccountAggregate, AccountId>
    {
        public DailyWireTransferSetEvent(AccountId accountId, float dailyWireTransfer)
        {
            AccountId = accountId;
            DailyWireTransfer = dailyWireTransfer;
        }
        public AccountId AccountId { get; set; }
        public float DailyWireTransfer { get; set; }
    }
}
