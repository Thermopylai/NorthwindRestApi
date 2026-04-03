using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.DTOs.Suppliers;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class SupplierListProjections
    {
        public static IQueryable<SupplierListDto> Build(
            IQueryable<Supplier> suppliers)
        {
            return suppliers
                .Select(s => new SupplierListDto
                {
                    SupplierID = s.SupplierID,
                    CompanyName = s.CompanyName,
                    ContactName = s.ContactName,
                    ContactTitle = s.ContactTitle,
                    Address = s.Address,
                    City = s.City,
                    Region = s.Region,
                    PostalCode = s.PostalCode,
                    Country = s.Country,
                    Phone = s.Phone,
                    Fax = s.Fax,
                    HomePage = s.HomePage,
                    IsDeleted = s.IsDeleted
                });
        }
    }
}
