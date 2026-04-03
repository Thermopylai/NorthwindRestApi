using NorthwindRestApi.Common;

namespace NorthwindRestApi.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddApplicationAuthorization(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.AdminOnly, policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy(AuthorizationPolicies.CanReadCategories, policy =>
                    policy.RequireClaim("permission",
                        Permissions.CategoriesRead,
                        Permissions.CategoriesManage));

                options.AddPolicy(AuthorizationPolicies.CanManageCategories, policy =>
                    policy.RequireClaim("permission",
                        Permissions.CategoriesManage));

                options.AddPolicy(AuthorizationPolicies.CanReadCustomers, policy =>
                    policy.RequireClaim("permission",
                        Permissions.CustomersRead,
                        Permissions.CustomersManage));

                options.AddPolicy(AuthorizationPolicies.CanManageCustomers, policy =>
                    policy.RequireClaim("permission",
                        Permissions.CustomersManage));

                options.AddPolicy(AuthorizationPolicies.CanReadEmployees, policy =>
                    policy.RequireClaim("permission",
                        Permissions.EmployeesRead,
                        Permissions.EmployeesManage));

                options.AddPolicy(AuthorizationPolicies.CanManageEmployees, policy =>
                    policy.RequireClaim("permission",
                        Permissions.EmployeesManage));

                options.AddPolicy(AuthorizationPolicies.CanReadOrderDetails, policy =>
                    policy.RequireClaim("permission",
                        Permissions.OrderDetailsRead,
                        Permissions.OrderDetailsManage));

                options.AddPolicy(AuthorizationPolicies.CanManageOrderDetails, policy =>
                    policy.RequireClaim("permission",
                        Permissions.OrderDetailsManage));

                options.AddPolicy(AuthorizationPolicies.CanReadOrders, policy =>
                    policy.RequireClaim("permission",
                        Permissions.OrdersRead,
                        Permissions.OrdersManage));

                options.AddPolicy(AuthorizationPolicies.CanManageOrders, policy =>
                    policy.RequireClaim("permission",
                        Permissions.OrdersManage));

                options.AddPolicy(AuthorizationPolicies.CanReadProducts, policy =>
                    policy.RequireClaim("permission",
                        Permissions.ProductsRead,
                        Permissions.ProductsManage));

                options.AddPolicy(AuthorizationPolicies.CanManageProducts, policy =>
                    policy.RequireClaim("permission",
                        Permissions.ProductsManage));

                options.AddPolicy(AuthorizationPolicies.CanReadRegions, policy =>
                    policy.RequireClaim("permission",
                        Permissions.RegionsRead,
                        Permissions.RegionsManage));

                options.AddPolicy(AuthorizationPolicies.CanManageRegions, policy =>
                    policy.RequireClaim("permission",
                        Permissions.RegionsManage));

                options.AddPolicy(AuthorizationPolicies.CanReadShippers, policy =>
                    policy.RequireClaim("permission",
                        Permissions.ShippersRead,
                        Permissions.ShippersManage));

                options.AddPolicy(AuthorizationPolicies.CanManageShippers, policy =>
                    policy.RequireClaim("permission",
                        Permissions.ShippersManage));

                options.AddPolicy(AuthorizationPolicies.CanReadSuppliers, policy =>
                    policy.RequireClaim("permission",
                        Permissions.SuppliersRead,
                        Permissions.SuppliersManage));

                options.AddPolicy(AuthorizationPolicies.CanManageSuppliers, policy =>
                    policy.RequireClaim("permission",
                        Permissions.SuppliersManage));

                options.AddPolicy(AuthorizationPolicies.CanReadTerritories, policy =>
                    policy.RequireClaim("permission",
                        Permissions.TerritoriesRead,
                        Permissions.TerritoriesManage));

                options.AddPolicy(AuthorizationPolicies.CanManageTerritories, policy =>
                    policy.RequireClaim("permission",
                        Permissions.TerritoriesManage));
            });

            return services;
        }
    }
}