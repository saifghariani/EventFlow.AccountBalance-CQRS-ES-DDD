using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace AccountBalance.Domain.Aggregates.AccountAggregate.Events
{
    [EventVersion("AccountCreate", 1)]
    public class AccountCreatedEvent : AggregateEvent<AccountAggregate, AccountId>
    {
        public AccountCreatedEvent(AccountId accountId, string holderName) : base()
        {
            AccountId = accountId;
            HolderName = holderName;
        }
        public AccountId AccountId { get; set; }
        public string HolderName { get; set; }
    }
}
