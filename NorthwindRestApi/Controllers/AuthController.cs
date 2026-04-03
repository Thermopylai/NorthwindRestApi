using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindRestApi.DTOs.Auth;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register(
            RegisterDto dto,
            CancellationToken ct)
        {
            var result = await _service.RegisterAsync(dto, ct);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> AssignRole(
            AssignRoleDto dto,
            CancellationToken ct)
        {
            var result = await _service.AssignRoleAsync(dto, ct);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(
            LoginDto dto,
            CancellationToken ct)
        {
            var result = await _service.LoginAsync(dto, ct);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                UserName = User.Identity?.Name,
                Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            });
        }

        [Authorize]
        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return Ok(claims);
        }
    }
}
