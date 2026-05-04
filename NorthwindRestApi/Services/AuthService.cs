using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Auth;
using NorthwindRestApi.Models.Identity;
using NorthwindRestApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace NorthwindRestApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthDbContext _authDbContext;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AuthDbContext authDbContext,
            IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authDbContext = authDbContext;
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

            var (accessToken, accessTokenExpiresAt) = await GenerateJwtTokenAsync(user);

            var rawRefreshToken = GenerateRefreshToken();
            var refreshToken = CreateRefreshToken(user.Id, rawRefreshToken);

            _authDbContext.RefreshTokens.Add(refreshToken);
            await _authDbContext.SaveChangesAsync(ct);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully.",

                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,

                RefreshToken = rawRefreshToken,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,

                UserId = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? ""
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
                    Message = "Invalid username."
                };
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!passwordValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid password."
                };
            }

            var (accessToken, accessTokenExpiresAt) = await GenerateJwtTokenAsync(user);

            var rawRefreshToken = GenerateRefreshToken();
            var refreshToken = CreateRefreshToken(user.Id, rawRefreshToken);

            _authDbContext.RefreshTokens.Add(refreshToken);
            await _authDbContext.SaveChangesAsync(ct);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",

                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,

                RefreshToken = rawRefreshToken,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,

                UserId = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? ""
            };
        }

        public async Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto dto, CancellationToken ct)
        {
            var tokenHash = HashToken(dto.RefreshToken);

            var storedToken = await _authDbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

            if (storedToken == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }

            if (!storedToken.IsActive)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Refresh token is expired or revoked."
                };
            }

            var user = storedToken.User;

            var (newAccessToken, newAccessTokenExpiresAt) = await GenerateJwtTokenAsync(user);

            var newRawRefreshToken = GenerateRefreshToken();
            var newRefreshToken = CreateRefreshToken(user.Id, newRawRefreshToken);

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.ReplacedByTokenHash = newRefreshToken.TokenHash;

            _authDbContext.RefreshTokens.Add(newRefreshToken);
            await _authDbContext.SaveChangesAsync(ct);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Token refreshed successfully.",

                AccessToken = newAccessToken,
                AccessTokenExpiresAt = newAccessTokenExpiresAt,

                RefreshToken = newRawRefreshToken,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,

                UserId = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? ""
            };
        }

        public async Task<AuthResponseDto> LogoutAsync(LogoutDto dto, CancellationToken ct)
        {
            var tokenHash = HashToken(dto.RefreshToken);

            var storedToken = await _authDbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

            if (storedToken == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }

            if (storedToken.RevokedAt != null)
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Refresh token was already revoked."
                };
            }

            storedToken.RevokedAt = DateTime.UtcNow;

            await _authDbContext.SaveChangesAsync(ct);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Logout successful. Refresh token revoked."
            };
        }

        private async Task<(string token, DateTime expiresAt)> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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

        private static string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        private static string HashToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        private RefreshToken CreateRefreshToken(string userId, string rawRefreshToken)
        {
            return new RefreshToken
            {
                UserId = userId,
                TokenHash = HashToken(rawRefreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<AuthResponseDto> AssignRoleAsync(UserRoleDto dto, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
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
                Message = $"Role '{dto.RoleName}' assigned to '{user.UserName}'."
            };
        }

        public async Task<AuthResponseDto> RemoveRoleAsync(UserRoleDto dto, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

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

            if (!await _userManager.IsInRoleAsync(user, dto.RoleName))
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "User does not have this role."
                };
            }

            if (user.UserName == "admin" && dto.RoleName == "Admin")
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Cannot remove 'Admin' role from the default admin user."
                };
            }

            var result = await _userManager.RemoveFromRoleAsync(user, dto.RoleName);

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
                Message = $"Role '{dto.RoleName}' removed from '{user.UserName}'."
            };
        }

        public async Task<AuthResponseDto> ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordDto dto, CancellationToken ct)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var appUser = await _userManager.FindByIdAsync(userId);

            if (appUser == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var passwordValid = await _userManager.CheckPasswordAsync(appUser, dto.CurrentPassword);

            if (!passwordValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Current password is incorrect."
                };
            }

            var result = await _userManager.ChangePasswordAsync(appUser, dto.CurrentPassword, dto.NewPassword);

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
                Message = "Password changed successfully."
            };
        }

        public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

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
                Message = "Password reset successfully."
            };
        }

        public async Task<AuthResponseDto> UpdateUserAsync(ClaimsPrincipal user, UpdateUserDto dto, CancellationToken ct)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var passwordValid = await _userManager.CheckPasswordAsync(appUser, dto.Password);

            if (!passwordValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid password."
                };
            }

            if (!string.IsNullOrWhiteSpace(dto.UserName))
                appUser.UserName = dto.UserName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                appUser.Email = dto.Email;

            var result = await _userManager.UpdateAsync(appUser);

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
                Message = "User updated successfully."
            };
        }

        public async Task<AuthResponseDto> DeleteUserAsync(string userId, ClaimsPrincipal user, CancellationToken ct)
        {
            var delUser = await _userManager.FindByIdAsync(userId);
            var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == userId)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Users cannot delete themselves."
                };
            }

            if (delUser == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            if (delUser.UserName == "admin")
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Cannot delete the default admin user."
                };
            }

            var result = await _userManager.DeleteAsync(delUser);

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
                Message = "User deleted successfully."
            };
        }

        public async Task<AuthResponseDto> GetAllUsersAsync(CancellationToken ct)
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync(ct);
            var userIds = users.Select(u => u.Id).ToList();

            // Load all user->role rows in one SQL query
            var userRoleRows = await (
                from ur in _authDbContext.UserRoles
                join r in _authDbContext.Roles on ur.RoleId equals r.Id
                where userIds.Contains(ur.UserId)
                select new
                {
                    ur.UserId,
                    RoleName = r.Name
                }
            ).ToListAsync(ct);

            var rolesByUserId = userRoleRows
                .Where(x => !string.IsNullOrWhiteSpace(x.RoleName))
                .GroupBy(x => x.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => (IList<string>)g.Select(x => x.RoleName!).Distinct().ToList()
                );

            var userDtos = users.Select(u =>
            {
                rolesByUserId.TryGetValue(u.Id, out var roles);
                roles ??= new List<string>();

                var permissions = roles
                    .SelectMany(role => RolePermissions.Map.TryGetValue(role, out var perms)
                        ? perms
                        : Array.Empty<string>())
                    .Distinct()
                    .ToList();

                var rolePermissions = roles
                    .Distinct()
                    .Select(role => new RolePermissionsDto
                    {
                        RoleName = role,
                        Permissions = RolePermissions.Map.TryGetValue(role, out var perms)
                            ? perms
                            : Array.Empty<string>()
                    })
                    .ToList();

                var claims = rolesByUserId.TryGetValue(u.Id, out var userRoles)
                    ? userRoles.SelectMany(role => RolePermissions.Map.TryGetValue(role, out var perms)
                        ? perms
                        : Array.Empty<string>())
                        .Select(p => new ClaimDto
                        {
                            Type = "permission",
                            Value = p
                        })
                    : Enumerable.Empty<ClaimDto>();

                return new UserReadDto
                {
                    UserId = u.Id,
                    UserName = u.UserName ?? "",
                    Email = u.Email ?? "",
                    Roles = roles,
                    Permissions = permissions,
                    RolePermissions = rolePermissions,
                    Claims = claims
                };
            }).ToList();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Users retrieved successfully.",
                Users = userDtos
            };
        }

        public async Task<AuthResponseDto> GetUserInfoAsync(ClaimsPrincipal user, CancellationToken ct)
        {
            var currentUser = await _userManager.GetUserAsync(user);

            if (currentUser == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            // SQL: only fetch role names
            var roles = await (
                from ur in _authDbContext.UserRoles
                join r in _authDbContext.Roles on ur.RoleId equals r.Id
                where ur.UserId == currentUser.Id
                select r.Name
            )
            .Where(name => name != null)
            .Select(name => name!)
            .Distinct()
            .ToListAsync(ct);

            // In-memory mapping: role -> permissions
            var permissions = roles
                .SelectMany(role => RolePermissions.Map.TryGetValue(role, out var perms)
                    ? perms
                    : Array.Empty<string>())
                .Distinct()
                .ToList();

            var rolePermissionsDtos = roles
                .Select(role => new RolePermissionsDto
                {
                    RoleName = role,
                    Permissions = RolePermissions.Map.TryGetValue(role, out var perms)
                        ? perms
                        : Array.Empty<string>()
                })
                .ToList();

            var claims = rolePermissionsDtos
                .SelectMany(rp => rp.Permissions.Select(p => new ClaimDto
                {
                    Type = "permission",
                    Value = p
                }))
                .ToList();

            return new AuthResponseDto
            {
                Success = true,
                Message = "User info retrieved successfully.",
                UserId = currentUser.Id,
                UserName = currentUser.UserName ?? "",
                Email = currentUser.Email ?? "",
                Roles = roles,
                Permissions = permissions,
                RolePermissions = rolePermissionsDtos,
                Claims = claims
            };
        }

        public async Task<AuthResponseDto> SearchAsync(UserQueryParameters parameters, CancellationToken ct)
        {
            var query = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(parameters.UserId))
            {
                query = query.Where(u => u.Id.Contains(parameters.UserId));
            }

            if (!string.IsNullOrEmpty(parameters.UserName))
            {
                query = query.Where(u => u.UserName.Contains(parameters.UserName));
            }

            if (!string.IsNullOrEmpty(parameters.Email))
            {
                query = query.Where(u => u.Email.Contains(parameters.Email));
            }

            // Role filter (SQL)
            if (!string.IsNullOrEmpty(parameters.Role))
            {
                var normalizedRole = parameters.Role.ToUpperInvariant();

                var userIdsInRole =
                    from ur in _authDbContext.UserRoles
                    join r in _authDbContext.Roles on ur.RoleId equals r.Id
                    where r.NormalizedName != null && r.NormalizedName.Contains(normalizedRole)
                    select ur.UserId;

                query = query.Where(u => userIdsInRole.Contains(u.Id));
            }

            // Permission filter (SQL)
            if (!string.IsNullOrEmpty(parameters.Permission))
            {
                var permissionTerm = parameters.Permission.Trim();

                var rolesWithPermissionNormalized = RolePermissions.Map
                    .Where(kvp => kvp.Value.Any(p => p.Contains(permissionTerm, StringComparison.OrdinalIgnoreCase)))
                    .Select(kvp => kvp.Key.ToUpperInvariant())
                    .ToList();

                if (rolesWithPermissionNormalized.Count == 0)
                {
                    query = query.Where(_ => false);
                }
                else
                {
                    var userIdsWithPermission =
                        from ur in _authDbContext.UserRoles
                        join r in _authDbContext.Roles on ur.RoleId equals r.Id
                        where r.NormalizedName != null && rolesWithPermissionNormalized.Contains(r.NormalizedName)
                        select ur.UserId;

                    query = query.Where(u => userIdsWithPermission.Contains(u.Id));
                }
            }

            // Materialize matched users
            var users = await query.ToListAsync(ct);
            var userIds = users.Select(u => u.Id).ToList();

            if (users.Count > 0)
            {

                // Batch load roles for all matched users in ONE query
                var userRoleRows = await (
                    from ur in _authDbContext.UserRoles
                    join r in _authDbContext.Roles on ur.RoleId equals r.Id
                    where userIds.Contains(ur.UserId)
                    select new
                    {
                        ur.UserId,
                        RoleName = r.Name
                    }
                ).ToListAsync(ct);

                var rolesByUserId = userRoleRows
                    .Where(x => !string.IsNullOrWhiteSpace(x.RoleName))
                    .GroupBy(x => x.UserId)
                    .ToDictionary(
                        g => g.Key,
                        g => (IList<string>)g.Select(x => x.RoleName!).Distinct().ToList()
                    );

                var userDtos = users.Select(u =>
                {
                    rolesByUserId.TryGetValue(u.Id, out var roles);
                    roles ??= new List<string>();

                    var permissions = roles
                        .SelectMany(role => RolePermissions.Map.TryGetValue(role, out var perms)
                            ? perms
                            : Array.Empty<string>())
                        .Distinct()
                        .ToList();

                    var rolePermissions = roles
                        .Distinct()
                        .Select(role => new RolePermissionsDto
                        {
                            RoleName = role,
                            Permissions = RolePermissions.Map.TryGetValue(role, out var perms)
                                ? perms
                                : Array.Empty<string>()
                        })
                        .ToList();

                    return new UserReadDto
                    {
                        UserId = u.Id,
                        UserName = u.UserName ?? "",
                        Email = u.Email ?? "",
                        Roles = roles,
                        Permissions = permissions,
                        RolePermissions = rolePermissions
                    };
                }).ToList();

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Search completed successfully.",
                    Users = userDtos
                };
            }
            else
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Search completed successfully. No users matched the criteria.",
                    Users = new List<UserReadDto>()
                };
            }
        }

        public async Task<AuthResponseDto> ListRolePermissionsAsync(CancellationToken ct)
        {
            var rolePermissions = RolePermissions.Map.Select(kvp => new RolePermissionsDto
            {
                RoleName = kvp.Key,
                Permissions = kvp.Value
            }).ToList();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Role permissions retrieved successfully.",
                RolePermissions = rolePermissions
            };
        }
    }
}
