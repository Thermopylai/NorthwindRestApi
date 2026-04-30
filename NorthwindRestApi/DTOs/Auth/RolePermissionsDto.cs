namespace NorthwindRestApi.DTOs.Auth
{
    public class RolePermissionsDto
    {
        public string RoleName { get; set; } = "";
        public IEnumerable<string> Permissions { get; set; } = new List<string>();
    }
}
