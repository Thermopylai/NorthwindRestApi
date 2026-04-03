using NorthwindRestApi.DTOs.Customers;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class CustomerReadProjections
    {
        public static IQueryable<CustomerReadDto> Build(
            IQueryable<Customer> customers,
            IQueryable<OrderReadDto> orders)
        {
            return customers
                .Select(c => new CustomerReadDto
                {
                    CustomerID = c.CustomerID,
                    CompanyName = c.CompanyName,
                    ContactName = c.ContactName,
                    ContactTitle = c.ContactTitle,
                    Address = c.Address,
                    City = c.City,
                    Region = c.Region,
                    PostalCode = c.PostalCode,
                    Country = c.Country,
                    Phone = c.Phone,
                    Fax = c.Fax,
                    IsDeleted = c.IsDeleted,

                    Orders = orders
                        .Where(o => o.CustomerID == c.CustomerID)
                        .OrderByDescending(o => o.OrderDate)
                        .ToList(),
                    OrderCount = orders
                        .Count(o => o.CustomerID == c.CustomerID)
                });
        }
    }
}
