namespace NorthwindRestApi.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public IEnumerable<string> Permissions { get; set; } = new List<string>();
        public IEnumerable<UserReadDto> Users { get; set; } = new List<UserReadDto>();
        public IEnumerable<RolePermissionsDto> RolePermissions { get; set; } = new List<RolePermissionsDto>();
    }
}
