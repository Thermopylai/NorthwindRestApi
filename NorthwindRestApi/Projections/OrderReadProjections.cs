using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Order_Details;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class OrderReadProjections
    {
        public static IQueryable<OrderReadDto> Build(IQueryable<Order> orders)
        {
            return orders
                .Select(o => new
                {
                    o.OrderID,
                    o.CustomerID,
                    CustomerCompanyName = o.Customer != null ? o.Customer.CompanyName : null,
                    o.EmployeeID,
                    EmployeeFullName = o.Employee != null ? o.Employee.FirstName + " " + o.Employee.LastName : null,
                    o.OrderDate,
                    o.RequiredDate,
                    o.ShippedDate,
                    o.ShipVia,
                    ShipViaCompanyName = o.ShipViaNavigation != null ? o.ShipViaNavigation.CompanyName : null,
                    o.Freight,
                    o.ShipName,
                    o.ShipAddress,
                    o.ShipCity,
                    o.ShipRegion,
                    o.ShipPostalCode,
                    o.ShipCountry,
                    o.IsDeleted,

                    OrderDetails = o.Order_Details
                        .Where(od => !od.IsDeleted)
                        .Select(od => new
                        {
                            od.OrderID,
                            od.ProductID,
                            ProductName = od.Product != null ? od.Product.ProductName : null,
                            CategoryID = od.Product != null ? od.Product.CategoryID : null,
                            CategoryName = od.Product != null && od.Product.Category != null ? od.Product.Category.CategoryName : null,
                            SupplierID = od.Product != null ? od.Product.SupplierID : null,
                            SupplierName = od.Product != null && od.Product.Supplier != null ? od.Product.Supplier.CompanyName : null,
                            CustomerID = od.Order != null ? od.Order.CustomerID : null,
                            CustomerCompanyName = od.Order != null && od.Order.Customer != null ? od.Order.Customer.CompanyName : null,
                            EmployeeID = od.Order != null ? od.Order.EmployeeID : null,
                            EmployeeFullName = od.Order != null && od.Order.Employee != null ? od.Order.Employee.FirstName + " " + od.Order.Employee.LastName : null,
                            ShipVia = od.Order != null ? od.Order.ShipVia : null,
                            ShipViaCompanyName = od.Order != null && od.Order.ShipViaNavigation != null ? od.Order.ShipViaNavigation.CompanyName : null,
                            od.UnitPrice,
                            od.Quantity,
                            od.Discount,
                            od.IsDeleted,
                            TotalPrice = od.UnitPrice * od.Quantity,
                            PriceWithDiscount = od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount),
                            VatRate = od.Product != null && od.Product.Category != null &&
                                      VatRules.ReducedVatCategories.Contains(od.Product.Category.CategoryName)
                                            ? VatRules.ReducedVatRate
                                            : VatRules.StandardVatRate
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
                        })
                        .OrderBy(od => od.OrderID)
                        .ThenBy(od => od.ProductID)
                        .ToList()
                })
                .Select(o => new OrderReadDto
                {
                    OrderID = o.OrderID,
                    CustomerID = o.CustomerID,
                    CustomerCompanyName = o.CustomerCompanyName,
                    EmployeeID = o.EmployeeID,
                    EmployeeFullName = o.EmployeeFullName,
                    OrderDate = o.OrderDate,
                    RequiredDate = o.RequiredDate,
                    ShippedDate = o.ShippedDate,
                    ShipVia = o.ShipVia,
                    ShipViaCompanyName = o.ShipViaCompanyName,
                    Freight = o.Freight,
                    ShipName = o.ShipName,
                    ShipAddress = o.ShipAddress,
                    ShipCity = o.ShipCity,
                    ShipRegion = o.ShipRegion,
                    ShipPostalCode = o.ShipPostalCode,
                    ShipCountry = o.ShipCountry,
                    IsDeleted = o.IsDeleted,
                    OrderDetails = o.OrderDetails,
                    OrderRowCount = o.OrderDetails.Count,
                    TotalAmount = o.OrderDetails.Sum(od => (decimal)(od.TotalPrice ?? 0)),
                    TotalVatAmount = o.OrderDetails.Sum(od => od.VatAmount),
                    TotalAmountWithVat = o.OrderDetails.Sum(od => od.PriceWithVat),
                    FinalAmount = o.OrderDetails.Sum(od => od.PriceWithVat) + (o.Freight ?? 0)
                });
        }
    }
}
