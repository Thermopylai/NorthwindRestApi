namespace NorthwindRestApi.DTOs.Auth
{
    public class UserReadDto
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public IList<string> Roles { get; set; } = new List<string>();
        public IList<string> Permissions { get; set; } = new List<string>();
        public IList<RolePermissionsDto> RolePermissions { get; set; } = new List<RolePermissionsDto>();
        public IEnumerable<ClaimDto> Claims { get; set; } = new List<ClaimDto>();
    }
}
