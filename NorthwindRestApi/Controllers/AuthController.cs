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
        [HttpPost("logout")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<AuthResponseDto> Logout()
        {
            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Logout successful. Remove the token on the client."
            });
        }

        [HttpPut("assign-role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

        [HttpPut("remove-role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> RemoveRole(
            AssignRoleDto dto,
            CancellationToken ct)
        {
            var result = await _service.RemoveRoleAsync(dto, ct);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{userId}/change-password")]
        [Authorize]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> ChangePassword(
            string userId,
            ChangePasswordDto dto,
            CancellationToken ct)
        {
            var result = await _service.ChangePasswordAsync(userId, dto, ct);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("reset-password")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> ResetPassword(
            ResetPasswordDto dto,
            CancellationToken ct)
        {
            var result = await _service.ResetPasswordAsync(dto, ct);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{userId}/update-user-info")]
        [Authorize]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> UpdateUserInfo(
            string userId,
            UpdateUserDto dto,
            CancellationToken ct)
        {
            var result = await _service.UpdateUserAsync(userId, dto, ct);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}/delete-user")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> DeleteUser(
            string userId,
            CancellationToken ct)
        {
            var result = await _service.DeleteUserAsync(userId, ct);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-users")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthResponseDto>> GetAllUsers(CancellationToken ct)
        {
            var users = await _service.GetAllUsersAsync(ct);
            if (users == null)
                return NotFound();
            return Ok(users);
        }

        [HttpGet("get-user-info")]
        [Authorize]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthResponseDto>> GetUserInfo(CancellationToken ct)
        {
            var user = User;
            var userInfo = await _service.GetUserInfoAsync(user, ct);
            if (userInfo == null)
                return NotFound();
            return Ok(userInfo);
        }

        [HttpGet("search-users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthResponseDto>> SearchUsers(
            [FromQuery] UserQueryParameters parameters,
            CancellationToken ct)
        {
            var result = await _service.SearchAsync(parameters, ct);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("list-role-permissions")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthResponseDto>> ListRolePermissions(CancellationToken ct)
        {
            var result = await _service.ListRolePermissionsAsync(ct);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
