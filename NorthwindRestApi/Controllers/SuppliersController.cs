using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Categories;
using NorthwindRestApi.DTOs.Suppliers;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SuppliersController(ISupplierService service)
        {
            _service = service;
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadSuppliers)]
        [HttpGet]
        [ProducesResponseType(typeof(List<SupplierListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<SupplierListDto>>> GetAll(CancellationToken ct)
        {
            var suppliers = await _service.GetAllAsync(ct);
            return Ok(suppliers);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadSuppliers)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(SupplierReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierReadDto>> GetById(int id, CancellationToken ct)
        {
            var supplier = await _service.GetByIdAsync(id, ct);

            if (supplier == null)
                return NotFound();

            return Ok(supplier);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadSuppliers)]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<SupplierReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedResult<SupplierReadDto>>> GetPaged(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadSuppliers)]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<SupplierReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<SupplierReadDto>>> Search(
            [FromQuery] SupplierQueryParameters parameters,
            CancellationToken ct)
        {
            var result = await _service.SearchAsync(parameters, ct);

            if (result.Items.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageSuppliers)]
        [HttpPost]
        [ProducesResponseType(typeof(SupplierReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SupplierReadDto>> Create(SupplierCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.SupplierID }, created);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageSuppliers)]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(SupplierReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SupplierReadDto?>> Update(int id, SupplierUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageSuppliers)]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _service.DeleteAsync(id, ct);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageSuppliers)]
        [HttpPut("{id:int}/restore")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Restore(int id, CancellationToken ct)
        {
            var success = await _service.RestoreAsync(id, ct);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
