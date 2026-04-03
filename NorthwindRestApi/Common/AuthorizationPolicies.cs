namespace NorthwindRestApi.Common
{
    public static class AuthorizationPolicies
    {
        public const string AdminOnly = "AdminOnly";

        public const string CanReadCategories = "CanReadCategories";
        public const string CanManageCategories = "CanManageCategories";

        public const string CanReadCustomers = "CanReadCustomers";
        public const string CanManageCustomers = "CanManageCustomers";

        public const string CanReadEmployees = "CanReadEmployees";
        public const string CanManageEmployees = "CanManageEmployees";

        public const string CanReadOrderDetails = "CanReadOrderDetails";
        public const string CanManageOrderDetails = "CanManageOrderDetails";

        public const string CanReadOrders = "CanReadOrders";
        public const string CanManageOrders = "CanManageOrders";

        public const string CanReadProducts = "CanReadProducts";
        public const string CanManageProducts = "CanManageProducts";

        public const string CanReadRegions = "CanReadRegions";
        public const string CanManageRegions = "CanManageRegions";

        public const string CanReadShippers = "CanReadShippers";
        public const string CanManageShippers = "CanManageShippers";

        public const string CanReadSuppliers = "CanReadSuppliers";
        public const string CanManageSuppliers = "CanManageSuppliers";

        public const string CanReadTerritories = "CanReadTerritories";
        public const string CanManageTerritories = "CanManageTerritories";
    }
}
