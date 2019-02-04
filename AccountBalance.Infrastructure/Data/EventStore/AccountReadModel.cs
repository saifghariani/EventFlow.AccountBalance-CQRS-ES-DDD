using AccountBalance.Domain.Aggregates.AccountAggregate;
using AccountBalance.Domain.Aggregates.AccountAggregate.Events;
using AccountBalance.Domain.Global;
using EventFlow.Aggregates;
using EventFlow.ReadStores;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using System;
using System.Text;

namespace AccountBalance.Infrastructure.Data.EventStore
{
    public class AccountReadModel : IReadModel
    , IAmReadModelFor<AccountAggregate, AccountId, AccountCreatedEvent>
    , IAmReadModelFor<AccountAggregate, AccountId, DailyWireTransferSetEvent>
    , IAmReadModelFor<AccountAggregate, AccountId, OverDraftLimitSetEvent>
    , IAmReadModelFor<AccountAggregate, AccountId, CashWithdrawnEvent>
    , IAmReadModelFor<AccountAggregate, AccountId, CashDepositedEvent>
    , IAmReadModelFor<AccountAggregate, AccountId, CheckDepositedEvent>
    , IAmReadModelFor<AccountAggregate, AccountId, AccountBlockedEvent>
    , IAmReadModelFor<AccountAggregate, AccountId, AccountUnblockedEvent>
    {
        private readonly UserCredentials _adminCredentials = Credentials.AdminCredentials;

        #region Connect to EventStore

        public IEventStoreConnection Connect()
        {
            var conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            conn.ConnectAsync().Wait();
            return conn;
        }

        #endregion

        #region Apply

        public void Apply(IReadModelContext context, IDomainEvent<AccountAggregate, AccountId, AccountCreatedEvent> domainEvent)
        {
            var conn = Connect();
            var streamName = domainEvent.AggregateIdentity.Value;
            AccountId = domainEvent.AggregateEvent.AccountId.Value;
            HolderName = domainEvent.AggregateEvent.HolderName;
            Balance = default(float);
            AccountState = Domain.Aggregates.AccountAggregate.Enumerations.AccountState.Actif;

            var obj = new {
                accountId = AccountId,
                holderName = HolderName,
                balance = Balance,
                accountState = AccountState
            };
            var account = JsonConvert.SerializeObject(obj);
            EventData eventData = new EventData(_accountId.GetGuid(), domainEvent.EventType.Name, true, Encoding.UTF8.GetBytes(account), null);
            conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, _adminCredentials, eventData).Wait();
            conn.Close();
        }

