using NorthwindRestApi.DTOs.Auth;
using System.Security.Claims;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct);
        Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct);
        Task<AuthResponseDto> AssignRoleAsync(AssignRoleDto dto, CancellationToken ct);
        Task<AuthResponseDto> RemoveRoleAsync(AssignRoleDto dto, CancellationToken ct);
        Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto dto, CancellationToken ct);
        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct);
        Task<AuthResponseDto> UpdateUserAsync(string userId, UpdateUserDto dto, CancellationToken ct);
        Task<AuthResponseDto> DeleteUserAsync(string userId, CancellationToken ct);
        Task<AuthResponseDto> GetAllUsersAsync(CancellationToken ct);
        Task<AuthResponseDto> GetUserInfoAsync(ClaimsPrincipal user, CancellationToken ct);
        Task<AuthResponseDto> SearchAsync(UserQueryParameters parameters, CancellationToken ct);
        Task<AuthResponseDto> ListRolePermissionsAsync(CancellationToken ct);
    }
}
