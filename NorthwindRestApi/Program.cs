using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using NorthwindRestApi.Data;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models.Identity;
using NorthwindRestApi.Services;
using NorthwindRestApi.Services.Interfaces;
using System.Globalization;

namespace NorthwindRestApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddDbContext<NorthwindOriginalContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind")));

            builder.Services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnection")));

            builder.Services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.Password.RequiredLength = 15;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddApplicationAuthorization();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "NorthwindRestApi",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token only. Swagger adds 'Bearer' automatically."
                });
                                
                c.AddSecurityRequirement(d => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", d)] = []
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            // Register application services for dependency injection, allowing them to be injected into controllers and other components as needed.
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrder_DetailService, Order_DetailService>();
            builder.Services.AddScoped<IRegionService, RegionService>();
            builder.Services.AddScoped<IShipperService, ShipperService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<ITerritoryService, TerritoryService>();
                        
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var fi = new CultureInfo("fi-FI");

                options.DefaultRequestCulture = new RequestCulture(fi);
                options.SupportedCultures = new[] { fi };
                options.SupportedUICultures = new[] { fi };

                options.CultureInfoUseUserOverride = false;
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("All", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();
                       
            app.UseRequestLocalization();
                        
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NorthwindRestApi v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseCors("All");

            app.MapControllers();
            
            using (var scope = app.Services.CreateScope())
            {
                await IdentitySeeder.SeedAsync(scope.ServiceProvider);
            }

            app.Run();
        }
    }
}
