using AccountBalance.Domain.Aggregates.AccountAggregate;
using EventFlow;
using EventFlow.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace AccountBalance.Application.Accounts.Commands
{
    public class SetDailyWireTransferLimitCommand : Command<AccountAggregate, AccountId>
    {
        public SetDailyWireTransferLimitCommand(AccountId accountId, float dailyWireTransferLimit) : base(accountId)
        {
            DailyWireTransferLimit = dailyWireTransferLimit;
        }

        public float DailyWireTransferLimit { get; set; }

        public class
            SetDailyWireTransferLimitCommandHandler : CommandHandler<AccountAggregate, AccountId,
                SetDailyWireTransferLimitCommand>
        {
            private readonly ICommandBus _commandBus;

            public SetDailyWireTransferLimitCommandHandler(ICommandBus commandBus) : base()
            {
                _commandBus = commandBus;
            }

            public override Task ExecuteAsync(AccountAggregate aggregate, SetDailyWireTransferLimitCommand command,
                CancellationToken cancellationToken)
            {
                aggregate.SetDailyWireTransferLimit(command.AggregateId.Value, command.DailyWireTransferLimit);
                return Task.FromResult(0);
            }
        }
    }
}
