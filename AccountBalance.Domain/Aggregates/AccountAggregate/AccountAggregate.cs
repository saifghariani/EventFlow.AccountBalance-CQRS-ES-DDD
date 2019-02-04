using AccountBalance.Domain.Aggregates.AccountAggregate.Events;
using AccountBalance.Domain.Aggregates.AccountAggregate.Snapshots;
using EventFlow.Aggregates;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Strategies;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Domain.Aggregates.AccountAggregate
{
    public class AccountAggregate : SnapshotAggregateRoot<AccountAggregate, AccountId, AccountSnapshot>
        , IEmit<AccountCreatedEvent>
        , IEmit<DailyWireTransferSetEvent>
        , IEmit<OverDraftLimitSetEvent>
        , IEmit<CashWithdrawnEvent>
        , IEmit<CashDepositedEvent>
        , IEmit<AccountBlockedEvent>
        , IEmit<AccountUnblockedEvent>
    {
        public const int SnapshotEveryVersion = 10;

        public AccountAggregate(AccountId accountId) : base(accountId, SnapshotEveryFewVersionsStrategy.With(SnapshotEveryVersion))
        {
            Register<AccountCreatedEvent>(Apply);
            Register<DailyWireTransferSetEvent>(Apply);
            Register<OverDraftLimitSetEvent>(Apply);
            Register<CashWithdrawnEvent>(Apply);
            Register<CashDepositedEvent>(Apply);
            Register<CheckDepositedEvent>(Apply);
            Register<AccountBlockedEvent>(Apply);
            Register<AccountUnblockedEvent>(Apply);
        }

       




        #region Apply

        public void Apply(AccountCreatedEvent @event)
        {

            HolderName = @event.HolderName;
            AccountState = Enumerations.AccountState.Actif;
            Balance = default(float);
        }

        public void Apply(DailyWireTransferSetEvent @event)
        {
            DailyWireTransferLimit = @event.DailyWireTransfer;
        }

        public void Apply(OverDraftLimitSetEvent @event)
        {
            OverdraftLimit = @event.OverDraftLimit;
        }

        public void Apply(CashWithdrawnEvent @event)
        {
            Balance -= @event.Amount;
            if (Balance < 0 && Math.Abs(Balance) > OverdraftLimit) {
                AccountState = Enumerations.AccountState.Blocked;
            }
        }

        public void Apply(AccountUnblockedEvent @event)
        {
            AccountState = Enumerations.AccountState.Actif;
        }

        public void Apply(AccountBlockedEvent @event)
        {
            AccountState = Enumerations.AccountState.Blocked;
        }
        public void Apply(CashDepositedEvent @event)
        {
            Balance += @event.Amount;
            if (Balance >= 0 && AccountState == Enumerations.AccountState.Blocked) {
                AccountState = Enumerations.AccountState.Actif;
            }
        }
        public void Apply(CheckDepositedEvent @event)
        {
            Balance += @event.Amount;
            if (Balance >= 0 && AccountState == Enumerations.AccountState.Blocked) {
                AccountState = Enumerations.AccountState.Actif;
            }
        }

        #endregion

        #region Raise Events

        public void CreateAccount(string accountId, string holderName)
        {
            Emit(new AccountCreatedEvent(AccountId.With(accountId), holderName));
        }

        public void SetDailyWireTransferLimit(string accountId, float dailyWireTransferLimit)
        {
            Emit(new DailyWireTransferSetEvent(AccountId.With(accountId), dailyWireTransferLimit));
        }

        public void SetOverDraftLimit(string accountId, float overDraftLimit)
        {
            Emit(new OverDraftLimitSetEvent(AccountId.With(accountId), overDraftLimit));
        }

        public void WithdrawCash(string accountId, float amount)
        {
            Emit(new CashWithdrawnEvent(AccountId.With(accountId), amount));
        }
        public void DepositCash(string accountId, float amount)
        {
            Emit(new CashDepositedEvent(AccountId.With(accountId), amount));
        }
        public void DepositCheck(string accountId, float amount)
        {
            Emit(new CheckDepositedEvent(AccountId.With(accountId), amount));
        }
        public void BlockAccount(string accountId)
        {
            Emit(new AccountBlockedEvent(AccountId.With(accountId)));
        }

        public void UnblockAccount(string accountId)
        {
            Emit(new AccountUnblockedEvent(AccountId.With(accountId)));
        }

        #endregion

        #region Snapshot

        protected override Task<AccountSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
        {
            var snapshot = new AccountSnapshot(HolderName,
                OverdraftLimit,
                DailyWireTransferLimit,
                AccountState);

            return Task.FromResult(snapshot);
        }

        protected override Task LoadSnapshotAsync(AccountSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
        {
            HolderName = snapshot.HolderName;
            OverdraftLimit = snapshot.OverdraftLimit;
            DailyWireTransferLimit = snapshot.DailyWireTransferLimit;
            AccountState = snapshot.AccountState;
            return Task.FromResult(0);
        }

        #endregion

        #region Properties
        public string HolderName { get; set; }
        public float OverdraftLimit { get; set; }
        public float Balance { get; set; }
        public float DailyWireTransferLimit { get; set; }
        public string AccountState { get; set; }
        #endregion
    }
}
