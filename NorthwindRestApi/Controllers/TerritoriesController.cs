using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Categories;
using NorthwindRestApi.DTOs.Territories;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerritoriesController : ControllerBase
    {
        private readonly ITerritoryService _service;

        public TerritoriesController(ITerritoryService service)
        {
            _service = service;
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadTerritories)]
        [HttpGet]
        [ProducesResponseType(typeof(List<TerritoryReadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TerritoryReadDto>>> GetAll(CancellationToken ct)
        {
            var territories = await _service.GetAllAsync(ct);
            return Ok(territories);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadTerritories)]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TerritoryReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TerritoryReadDto>> GetById(string id, CancellationToken ct)
        {
            var territory = await _service.GetByIdAsync(id, ct);

            if (territory == null)
                return NotFound();

            return Ok(territory);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadTerritories)]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<TerritoryReadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<TerritoryReadDto>>> GetPaged(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanReadTerritories)]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<TerritoryReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<TerritoryReadDto>>> Search(
            [FromQuery] TerritoryQueryParameters parameters,
            CancellationToken ct)
        {
            var result = await _service.SearchAsync(parameters, ct);

            if (!result.Items.Any())
                return NotFound();

            return Ok(result);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageTerritories)]
        [HttpPost]
        [ProducesResponseType(typeof(TerritoryReadDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<TerritoryReadDto>> Create(TerritoryCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.TerritoryID }, created);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageTerritories)]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TerritoryReadDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TerritoryReadDto?>> Update(string id, TerritoryUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        //[Authorize(Policy = AuthorizationPolicies.CanManageTerritories)]
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

        //[Authorize(Policy = AuthorizationPolicies.CanManageTerritories)]
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
