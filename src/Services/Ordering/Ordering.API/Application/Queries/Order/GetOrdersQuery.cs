using Dapper;
using Microsoft.Extensions.Options;
using Ordering.Domain.Aggregates;
using Ordering.Infrastructure;
using Ordering.Infrastructure.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.Queries
{
    public class GetOrdersQuery
    { }

    public class GetOrderQueryHandler : QueryHandler<GetOrdersQuery, IEnumerable<Order>>
    {
        public GetOrderQueryHandler(IOptions<OrderingSettings> orderingOptions) : base(orderingOptions.Value)
        { }

        public override async Task<IEnumerable<Order>> Action(GetOrdersQuery query)
        {
            return await connection.QueryAsync<Order>("Select * from Orders");
        }
    }
}
