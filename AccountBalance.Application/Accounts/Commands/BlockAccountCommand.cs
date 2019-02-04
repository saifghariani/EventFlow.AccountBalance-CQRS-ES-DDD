using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountBalance.Domain.Aggregates.AccountAggregate;
using EventFlow;
using EventFlow.Commands;
using EventFlow.Core;

namespace AccountBalance.Application.Accounts.Commands
{
    public class BlockAccountCommand : Command<AccountAggregate, AccountId>
    {
        public BlockAccountCommand(AccountId aggregateId) : base(aggregateId)
        {
        }

        public class BlockAccountCommandHandler : CommandHandler<AccountAggregate, AccountId, BlockAccountCommand>
        {
            private readonly ICommandBus _commandBus;

            public BlockAccountCommandHandler(ICommandBus commandBus) : base()
            {
                _commandBus = commandBus;
            }
            public override Task ExecuteAsync(AccountAggregate aggregate, BlockAccountCommand command, CancellationToken cancellationToken)
            {
                aggregate.BlockAccount(command.AggregateId.Value);
                return Task.FromResult(0);
            }
        }
    }
}
