using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Domain.Aggregates;
using Ordering.Infrastructure;
using Ordering.Infrastructure.CQRS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.Application.Queries
{
    public class GetOrdersQuery : IQuery<IEnumerable<Order>>
    { }

    public class GetOrderQueryHandler : QueryHandler<GetOrdersQuery, IEnumerable<Order>>
    {
        public GetOrderQueryHandler(IOptions<OrderingSettings> orderingOptions, ILogger<GetOrderQueryHandler> logger) : base(orderingOptions.Value, logger)
        { }

        public override async Task<IEnumerable<Order>> Action(GetOrdersQuery query)
        {
            return await connection.QueryAsync<Order>("Select * from Orders");
        }
    }
}
