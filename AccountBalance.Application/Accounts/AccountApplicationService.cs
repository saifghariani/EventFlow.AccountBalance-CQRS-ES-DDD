using AccountBalance.Application.Accounts.Commands;
using AccountBalance.Application.Interfaces;
using AccountBalance.Domain.Aggregates.AccountAggregate;
using AccountBalance.Domain.Aggregates.AccountAggregate.Enumerations;
using EventFlow;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Application.Accounts
{
    public class AccountApplicationService : IAccountApplicationService
    {
        private readonly ICommandBus _commandBus;
        private readonly IAccountQueryService _accountQueryService;

        public AccountApplicationService(ICommandBus commandBus, IAccountQueryService accountQueryService)
        {
            _commandBus = commandBus;
            _accountQueryService = accountQueryService;
        }

        public async Task<AccountId> CreateAccountAsync(string holderName)
        {
            var accountId = AccountId.New;
            await _commandBus.PublishAsync(new CreateAccountCommand(accountId, holderName), CancellationToken.None)
                .ConfigureAwait(false);
            return accountId;
        }



        public async Task SetDailyWireTransferLimitAsync(AccountId accountId, float dailyWireTransferLimit)
        {
            if (dailyWireTransferLimit < 0)
            {
                throw new Exception("Daily Wire Transfer Limit cannot be a negative value");
            }
            await _commandBus.PublishAsync(new SetDailyWireTransferLimitCommand(accountId, dailyWireTransferLimit), CancellationToken.None)
                .ConfigureAwait(false);
        }

        public async Task SetOverDraftLimitAsync(AccountId accountId, float overDraftLimit)
        {
            if (overDraftLimit < 0) {
                throw new Exception("Over Draft Limit cannot be a negative value");
            }
            await _commandBus.PublishAsync(new SetOverDraftLimitCommand(accountId, overDraftLimit), CancellationToken.None)
                .ConfigureAwait(false);
        }

        public async Task WithdrawCashAsync(AccountId accountId, float amount)
        {
            if (amount < 0) {
                throw new Exception("Cannot withdraw a negative value");
            }

            var account = await _accountQueryService.GetAccountByIdAsync(accountId);

            if (account.AccountState == AccountState.Blocked)
                throw new Exception("Unable to Withdraw : Blocked Account.");
            var withdrawnToday = await _accountQueryService.GetTodayWithdrawAsync(accountId);
            if (withdrawnToday + amount > account.DailyWireTransferLimit)
            {
                var leftToWithdraw = account.DailyWireTransferLimit - withdrawnToday;
                throw new Exception("Unable to Withdraw : DailyLimit reached || You have : "+ leftToWithdraw + " left to withdraw today.");
            }

            await _commandBus.PublishAsync(new WithdrawCashCommand(accountId, amount), CancellationToken.None)
                    .ConfigureAwait(false);

            account.Balance -= amount; 
            if (account.Balance < 0 && Math.Abs(account.Balance) > account.OverDraftLimit)
                await _commandBus.PublishAsync(new BlockAccountCommand(accountId), CancellationToken.None);

        }

        public async Task DepositCashAsync(AccountId accountId, float amount)
        {
            if (amount < 0) {
                throw new Exception("Cannot deposit a negative value");
            }

            await _commandBus.PublishAsync(new DepositCashCommand(accountId, amount), CancellationToken.None)
                .ConfigureAwait(false);

            var account = await _accountQueryService.GetAccountByIdAsync(accountId);
            if (account.Balance >= 0 && account.AccountState == AccountState.Blocked)
                await _commandBus.PublishAsync(new UnblockAccountCommand(accountId), CancellationToken.None);
        }

        public async Task DepositCheckAsync(AccountId accountId, float amount)
        {
            if (amount < 0) {
                throw new Exception("Cannot deposit a negative value");
            }

            await _commandBus.PublishAsync(new DepositCheckCommand(accountId, amount), CancellationToken.None)
                .ConfigureAwait(false);

            var account = await _accountQueryService.GetAccountByIdAsync(accountId);
            if (account.Balance >= 0 && account.AccountState == AccountState.Blocked)
                await _commandBus.PublishAsync(new UnblockAccountCommand(accountId), CancellationToken.None);
        }
    }
}
