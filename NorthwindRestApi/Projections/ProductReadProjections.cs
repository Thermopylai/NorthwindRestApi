using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Customers;
using NorthwindRestApi.DTOs.Order_Details;
using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class ProductReadProjections
    {
        public static IQueryable<ProductReadDto> Build(
            IQueryable<Product> products,
            IQueryable<Order_DetailReadDto> orderDetails,
            IQueryable<CustomerListDto> customers)
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
                .Select(x => new ProductReadDto
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
                    StockValue = x.UnitPrice.HasValue && x.UnitsInStock.HasValue
                        ? Math.Round(x.UnitPrice.Value * x.UnitsInStock.Value, 2)
                        : null,
                    StockValueWithVat = x.UnitPrice.HasValue && x.UnitsInStock.HasValue
                        ? Math.Round(x.UnitPrice.Value * (1 + x.VatRate) * x.UnitsInStock.Value, 2)
                        : null,

                    OrderDetails = orderDetails
                        .Where(od => od.ProductID == x.ProductID)
                        .OrderBy(od => od.OrderID)
                        .ThenBy(od => od.ProductID)
                        .ToList(),
                    OrderDetailsCount = orderDetails.Where(od => od.ProductID == x.ProductID).Count(),

                    Customers = customers
                        .Where(c => orderDetails.Any(od => od.ProductID == x.ProductID && od.CustomerID == c.CustomerID))
                        .Distinct()
                        .OrderBy(c => c.CustomerID)
                        .ToList(),
                });
        }
    }
}
