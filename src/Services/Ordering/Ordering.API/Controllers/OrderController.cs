using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Queries;
using Ordering.Infrastructure.CQRS;
using Ordering.Domain.Aggregates;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ordering.Application.Commands;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IQueryExecuter queryExecuter;
        private readonly ICommandExecuter commandExecuter;

        public OrderController(IQueryExecuter queryExecuter, ICommandExecuter commandExecuter)
        {
            this.queryExecuter = queryExecuter;
            this.commandExecuter = commandExecuter;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)] // No need to define type of the body since we are using ActionResult<T>
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var result = await queryExecuter.Execute(new GetOrdersQuery());
            return Ok(result);
        }
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateOrder([FromBody] InsertOrderCommand command)
        {
            var result = await commandExecuter.Execute<bool>(command);
            return Ok(result);
        }
    }
}
