using AccountBalance.Domain.Aggregates.AccountAggregate;
using EventFlow;
using EventFlow.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Application.Accounts.Commands
{
    public class DepositCashCommand : Command<AccountAggregate, AccountId>
    {
        public DepositCashCommand(AccountId accountId, float amount) : base(accountId)
        {
            Amount = amount;
        }

        public float Amount { get; set; }

        public class DepositCashCommandHandler : CommandHandler<AccountAggregate, AccountId, DepositCashCommand>
        {
            private readonly ICommandBus _commandBus;

            public DepositCashCommandHandler(ICommandBus commandBus) : base()
            {
                _commandBus = commandBus;
            }

            public override Task ExecuteAsync(AccountAggregate aggregate, DepositCashCommand command,
                CancellationToken cancellationToken)
            {
                aggregate.DepositCash(command.AggregateId.Value, command.Amount);
                return Task.FromResult(0);
            }
        }
    }
}
