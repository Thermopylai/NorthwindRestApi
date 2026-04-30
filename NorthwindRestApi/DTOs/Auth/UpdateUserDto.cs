using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Auth
{
    public class UpdateUserDto
    {
        [StringLength(50)]
        public string? UserName { get; set; }
                
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Password { get; set; } = "";
    }
}
