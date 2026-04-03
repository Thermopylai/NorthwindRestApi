using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.DTOs.Suppliers;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class SupplierReadProjections
    {
        public static IQueryable<SupplierReadDto> Build(
            IQueryable<Supplier> suppliers,
            IQueryable<ProductListDto> products)
        {
            return suppliers
                .Select(s => new SupplierReadDto
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
                    IsDeleted = s.IsDeleted,

                    Products = products
                        .Where(p => p.SupplierID == s.SupplierID)
                        .OrderBy(p => p.ProductID)
                        .ToList(),
                    ProductCount = products
                        .Count(p => p.SupplierID == s.SupplierID)
                });
        }
    }
}
