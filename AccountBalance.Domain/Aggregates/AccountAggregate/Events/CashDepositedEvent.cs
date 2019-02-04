using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace AccountBalance.Domain.Aggregates.AccountAggregate.Events
{
    [EventVersion("DepositCash",1)]
    public class CashDepositedEvent : IAggregateEvent<AccountAggregate, AccountId>
    {
        public CashDepositedEvent(AccountId id, float amount)
        {
            AccountId = id;
            Amount = amount;
        }
        public AccountId AccountId { get; set; }
        public float Amount { get; set; }
    }
}
