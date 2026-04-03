namespace NorthwindRestApi.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
