using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AccountBalance.Domain.Aggregates.AccountAggregate;

namespace AccountBalance.Application.Interfaces
{
    public interface IAccountApplicationService
    {
        Task<AccountId> CreateAccountAsync(string holderName);
        Task SetDailyWireTransferLimitAsync(AccountId accountId, float dailyWireTransferLimit);
        Task SetOverDraftLimitAsync(AccountId accountId, float overDraftLimit);
        Task WithdrawCashAsync(AccountId accountId, float amount);
        Task DepositCashAsync(AccountId accountId, float amount);
        Task DepositCheckAsync(AccountId accountId, float amount);
    }
}
