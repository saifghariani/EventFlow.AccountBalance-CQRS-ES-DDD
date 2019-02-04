using AccountBalance.Domain.Aggregates.AccountAggregate;
using EventFlow;
using EventFlow.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Application.Accounts.Commands
{
    public class SetOverDraftLimitCommand : Command<AccountAggregate, AccountId>
    {
        public SetOverDraftLimitCommand(AccountId accountId, float overDraftLimit) : base(accountId)
        {
            OverDraftLimit = overDraftLimit;
        }

        public float OverDraftLimit { get; set; }

        public class
            SetOverDraftLimitCommandHandler : CommandHandler<AccountAggregate, AccountId, SetOverDraftLimitCommand>
        {
            private readonly ICommandBus _commandBus;

            public SetOverDraftLimitCommandHandler(ICommandBus commandBus) : base()
            {
                _commandBus = commandBus;
            }

            public override Task ExecuteAsync(AccountAggregate aggregate, SetOverDraftLimitCommand command,
                CancellationToken cancellationToken)
            {
                aggregate.SetOverDraftLimit(command.AggregateId.Value, command.OverDraftLimit);
                return Task.FromResult(0);
            }
        }
    }
}
