using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountBalance.Application.Accounts.Queries.Abstract;
using AccountBalance.Domain.Aggregates.AccountAggregate;
using EventFlow.Queries;

namespace AccountBalance.Application.Accounts.Queries
{
    public class GetTodayWithdrawQuery : IQuery<float>
    {
        public GetTodayWithdrawQuery(AccountId accountId)
        {
            AccountId = accountId;
        }

        public AccountId AccountId { get; set; }
        public class GetAllAccountsQueryHandler : IQueryHandler<GetTodayWithdrawQuery, float>
        {
            private readonly IAccountReadStore _accountQueryService;

            public GetAllAccountsQueryHandler(IAccountReadStore accountQueryService)
            {
                _accountQueryService = accountQueryService;
            }
            public async Task<float> ExecuteQueryAsync(GetTodayWithdrawQuery query, CancellationToken cancellationToken)
            {
                var withdraw = await _accountQueryService.GetTodayWithdraw(query, cancellationToken);
                return withdraw;
            }
        }
    }
}
