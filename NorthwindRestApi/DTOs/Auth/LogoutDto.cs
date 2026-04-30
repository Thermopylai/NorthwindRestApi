using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Auth
{
    public class LogoutDto
    {
        [Required]
        public string RefreshToken { get; set; } = "";
    }
}
