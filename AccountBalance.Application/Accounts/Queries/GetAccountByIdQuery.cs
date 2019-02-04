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
    public class GetAccountByIdQuery : IQuery<Account>
    {
        public AccountId AccountId { get; set; }

        public GetAccountByIdQuery(AccountId id)
        {
            AccountId = id;
        }

        public class GetAccountByIdQueryHandler : IQueryHandler<GetAccountByIdQuery, Account>
        {
            private readonly IAccountReadStore _accountQueryService;

            public GetAccountByIdQueryHandler(IAccountReadStore accountQueryService)
            {
                _accountQueryService = accountQueryService;
            }
            public async Task<Account> ExecuteQueryAsync(GetAccountByIdQuery query, CancellationToken cancellationToken)
            {
                var account = await _accountQueryService.GetAccountById(query, cancellationToken);
                return account;
            }
        }
    }
}
