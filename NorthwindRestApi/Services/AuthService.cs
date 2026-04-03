using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Auth;
using NorthwindRestApi.Models.Identity;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username is already in use."
                };
            }

            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingEmail != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email is already in use."
                };
            }

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(" | ", result.Errors.Select(e => e.Description))
                };
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Default role 'User' does not exist."
                };
            }

            // Lisätään oletusrooli heti rekisteröinnin jälkeen
            var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!addToRoleResult.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User was created, but assigning default role failed: " +
                              string.Join(" | ", addToRoleResult.Errors.Select(e => e.Description))
                };
            }

            var (token, expiresAt) = await GenerateJwtTokenAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully.",
                Token = token,
                ExpiresAt = expiresAt
            };
        }

        public async Task<AuthResponseDto> AssignRoleAsync(AssignRoleDto dto, CancellationToken ct)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            if (!await _roleManager.RoleExistsAsync(dto.RoleName))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Role does not exist."
                };
            }

            if (await _userManager.IsInRoleAsync(user, dto.RoleName))
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "User already has this role."
                };
            }

            var result = await _userManager.AddToRoleAsync(user, dto.RoleName);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(" | ", result.Errors.Select(e => e.Description))
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = $"Role '{dto.RoleName}' assigned to '{dto.UserName}'."
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!passwordValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            var (token, expiresAt) = await GenerateJwtTokenAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                ExpiresAt = expiresAt
            };
        }

        private async Task<(string token, DateTime expiresAt)> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            // Lisää roolit
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Lisää permissionit roolien perusteella
            var permissions = roles
                .Where(role => RolePermissions.Map.ContainsKey(role))
                .SelectMany(role => RolePermissions.Map[role])
                .Distinct();

            claims.AddRange(permissions.Select(p => new Claim("permission", p)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expiresAt);
        }
    }
}
