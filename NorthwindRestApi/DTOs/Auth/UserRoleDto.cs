using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Auth
{
    public class UserRoleDto
    {
        [Required]
        public string UserId { get; set; } = "";

        [Required]
        public string RoleName { get; set; } = "";
    }
}
