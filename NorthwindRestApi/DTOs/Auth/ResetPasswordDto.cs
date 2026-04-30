using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required]
        public string UserName { get; set; } = "";
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = "";
    }
}
