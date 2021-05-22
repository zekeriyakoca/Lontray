using Ordering.Domain.Aggregates;
using Ordering.Domain.Events;
using Ordering.Infrastructure;
using Ordering.Infrastructure.CQRS;
using System.Threading.Tasks;

namespace Ordering.Application.Commands
{
    public class InsertOrderCommand : ICommand<bool>
    {
        public int TestProp { get; set; }
    }

    public class InsertOrderCommandHandler : CommandHandler<InsertOrderCommand, bool>
    {
        public InsertOrderCommandHandler(OrderingContext context) : base(context)
        { }

        public override async Task<bool> Action(InsertOrderCommand query)
        {
            var orderToCreate = Order.NewDraft();
            orderToCreate.AddDomainEvent(new OrderCreatedDomainEvent());

            context.Orders.Add(orderToCreate);

            await context.SaveChangesAsync();
            return true;
        }
    }
}
