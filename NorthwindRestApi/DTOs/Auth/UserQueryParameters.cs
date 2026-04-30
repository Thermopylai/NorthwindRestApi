namespace NorthwindRestApi.DTOs.Auth
{
    public class UserQueryParameters
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Permission { get; set; }
    }
}
