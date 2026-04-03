using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Categories;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadCategories)]
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoryListDto>>> GetAll(CancellationToken ct)
        {
            var categories = await _service.GetAllAsync(ct);
            return Ok(categories);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadCategories)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryReadDto>> GetById(int id, CancellationToken ct)
        {
            var category = await _service.GetByIdAsync(id, ct);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadCategories)]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<CategoryListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<CategoryListDto>>> GetPaged(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadCategories)]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<CategoryListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<CategoryListDto>>> Search(
            [FromQuery] CategoryQueryParameters parameters, 
            CancellationToken ct)
        {
            var result = await _service.SearchAsync(parameters, ct);

            if (!result.Items.Any())
                return NotFound();

            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageCategories)]
        [HttpPost]
        [ProducesResponseType(typeof(CategoryReadDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<CategoryReadDto>> Create(CategoryCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.CategoryID }, created);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageCategories)]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CategoryReadDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CategoryReadDto?>> Update(int id, CategoryUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageCategories)]
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
