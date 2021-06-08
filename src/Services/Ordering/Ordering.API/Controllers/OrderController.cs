using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.Domain.Aggregates;
using Ordering.Infrastructure.CQRS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [HttpGet("{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // No need to define type of the body since we are using ActionResult<T>
        public async Task<ActionResult<Order>> GetOrder([FromBody] int orderId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateOrder([FromBody] InsertOrderCommand command)
        {
            var result = await commandExecuter.Execute<bool>(command);
            return Ok(result);
        }
        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateOrder([FromBody] dynamic command)
        {
            throw new NotImplementedException();
        }

        [HttpGet("Cancel/{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CancelOrder([FromBody] int orderId)
        {
            throw new NotImplementedException();
        }

    }
}
