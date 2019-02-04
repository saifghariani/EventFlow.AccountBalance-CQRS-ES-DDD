using EventFlow.Core;
using Newtonsoft.Json;

namespace AccountBalance.Domain.Aggregates.AccountAggregate
{
    public class AccountId : Identity<AccountId>
    {
        public AccountId(string value) : base(value)
        {
        }
    }
}