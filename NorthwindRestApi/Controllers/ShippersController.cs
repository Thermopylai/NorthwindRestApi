using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Categories;
using NorthwindRestApi.DTOs.Shippers;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippersController : ControllerBase
    {
        private readonly IShipperService _service;

        public ShippersController(IShipperService service)
        {
            _service = service;
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadShippers)]
        [HttpGet]
        [ProducesResponseType(typeof(List<ShipperListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ShipperListDto>>> GetAll(CancellationToken ct)
        {
            var shippers = await _service.GetAllAsync(ct);
            return Ok(shippers);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadShippers)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ShipperReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShipperReadDto>> GetById(int id, CancellationToken ct)
        {
            var shipper = await _service.GetByIdAsync(id, ct);

            if (shipper == null)
                return NotFound();

            return Ok(shipper);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadShippers)]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<ShipperListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ShipperListDto>>> GetPaged(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadShippers)]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<ShipperListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<ShipperListDto>>> Search(
            [FromQuery] ShipperQueryParameters parameters,
            CancellationToken ct)
        {
            var result = await _service.SearchAsync(parameters, ct);

            if (!result.Items.Any())
                return NotFound();

            return Ok(result);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageShippers)]
        [HttpPost]
        [ProducesResponseType(typeof(ShipperReadDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<ShipperReadDto>> Create(ShipperCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.ShipperID }, created);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageShippers)]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShipperReadDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ShipperReadDto?>> Update(int id, ShipperUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageShippers)]
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

        //[Authorize(Policy = AuthorizationPolicies.CanManageShippers)]
        [HttpPost("{id:int}/restore")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Restore(int id, CancellationToken ct)
        {
            var success = await _service.RestoreAsync(id, ct);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
