using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Categories;
using NorthwindRestApi.DTOs.Regions;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _service;

        public RegionsController(IRegionService service)
        {
            _service = service;
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadRegions)]
        [HttpGet]
        [ProducesResponseType(typeof(List<RegionListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RegionListDto>>> GetAll(CancellationToken ct)
        {
            var regions = await _service.GetAllAsync(ct);
            return Ok(regions);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadRegions)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RegionReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RegionReadDto>> GetById(int id, CancellationToken ct)
        {
            var region = await _service.GetByIdAsync(id, ct);

            if (region == null)
                return NotFound();

            return Ok(region);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadRegions)]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<RegionReadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<RegionReadDto>>> GetPaged(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadRegions)]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<RegionReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<RegionReadDto>>> Search(
            [FromQuery] RegionQueryParameters parameters,
            CancellationToken ct)
        {
            var result = await _service.SearchAsync(parameters, ct);

            if (!result.Items.Any())
                return NotFound();

            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageRegions)]
        [HttpPost]
        [ProducesResponseType(typeof(RegionReadDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<RegionReadDto>> Create(RegionCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.RegionID }, created);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageRegions)]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RegionReadDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<RegionReadDto?>> Update(int id, RegionUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageRegions)]
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

        [Authorize(Policy = AuthorizationPolicies.CanManageRegions)]
        [HttpPut("{id:int}/restore")]
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
