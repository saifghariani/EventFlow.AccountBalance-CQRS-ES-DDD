using AccountBalance.Domain.Aggregates.AccountAggregate.Enumerations;
using System;
using EventFlow.Entities;
using Newtonsoft.Json;

namespace AccountBalance.Domain.Aggregates.AccountAggregate
{
    public class Account : Entity<AccountId>
    {
        public Account(AccountId accountId) :base(accountId)
        {
        }

        [JsonConstructor]
        public Account(AccountId accountId, string holderName,float balance, float overDraftLimit, float dailyWireTransferLimit, string accountState):base(accountId)
        {
            HolderName = holderName;
            Balance = balance;
            OverDraftLimit = overDraftLimit;
            DailyWireTransferLimit = dailyWireTransferLimit;
            AccountState = accountState;
        }

        public string HolderName { get; set; }
        public float Balance { get; set; }
        public float OverDraftLimit { get; set; }
        public float DailyWireTransferLimit { get; set; }
        public string AccountState { get; set; }
    }
}
