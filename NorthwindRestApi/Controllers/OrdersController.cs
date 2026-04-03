using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadOrders)]
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<OrderListDto>>> GetAll(CancellationToken ct)
        {
            var orders = await _service.GetAllAsync(ct);
            return Ok(orders);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadOrders)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(OrderReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderReadDto>> GetById(int id, CancellationToken ct)
        {
            var order = await _service.GetByIdAsync(id, ct);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadOrders)]
        [HttpGet("by-customer/{customerId}")]
        [ProducesResponseType(typeof(List<OrderListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<OrderListDto>>> GetByCustomerId(string customerId, CancellationToken ct)
        {
            var orders = await _service.GetByCustomerIdAsync(customerId, ct);

            if (!orders.Any()) 
                return NotFound();

            return Ok(orders);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadOrders)]
        [HttpGet("by-date-range")]
        [ProducesResponseType(typeof(List<OrderListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<OrderListDto>>> GetByDateRange(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            CancellationToken ct)
        {
            if (end < start)
                return BadRequest("End date cannot be earlier than start date.");

            var orders = await _service.GetByDateRangeAsync(start, end, ct);

            if (!orders.Any()) 
                return NotFound();

            return Ok(orders);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadOrders)]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<OrderListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<OrderListDto>>> GetPaged(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadOrders)]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<OrderListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<OrderListDto>>> Search([FromQuery] OrderQueryParameters parameters, CancellationToken ct)
        {
            if (parameters.Start.HasValue && parameters.End.HasValue &&
                parameters.End.Value.Date < parameters.Start.Value.Date)
            {
                return BadRequest("End date cannot be earlier than start date.");
            }

            var result = await _service.SearchAsync(parameters, ct);

            if (!result.Items.Any())
                return NotFound();

            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageOrders)]
        [HttpPost]
        [ProducesResponseType(typeof(OrderReadDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<OrderReadDto>> Create(OrderCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.OrderID }, created);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageOrders)]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(PagedResult<OrderReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, OrderUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageOrders)]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _service.DeleteAsync(id, ct);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
