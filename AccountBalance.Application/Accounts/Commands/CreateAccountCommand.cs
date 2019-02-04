using AccountBalance.Domain.Aggregates.AccountAggregate;
using EventFlow;
using EventFlow.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Application.Accounts.Commands
{
    public class CreateAccountCommand : Command<AccountAggregate, AccountId>
    {
        public CreateAccountCommand(AccountId accountId, string holderName) : base(accountId)
        {
            HolderName = holderName;
        }

        public string HolderName { get; set; }

        public class CreateAccountCommandHandler : CommandHandler<AccountAggregate, AccountId, CreateAccountCommand>
        {
            private readonly ICommandBus _commandBus;

            public CreateAccountCommandHandler(ICommandBus commandBus) : base()
            {
                _commandBus = commandBus;
            }

            public override Task ExecuteAsync(AccountAggregate aggregate, CreateAccountCommand command,
                CancellationToken cancellationToken)
            {
                aggregate.CreateAccount(command.AggregateId.Value, command.HolderName);
                return Task.FromResult(0);
            }
        }
    }
}

