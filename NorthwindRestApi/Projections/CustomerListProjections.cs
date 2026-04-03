using NorthwindRestApi.DTOs.Customers;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class CustomerListProjections
    {
        public static IQueryable<CustomerListDto> Build(
            IQueryable<Customer> customers)
        {
            return customers
                .Select(c => new CustomerListDto
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
                    IsDeleted = c.IsDeleted
                });
        }
    }
}
