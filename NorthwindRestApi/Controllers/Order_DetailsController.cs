using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Order_Details;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Order_DetailsController : ControllerBase
    {
        private readonly IOrder_DetailService _service;

        public Order_DetailsController(IOrder_DetailService service)
        {
            _service = service;
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadOrderDetails)]
        [HttpGet]
        [ProducesResponseType(typeof(List<Order_DetailReadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Order_DetailReadDto>>> GetAll(CancellationToken ct)
        {
            var order_details = await _service.GetAllAsync(ct);
            return Ok(order_details);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadOrderDetails)]
        [HttpGet("{orderId:int}/{productId:int}")]
        [ProducesResponseType(typeof(Order_DetailReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order_DetailReadDto>> GetById(int orderId, int productId, CancellationToken ct)
        {
            var order_detail = await _service.GetByIdAsync(orderId, productId, ct);

            if (order_detail == null)
                return NotFound();

            return Ok(order_detail);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadOrderDetails)]
        [HttpGet("by-order/{orderId:int}")]
        [ProducesResponseType(typeof(List<Order_DetailReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Order_DetailReadDto>>> GetByOrderId(int orderId, CancellationToken ct)
        {
            var order_details = await _service.GetByOrderIdAsync(orderId, ct);

            if (!order_details.Any()) 
                return NotFound();

            return Ok(order_details);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadOrderDetails)]
        [HttpGet("by-product/{productId:int}")]
        [ProducesResponseType(typeof(List<Order_DetailReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Order_DetailReadDto>>> GetByProductId(int productId, CancellationToken ct)
        {
            var order_details = await _service.GetByProductIdAsync(productId, ct);

            if (!order_details.Any()) 
                return NotFound();

            return Ok(order_details);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadOrderDetails)]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<Order_DetailReadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<Order_DetailReadDto>>> GetPaged(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadOrderDetails)]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<Order_DetailReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<Order_DetailReadDto>>> Search([FromQuery] Order_DetailQueryParameters parameters, CancellationToken ct)
        {
            if (parameters.MinTotalPrice.HasValue && parameters.MaxTotalPrice.HasValue &&
                parameters.MaxTotalPrice.Value < parameters.MinTotalPrice.Value)
            {
                return BadRequest("Max. total price can't be lower than min. total price.");
            }

            var result = await _service.SearchAsync(parameters, ct);

            if (!result.Items.Any())
                return NotFound();

            return Ok(result);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageOrderDetails)]
        [HttpPost]
        [ProducesResponseType(typeof(Order_DetailReadDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<Order_DetailReadDto>> Create(Order_DetailCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { orderId = created.OrderID, productId = created.ProductID }, created);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageOrderDetails)]
        [HttpPut("{orderId:int}/{productId:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Order_DetailReadDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<Order_DetailReadDto?>> Update(int orderId, int productId, Order_DetailUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(orderId, productId, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageOrderDetails)]
        [HttpDelete("{orderId:int}/{productId:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int orderId, int productId, CancellationToken ct)
        {
            var success = await _service.DeleteAsync(orderId, productId, ct);

            if (!success)
                return NotFound();

            return NoContent();
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageOrderDetails)]
        [HttpPost("{orderId:int}/{productId:int}/restore")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Restore(int orderId, int productId, CancellationToken ct)
        {
            var success = await _service.RestoreAsync(orderId, productId, ct);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
