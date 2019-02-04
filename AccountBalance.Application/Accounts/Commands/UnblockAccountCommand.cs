using AccountBalance.Domain.Aggregates.AccountAggregate;
using EventFlow;
using EventFlow.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Application.Accounts.Commands
{
    class UnblockAccountCommand : Command<AccountAggregate, AccountId>
    {
        public UnblockAccountCommand(AccountId aggregateId) : base(aggregateId)
        {
        }


        public class UnblockAccountCommandHandler : CommandHandler<AccountAggregate, AccountId, UnblockAccountCommand>
        {
            private readonly ICommandBus _commandBus;

            public UnblockAccountCommandHandler(ICommandBus commandBus) : base()
            {
                _commandBus = commandBus;
            }
            public override Task ExecuteAsync(AccountAggregate aggregate, UnblockAccountCommand command, CancellationToken cancellationToken)
            {
                aggregate.UnblockAccount(command.AggregateId.Value);
                return Task.FromResult(0);
            }
        }
    }
}
