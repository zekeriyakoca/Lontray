using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Application.Queries;
using Ordering.Domain.Aggregates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IQueryExecuter queryExecuter;

        public OrderController(IQueryExecuter queryExecuter)
        {
            this.queryExecuter = queryExecuter;
        }

        [HttpGet("orders")]
        [ProducesResponseType(StatusCodes.Status200OK)] // No need to define type of the body since we are using ActionResult<T>
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var result = await queryExecuter.Execute(new GetOrdersQuery());
            return Ok(result);
        }
    }
}
