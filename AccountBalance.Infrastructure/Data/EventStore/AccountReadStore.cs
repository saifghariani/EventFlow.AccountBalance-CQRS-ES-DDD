using AccountBalance.Application.Accounts.Queries;
using AccountBalance.Application.Accounts.Queries.Abstract;
using AccountBalance.Domain.Aggregates.AccountAggregate;
using AccountBalance.Domain.Global;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Infrastructure.Data.EventStore
{
    public class AccountReadStore : IAccountReadStore
    {
        //private readonly string _streamName = StreamNames.AccountStream;
        private readonly UserCredentials _adminCredentials = Credentials.AdminCredentials;

        #region Connect to EventStore

        public async Task<IEventStoreConnection> Connect()
        {
            var conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            await conn.ConnectAsync();
            return conn;
        }

        #endregion

        public async Task<Account> GetAccountById(GetAccountByIdQuery query, CancellationToken cancellationToken)
        {
            var conn = await Connect();
            var streamName = query.AccountId.Value;
            Account _account = new Account(AccountId.With(streamName));
            var streamEvents = conn.ReadStreamEventsForwardAsync(streamName, 0, 100, true, _adminCredentials).Result;
            foreach (var evt in streamEvents.Events) {
                var json = Encoding.UTF8.GetString(evt.Event.Data);
                var account = JsonConvert.DeserializeObject<AccountReadModel>(json);
                if (!string.IsNullOrEmpty(account.HolderName)) {
                    _account.HolderName = account.HolderName;
                    _account.Balance = account.Balance;
                }

                if (!string.IsNullOrEmpty(account.AccountState))
                    _account.AccountState = account.AccountState;


                if (!account.Amount.Equals(default(float))) {
                    if (evt.Event.EventType.ToLower().Contains("withdraw"))
                        _account.Balance -= account.Amount;

                    if (evt.Event.EventType.ToLower().Contains("deposit")) {

                        if (evt.Event.EventType.ToLower().Contains("cash"))
                            _account.Balance += account.Amount;

                        if (evt.Event.EventType.ToLower().Contains("check")) {
                            var date = evt.Event.Created;
                            var depositDate = date.AddDays(1);
                            while (!(depositDate.DayOfWeek >= DayOfWeek.Monday && depositDate.DayOfWeek <= DayOfWeek.Friday)) {
                                depositDate = depositDate.AddDays(1);
                            }
                            if (DateTime.UtcNow.Date >= depositDate.Date)
                                _account.Balance += account.Amount;
                        }

                    }
                }

                if (!account.DailyWireTransferLimit.Equals(default(float)))
                    _account.DailyWireTransferLimit = account.DailyWireTransferLimit;

                if (!account.OverDraftLimit.Equals(default(float)))
                    _account.OverDraftLimit = account.OverDraftLimit;
            }
            conn.Close();
            return _account;
        }

        public async Task<List<Account>> GetAllAccounts(GetAllAccountsQuery query, CancellationToken cancellationToken)
        {
            var conn = await Connect();
            List<Account> _accounts = new List<Account>();
            ProjectionsManager pm = new ProjectionsManager(new ConsoleLogger(), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2113), TimeSpan.FromMilliseconds(5000));
            CreateProjection(pm);

            var streams = await pm.GetStateAsync("getAllStreams", _adminCredentials);
            var streamNames = JsonConvert.DeserializeObject<Streams>(streams);
            foreach (var streamName in streamNames.StreamNames) {
                Account _account = new Account(AccountId.With(streamName));
                var streamEvents = conn.ReadStreamEventsForwardAsync(streamName, 0, 100, true, _adminCredentials).Result;
                foreach (var evt in streamEvents.Events) {
                    var json = Encoding.UTF8.GetString(evt.Event.Data);
                    var account = JsonConvert.DeserializeObject<AccountReadModel>(json);
                    if (!string.IsNullOrEmpty(account.HolderName)) {
                        _account.HolderName = account.HolderName;
                        _account.Balance = account.Balance;
                    }

                    if (!string.IsNullOrEmpty(account.AccountState))
                        _account.AccountState = account.AccountState;

                    if (!account.Amount.Equals(default(float))) {
                        if (evt.Event.EventType.ToLower().Contains("withdraw"))
                            _account.Balance -= account.Amount;

                        if (evt.Event.EventType.ToLower().Contains("deposit")) {

                            if (evt.Event.EventType.ToLower().Contains("cash"))
                                _account.Balance += account.Amount;

                            if (evt.Event.EventType.ToLower().Contains("check")) {
                                var date = evt.Event.Created;
                                var depositDate = date.AddDays(1);
                                while (!(depositDate.DayOfWeek >= DayOfWeek.Monday && depositDate.DayOfWeek <= DayOfWeek.Friday)) {
                                    depositDate = depositDate.AddDays(1);
                                }
                                if (DateTime.UtcNow.Date >= depositDate.Date)
                                    _account.Balance += account.Amount;
                            }

                        }
                    }

                    if (!account.DailyWireTransferLimit.Equals(default(float)))
                        _account.DailyWireTransferLimit = account.DailyWireTransferLimit;

                    if (!account.OverDraftLimit.Equals(default(float)))
                        _account.OverDraftLimit = account.OverDraftLimit;

                }
                _accounts.Add(_account);
            }
            conn.Close();
            return _accounts;
        }

        public async Task<float> GetTodayWithdraw(GetTodayWithdrawQuery query, CancellationToken cancellationToken)
        {
            var conn = await Connect();
            float todayWithdraw = default(float);
            var streamEvents = conn.ReadStreamEventsForwardAsync(query.AccountId.Value, 0, 100, true, _adminCredentials).Result;
            foreach (var evt in streamEvents.Events) {
                var json = Encoding.UTF8.GetString(evt.Event.Data);
                var account = JsonConvert.DeserializeObject<AccountReadModel>(json);
                if (!account.Amount.Equals(default(float))) {
                    if (evt.Event.EventType.ToLower().Contains("withdraw") && evt.Event.Created.Date == DateTime.Today.Date)
                        todayWithdraw += account.Amount;
                }
            }
            conn.Close();
            return todayWithdraw;
        }

        public async void CreateProjection(ProjectionsManager pm)
        {
            var getAllStreams = @"options({
            resultStreamName: 'getAllStreams'
            })
            fromAll()
            .when({
                $init: function(){
                    return {
                        streamNames : []
                    }
                },
                
                AccountCreatedEvent: function(s, e){
                                 s.streamNames.push(e.streamId);
                            }
                        }).outputState()";
            var projections = await pm.ListContinuousAsync(_adminCredentials);
            if (!projections.Exists(p => p.EffectiveName == "getAllStreams")) {
                await pm.CreateContinuousAsync("getAllStreams", getAllStreams, true, _adminCredentials);
            }

        }



        //public void BuildingAccount(AccountReadModel account, AccountId accountId, out Account _account)
        //{
        //    _account = new Account(accountId);

        //    if (!string.IsNullOrEmpty(account.HolderName)) {
        //        _account.HolderName = account.HolderName;
        //        _account.Balance = account.Balance;
        //        _account.AccountState = account.AccountState;
        //    }
        //    if (!account.Balance.Equals(default(float))) {
        //        _account.Balance = account.Balance;
        //    }
        //    if (!account.DailyWireTransferLimit.Equals(default(float))) {
        //        _account.DailyWireTransferLimit = account.DailyWireTransferLimit;
        //    }
        //    if (!account.OverDraftLimit.Equals(default(float))) {
        //        _account.OverDraftLimit = account.OverDraftLimit;
        //    }
        //}

        public class Streams
        {
            public List<string> StreamNames { get; set; }
        }
    }
}
