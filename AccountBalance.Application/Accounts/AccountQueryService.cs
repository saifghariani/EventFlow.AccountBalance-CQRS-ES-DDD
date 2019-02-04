using AccountBalance.Application.Accounts.Commands;
using AccountBalance.Application.Accounts.Queries;
using AccountBalance.Application.Interfaces;
using AccountBalance.Domain.Aggregates.AccountAggregate;
using AccountBalance.Domain.Aggregates.AccountAggregate.Enumerations;
using EventFlow;
using EventFlow.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Application.Accounts
{
    public class AccountQueryService : IAccountQueryService
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandBus _commandBus;

        public AccountQueryService(IQueryProcessor queryProcessor, ICommandBus commandBus)
        {
            _queryProcessor = queryProcessor;
            _commandBus = commandBus;
        }

        public async Task<Account> GetAccountByIdAsync(AccountId accountId)
        {
            var account = await _queryProcessor.ProcessAsync(new GetAccountByIdQuery(accountId), CancellationToken.None);
            if (account.Balance >= 0 && account.AccountState == AccountState.Blocked) {
                await _commandBus.PublishAsync(new UnblockAccountCommand(accountId), CancellationToken.None);
                account.AccountState = AccountState.Actif;
            }

            return account;
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            var accounts = await _queryProcessor.ProcessAsync(new GetAllAccountsQuery(), CancellationToken.None);
            foreach (var account in accounts) {
                if (account.Balance >= 0 && account.AccountState == AccountState.Blocked) {
                    await _commandBus.PublishAsync(new UnblockAccountCommand(account.Id), CancellationToken.None);
                    account.AccountState = AccountState.Actif;
                }
            }
            return accounts;
        }
    }
}
