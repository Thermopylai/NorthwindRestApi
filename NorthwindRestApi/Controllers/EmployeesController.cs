using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Employees;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service)
        {
            _service = service;
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadEmployees)]
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EmployeeListDto>>> GetAll(CancellationToken ct)
        {
            var employees = await _service.GetAllAsync(ct);
            return Ok(employees);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadEmployees)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EmployeeReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeReadDto>> GetById(int id, CancellationToken ct)
        {
            var employee = await _service.GetByIdAsync(id, ct);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadEmployees)]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<EmployeeListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<EmployeeListDto>>> GetPaged(
            CancellationToken ct,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        [Authorize(Policy = AuthorizationPolicies.CanReadEmployees)]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<EmployeeListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<EmployeeListDto>>> Search([FromQuery] EmployeeQueryParameters parameters, CancellationToken ct)
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

        [Authorize(Policy = AuthorizationPolicies.CanManageEmployees)]
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeReadDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<EmployeeReadDto>> Create(EmployeeCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.EmployeeID }, created);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageEmployees)]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(EmployeeReadDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<EmployeeReadDto?>> Update(int id, EmployeeUpdateDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [Authorize(Policy = AuthorizationPolicies.CanManageEmployees)]
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
