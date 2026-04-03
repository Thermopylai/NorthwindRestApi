using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Order_Details;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class Order_DetailReadProjections
    {
        public static IQueryable<Order_DetailReadDto> Build(IQueryable<Order_Detail> orderDetails)
        {
            return orderDetails
                .Select(od => new
                {
                    od.OrderID,
                    od.ProductID,
                    ProductName = od.Product != null ? od.Product.ProductName : "",
                    CategoryID = od.Product != null && od.Product.Category != null ? od.Product.Category.CategoryID : (int?)null,
                    CategoryName = od.Product != null && od.Product.Category != null ? od.Product.Category.CategoryName : "",
                    SupplierID = od.Product != null && od.Product.Supplier != null ? od.Product.Supplier.SupplierID : (int?)null,
                    SupplierName = od.Product != null && od.Product.Supplier != null ? od.Product.Supplier.CompanyName : "",
                    CustomerID = od.Order != null && od.Order.Customer != null ? od.Order.Customer.CustomerID : "",
                    CustomerCompanyName = od.Order != null && od.Order.Customer != null ? od.Order.Customer.CompanyName : "",
                    EmployeeID = od.Order != null && od.Order.Employee != null ? od.Order.Employee.EmployeeID : (int?)null,
                    EmployeeFullName = od.Order != null && od.Order.Employee != null ? od.Order.Employee.FirstName + " " + od.Order.Employee.LastName : "",
                    ShipVia = od.Order != null ? od.Order.ShipVia : (int?)null,
                    ShipViaCompanyName = od.Order != null && od.Order.ShipViaNavigation != null ? od.Order.ShipViaNavigation.CompanyName : "",
                    od.UnitPrice,
                    od.Quantity,
                    od.Discount,
                    od.IsDeleted,
                    TotalPrice = od.Quantity * od.UnitPrice,
                    PriceWithDiscount = od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount),
                    VatRate = od.Product != null ? (od.Product.Category != null &&
                                      VatRules.ReducedVatCategories.Contains(od.Product.Category.CategoryName)
                                            ? VatRules.ReducedVatRate
                                            : VatRules.StandardVatRate) : VatRules.ReducedVatRate
                })
                .Select(x => new Order_DetailReadDto
                {
                    OrderID = x.OrderID,
                    ProductID = x.ProductID,
                    ProductName = x.ProductName,
                    CategoryID = x.CategoryID,
                    CategoryName = x.CategoryName,
                    SupplierID = x.SupplierID,
                    SupplierName = x.SupplierName,
                    CustomerID = x.CustomerID,
                    CustomerCompanyName = x.CustomerCompanyName,
                    EmployeeID = x.EmployeeID,
                    EmployeeFullName = x.EmployeeFullName,
                    ShipVia = x.ShipVia,
                    ShipViaCompanyName = x.ShipViaCompanyName,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity,
                    Discount = x.Discount,
                    IsDeleted = x.IsDeleted,
                    TotalPrice = x.TotalPrice,
                    PriceWithDiscount = x.PriceWithDiscount,
                    VatRate = x.VatRate,
                    VatAmount = Math.Round(x.PriceWithDiscount * x.VatRate, 2),
                    PriceWithVat = Math.Round(x.PriceWithDiscount * (1 + x.VatRate), 2)
                });
        }
    }
}
