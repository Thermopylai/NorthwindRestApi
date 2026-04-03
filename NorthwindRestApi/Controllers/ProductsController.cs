using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadProducts)]
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductListDto>>> GetAll(CancellationToken ct)
        {
            var products = await _service.GetAllAsync(ct);
            return Ok(products);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadProducts)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductReadDto>> GetById(int id, CancellationToken ct)
        {
            var product = await _service.GetByIdAsync(id, ct);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadProducts)]
        [HttpGet("by-category/{categoryId:int}")]
        [ProducesResponseType(typeof(List<ProductListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ProductListDto>>> GetByCategoryId(int categoryId, CancellationToken ct)
        {
            var products = await _service.GetByCategoryIdAsync(categoryId, ct);

            if (!products.Any())
                return NotFound();

            return Ok(products);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadProducts)]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<ProductListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ProductListDto>>> GetPaged(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadProducts)]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<ProductListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<ProductListDto>>> Search([FromQuery] ProductQueryParameters parameters, CancellationToken ct)
        {
            if (parameters.MinPrice.HasValue && parameters.MaxPrice.HasValue &&
                parameters.MaxPrice.Value < parameters.MinPrice.Value)
            {
                return BadRequest("Max. price can't be lower than min. price.");
            }

            var result = await _service.SearchAsync(parameters, ct);

            if (!result.Items.Any())
                return NotFound();

            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageProducts)]
        [HttpPost]
        [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<ProductReadDto>> Create(ProductCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.ProductID }, created);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageProducts)]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(PagedResult<ProductReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageProducts)]
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