        public void Apply(IReadModelContext context, IDomainEvent<AccountAggregate, AccountId, DailyWireTransferSetEvent> domainEvent)
        {
            var conn = Connect();
            var streamName = domainEvent.AggregateIdentity.Value;
            DailyWireTransferLimit = domainEvent.AggregateEvent.DailyWireTransfer;
            var obj = new {
                dailyWireTransferLimit = DailyWireTransferLimit
            };
            var account = JsonConvert.SerializeObject(obj);
            EventData eventData = new EventData(Guid.NewGuid(), domainEvent.EventType.Name, true, Encoding.UTF8.GetBytes(account), null);
            conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, _adminCredentials, eventData).Wait();
            conn.Close();
        }

        public void Apply(IReadModelContext context, IDomainEvent<AccountAggregate, AccountId, OverDraftLimitSetEvent> domainEvent)
        {
            var conn = Connect();
            var streamName = domainEvent.AggregateIdentity.Value;
            OverDraftLimit = domainEvent.AggregateEvent.OverDraftLimit;
            var obj = new {
                overDraftLimit = OverDraftLimit
            };
            var account = JsonConvert.SerializeObject(obj);
            EventData eventData = new EventData(Guid.NewGuid(), domainEvent.EventType.Name, true, Encoding.UTF8.GetBytes(account), null);
            conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, _adminCredentials, eventData).Wait();
            conn.Close();
        }

        public void Apply(IReadModelContext context, IDomainEvent<AccountAggregate, AccountId, CashWithdrawnEvent> domainEvent)
        {
            var conn = Connect();
            var streamName = domainEvent.AggregateIdentity.Value;
            var obj = new {
                amount = domainEvent.AggregateEvent.Amount
            };
            var withdraw = JsonConvert.SerializeObject(obj);
            EventData eventData = new EventData(Guid.NewGuid(), domainEvent.EventType.Name, true, Encoding.UTF8.GetBytes(withdraw), null);
            conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, _adminCredentials, eventData).Wait();
            conn.Close();
        }

        public void Apply(IReadModelContext context, IDomainEvent<AccountAggregate, AccountId, CashDepositedEvent> domainEvent)
        {
            var conn = Connect();
            var streamName = domainEvent.AggregateIdentity.Value;
            var obj = new {
                amount = domainEvent.AggregateEvent.Amount
            };
            var deposit = JsonConvert.SerializeObject(obj);
            EventData eventData = new EventData(Guid.NewGuid(), domainEvent.EventType.Name, true, Encoding.UTF8.GetBytes(deposit), null);
            conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, _adminCredentials, eventData).Wait();
            conn.Close();
        }
        public void Apply(IReadModelContext context, IDomainEvent<AccountAggregate, AccountId, CheckDepositedEvent> domainEvent)
        {
            var conn = Connect();
            var streamName = domainEvent.AggregateIdentity.Value;
            var obj = new {
                amount = domainEvent.AggregateEvent.Amount
            };
            var deposit = JsonConvert.SerializeObject(obj);
            EventData eventData = new EventData(Guid.NewGuid(), domainEvent.EventType.Name, true, Encoding.UTF8.GetBytes(deposit), null);
            conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, _adminCredentials, eventData).Wait();
            conn.Close();
        }

        public void Apply(IReadModelContext context, IDomainEvent<AccountAggregate, AccountId, AccountBlockedEvent> domainEvent)
        {
            var conn = Connect();
            var streamName = domainEvent.AggregateIdentity.Value;
            var obj = new {
                AccountState = Domain.Aggregates.AccountAggregate.Enumerations.AccountState.Blocked
            };
            var account = JsonConvert.SerializeObject(obj);
            EventData eventData = new EventData(Guid.NewGuid(), domainEvent.EventType.Name, true, Encoding.UTF8.GetBytes(account), null);
            conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, _adminCredentials, eventData).Wait();
            conn.Close();
        }

        public void Apply(IReadModelContext context, IDomainEvent<AccountAggregate, AccountId, AccountUnblockedEvent> domainEvent)
        {
            var conn = Connect();
            var streamName = domainEvent.AggregateIdentity.Value;
            var obj = new {
                AccountState = Domain.Aggregates.AccountAggregate.Enumerations.AccountState.Actif
            };
            var account = JsonConvert.SerializeObject(obj);
            EventData eventData = new EventData(Guid.NewGuid(), domainEvent.EventType.Name, true, Encoding.UTF8.GetBytes(account), null);
            conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, _adminCredentials, eventData).Wait();
            conn.Close();
        }

        #endregion

        public Account ToAccount()
        {
            return new Account(
                _accountId,
                HolderName,
                Balance,
                OverDraftLimit,
                DailyWireTransferLimit,
                AccountState
                );
        }

        

        #region Properties

        public string AccountId { get; set; }
        private AccountId _accountId { get { return string.IsNullOrEmpty(AccountId) ? null : new AccountId(AccountId); } set { AccountId = value.Value; } }
        public string HolderName { get; set; }
        public float DailyWireTransferLimit { get; set; }
        public float OverDraftLimit { get; set; }
        public float Balance { get; set; }
        public float Amount { get; set; }
        public string AccountState { get; set; }

        #endregion

       
    }
}
