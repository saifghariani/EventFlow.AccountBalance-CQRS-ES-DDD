using System;
using System.Collections.Generic;
using System.Text;
using AccountBalance.Domain.Aggregates.AccountAggregate.Enumerations;
using EventFlow.Snapshots;

namespace AccountBalance.Domain.Aggregates.AccountAggregate.Snapshots
{
    [SnapshotVersion("account", 1)]
    public class AccountSnapshot : ISnapshot
    {
        public AccountSnapshot(string holderName, float overdraftLimit, float dailyWireTransfertLimit, string accountState)
        {
            HolderName = holderName;
            OverdraftLimit = overdraftLimit;
            DailyWireTransferLimit = dailyWireTransfertLimit;
            AccountState = accountState;
        }
        public string HolderName { get; set; }
        public float OverdraftLimit { get; set; }
        public float DailyWireTransferLimit { get; set; }
        public string AccountState { get; set; }
    }
}
