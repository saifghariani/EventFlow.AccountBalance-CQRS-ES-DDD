using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AccountBalance.Domain.Aggregates.AccountAggregate;

namespace AccountBalance.Application.Interfaces
{
    public interface IAccountQueryService
    {
        Task<Account> GetAccountByIdAsync(AccountId accountId);
        Task<List<Account>> GetAllAccountsAsync();
        Task<float> GetTodayWithdrawAsync(AccountId accountId);
    }
}
