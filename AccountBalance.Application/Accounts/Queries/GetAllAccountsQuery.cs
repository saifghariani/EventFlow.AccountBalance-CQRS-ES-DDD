using AccountBalance.Application.Accounts.Queries.Abstract;
using AccountBalance.Domain.Aggregates.AccountAggregate;
using EventFlow.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Application.Accounts.Queries
{
    public class GetAllAccountsQuery : IQuery<List<Account>>
    {
        public GetAllAccountsQuery()
        {
        }

        public class GetAllAccountsQueryHandler : IQueryHandler<GetAllAccountsQuery, List<Account>>
        {
            private readonly IAccountReadStore _accountQueryService;

            public GetAllAccountsQueryHandler(IAccountReadStore accountQueryService)
            {
                _accountQueryService = accountQueryService;
            }
            public async Task<List<Account>> ExecuteQueryAsync(GetAllAccountsQuery query, CancellationToken cancellationToken)
            {
                var account = await _accountQueryService.GetAllAccounts(query, cancellationToken);
                return account;
            }
        }
    }
}
