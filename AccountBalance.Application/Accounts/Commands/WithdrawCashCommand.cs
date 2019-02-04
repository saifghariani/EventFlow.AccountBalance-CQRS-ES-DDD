using AccountBalance.Domain.Aggregates.AccountAggregate;
using EventFlow;
using EventFlow.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Application.Accounts.Commands
{
    public class WithdrawCashCommand : Command<AccountAggregate, AccountId>
    {
        public WithdrawCashCommand(AccountId accountId, float newBalance) : base(accountId)
        {
            NewBalance = newBalance;
        }

        public float NewBalance { get; set; }

        public class WithdrawCashCommandHandler : CommandHandler<AccountAggregate, AccountId, WithdrawCashCommand>
        {
            private readonly ICommandBus _commandBus;

            public WithdrawCashCommandHandler(ICommandBus commandBus) : base()
            {
                _commandBus = commandBus;
            }

            public override Task ExecuteAsync(AccountAggregate aggregate, WithdrawCashCommand command,
                CancellationToken cancellationToken)
            {
                aggregate.WithdrawCash(command.AggregateId.Value, command.NewBalance);
                return Task.FromResult(0);
            }
        }
    }
}
