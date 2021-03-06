using Cafe.Waiter.Contracts.Commands;
using CQSplit;
using CQSplit.Messaging;

namespace Cafe.Waiter.Command.Service.Consumers
{
    public class PlaceOrderConsumer : CommandConsumer<IPlaceOrderCommand>
    {
        public PlaceOrderConsumer(ICommandRouter commandRouter)
            : base(commandRouter)
        {
        }
    }
}