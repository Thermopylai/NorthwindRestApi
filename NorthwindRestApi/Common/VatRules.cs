namespace NorthwindRestApi.Common
{
    public static class VatRules
    {
        public const decimal ReducedVatRate = 0.135m;
        public const decimal StandardVatRate = 0.24m;

        public static readonly string[] ReducedVatCategories =
        {
            "Beverages",
            "Condiments",
            "Confections",
            "Dairy Products",
            "Grains/Cereals",
            "Meat/Poultry",
            "Produce",
            "Seafood"
        };
    }
}
