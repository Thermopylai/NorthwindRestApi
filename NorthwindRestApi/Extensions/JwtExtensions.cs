using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NorthwindRestApi.Common;

namespace NorthwindRestApi.Extensions
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration
                .GetSection("Jwt")
                .Get<JwtSettings>()
                ?? throw new InvalidOperationException("Jwt settings are missing.");

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Key)),

                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("JWT authentication failed:");
                            Console.WriteLine(context.Exception.ToString());
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            Console.WriteLine("JWT challenge triggered:");
                            Console.WriteLine($"Error: {context.Error}");
                            Console.WriteLine($"Description: {context.ErrorDescription}");
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}
