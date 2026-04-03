using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class OrderListProjections
    {
        public static IQueryable<OrderListDto> Build(IQueryable<Order> orders)
        {
            return orders
                .Select(o => new OrderListDto
                {
                    OrderID = o.OrderID,
                    CustomerID = o.CustomerID,
                    CustomerCompanyName = o.Customer != null ? o.Customer.CompanyName : null,
                    EmployeeID = o.EmployeeID,
                    EmployeeFullName = o.Employee != null ? o.Employee.FirstName + " " + o.Employee.LastName : null,
                    OrderDate = o.OrderDate,
                    RequiredDate = o.RequiredDate,
                    ShippedDate = o.ShippedDate,
                    ShipVia = o.ShipVia,
                    ShipViaCompanyName = o.ShipViaNavigation != null ? o.ShipViaNavigation.CompanyName : null,
                    Freight = o.Freight,
                    ShipName = o.ShipName,
                    ShipAddress = o.ShipAddress,
                    ShipCity = o.ShipCity,
                    ShipRegion = o.ShipRegion,
                    ShipPostalCode = o.ShipPostalCode,
                    ShipCountry = o.ShipCountry,
                    IsDeleted = o.IsDeleted
                });
        }
    }
}
