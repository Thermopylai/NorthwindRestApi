using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Auth
{
    public class AssignRoleDto
    {
        [Required]
        public string UserName { get; set; } = "";

        [Required]
        public string RoleName { get; set; } = "";
    }
}
