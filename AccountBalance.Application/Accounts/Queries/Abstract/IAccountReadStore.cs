using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountBalance.Domain.Aggregates.AccountAggregate;

namespace AccountBalance.Application.Accounts.Queries.Abstract
{
    public interface IAccountReadStore
    {
        Task<Account> GetAccountById(GetAccountByIdQuery query, CancellationToken cancellationToken);
        Task<List<Account>> GetAllAccounts(GetAllAccountsQuery query, CancellationToken cancellationToken);
    }
}
