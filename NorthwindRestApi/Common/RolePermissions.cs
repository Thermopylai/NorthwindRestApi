namespace NorthwindRestApi.Common
{
    public static class RolePermissions
    {
        public static readonly Dictionary<string, string[]> Map = new()
        {
            ["Admin"] =
            [
                Permissions.CategoriesRead, Permissions.CategoriesManage,
                Permissions.CustomersRead, Permissions.CustomersManage,
                Permissions.EmployeesRead, Permissions.EmployeesManage,
                Permissions.OrderDetailsRead, Permissions.OrderDetailsManage,
                Permissions.OrdersRead, Permissions.OrdersManage,
                Permissions.ProductsRead, Permissions.ProductsManage,
                Permissions.RegionsRead, Permissions.RegionsManage,
                Permissions.ShippersRead, Permissions.ShippersManage,
                Permissions.SuppliersRead, Permissions.SuppliersManage,
                Permissions.TerritoriesRead, Permissions.TerritoriesManage
            ],

            ["User"] =
            [
                Permissions.CategoriesRead,
                Permissions.CustomersRead,
                Permissions.EmployeesRead,
                Permissions.OrderDetailsRead,
                Permissions.OrdersRead,
                Permissions.ProductsRead,
                Permissions.RegionsRead,
                Permissions.ShippersRead,
                Permissions.SuppliersRead,
                Permissions.TerritoriesRead
            ]
        };
    }
}
