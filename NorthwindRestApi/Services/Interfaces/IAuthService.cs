using NorthwindRestApi.DTOs.Auth;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct);
        Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct);
        Task<AuthResponseDto> AssignRoleAsync(AssignRoleDto dto, CancellationToken ct);
    }
}
