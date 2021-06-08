using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderingGrpc
{
    public class OrderingService : Ordering.OrderingBase
    {
        private readonly ILogger<OrderingService> logger;

        public OrderingService(ILogger<OrderingService> logger)
        {
            this.logger = logger;
        }
    }
}
