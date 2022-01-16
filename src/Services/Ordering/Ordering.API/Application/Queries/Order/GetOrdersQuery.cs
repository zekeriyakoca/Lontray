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
    {
        public GetOrdersQuery()
        { }
        public GetOrdersQuery(int customerId)
        {
            customerId = customerId;
        }
        public int? customerId { get; set; }
    }

    public class GetOrderQueryHandler : QueryHandler<GetOrdersQuery, IEnumerable<Order>>
    {
        public GetOrderQueryHandler(IOptions<OrderingSettings> orderingOptions, ILogger<GetOrderQueryHandler> logger) : base(orderingOptions.Value, logger)
        { }

        public override async Task<IEnumerable<Order>> Action(GetOrdersQuery query)
        {
            var sqlQuery = "SELECT * FROM ordering.Orders";
            if (query.customerId.HasValue && query.customerId != default)
            {
                sqlQuery += string.Format(" WHERE Id = {0}", query.customerId);
            }
            return await connection.QueryAsync<Order>(sqlQuery);
        }
    }
}
