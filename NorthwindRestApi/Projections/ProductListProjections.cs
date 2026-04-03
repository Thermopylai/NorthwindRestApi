using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class ProductListProjections
    {
        public static IQueryable<ProductListDto> Build(IQueryable<Product> products)
        {
            return products
                .Select(p => new
                {
                    p.ProductID,
                    p.ProductName,
                    p.SupplierID,
                    SupplierName = p.Supplier != null ? p.Supplier.CompanyName : null,
                    p.CategoryID,
                    CategoryName = p.Category != null ? p.Category.CategoryName : null,
                    p.QuantityPerUnit,
                    p.UnitPrice,
                    p.UnitsInStock,
                    p.UnitsOnOrder,
                    p.ReorderLevel,
                    p.Discontinued,
                    p.ImageLink,
                    VatRate =
                        p.Category != null &&
                            VatRules.ReducedVatCategories.Contains(p.Category.CategoryName)
                                ? VatRules.ReducedVatRate
                                : VatRules.StandardVatRate
                })
                .Select(x => new ProductListDto
                {
                    ProductID = x.ProductID,
                    ProductName = x.ProductName,
                    SupplierID = x.SupplierID,
                    SupplierName = x.SupplierName,
                    CategoryID = x.CategoryID,
                    CategoryName = x.CategoryName,
                    QuantityPerUnit = x.QuantityPerUnit,
                    UnitPrice = x.UnitPrice,
                    UnitsInStock = x.UnitsInStock,
                    UnitsOnOrder = x.UnitsOnOrder,
                    ReorderLevel = x.ReorderLevel,
                    Discontinued = x.Discontinued,
                    ImageLink = x.ImageLink,
                    VatRate = x.VatRate,
                    VatAmount = x.UnitPrice.HasValue
                        ? Math.Round(x.UnitPrice.Value * x.VatRate, 2)
                        : null,
                    PriceWithVat = x.UnitPrice.HasValue
                        ? Math.Round(x.UnitPrice.Value * (1 + x.VatRate), 2)
                        : null,
                    StockValueWithVat = x.UnitPrice.HasValue && x.UnitsInStock.HasValue
                        ? Math.Round(x.UnitPrice.Value * (1 + x.VatRate) * x.UnitsInStock.Value, 2)
                        : null
                });
        }
    }
}
