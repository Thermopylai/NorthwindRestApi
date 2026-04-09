using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Customers;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.Models;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service)
        {
            _service = service;
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadCustomers)]
        [HttpGet]
        [ProducesResponseType(typeof(List<CustomerListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CustomerListDto>>> GetAll(CancellationToken ct)
        {
            var customers = await _service.GetAllAsync(ct);
            return Ok(customers);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadCustomers)]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerReadDto>> GetById(string id, CancellationToken ct)
        {
            var customer = await _service.GetByIdAsync(id, ct);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadCustomers)]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<CustomerListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<CustomerListDto>>> GetPaged(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadCustomers)]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<CustomerListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<CustomerListDto>>> Search([FromQuery] CustomerQueryParameters parameters, CancellationToken ct)
        {
            var result = await _service.SearchAsync(parameters, ct);

            if (!result.Items.Any())
                return NotFound();

            return Ok(result);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageCustomers)]
        [HttpPost]
        [ProducesResponseType(typeof(CustomerReadDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<CustomerReadDto>> Create(CustomerCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.CustomerID }, created);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageCustomers)]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomerReadDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerReadDto?>> Update(string id, CustomerUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageCustomers)]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var success = await _service.DeleteAsync(id, ct);

            if (!success)
                return NotFound();

            return NoContent();
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageCustomers)]
        [HttpPut("{id}/restore")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Restore(string id, CancellationToken ct)
        {
            var success = await _service.RestoreAsync(id, ct);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
