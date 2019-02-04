using AccountBalance.Domain.Aggregates.AccountAggregate;
using EventFlow;
using EventFlow.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Application.Accounts.Commands
{
    public class DepositCheckCommand : Command<AccountAggregate, AccountId>
    {
        public DepositCheckCommand(AccountId accountId, float amount) : base(accountId)
        {
            Amount = amount;
        }

        public float Amount { get; set; }

        public class DepositCheckCommandHandler : CommandHandler<AccountAggregate, AccountId, DepositCheckCommand>
        {
            private readonly ICommandBus _commandBus;

            public DepositCheckCommandHandler(ICommandBus commandBus) : base()
            {
                _commandBus = commandBus;
            }

            public override Task ExecuteAsync(AccountAggregate aggregate, DepositCheckCommand command,
                CancellationToken cancellationToken)
            {
                aggregate.DepositCheck(command.AggregateId.Value, command.Amount);
                return Task.FromResult(0);
            }
        }
    }
}
