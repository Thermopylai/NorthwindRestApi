using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.DTOs.Suppliers;

namespace NorthwindRestApi.Extensions
{
    public static class SupplierQueryableExtensions
    {
        public static IQueryable<SupplierListDto> ApplyFilter(
                this IQueryable<SupplierListDto> query,
                SupplierQueryParameters parameters)
        {
            query = query.IgnoreQueryFilters();

            if (parameters.IsDeleted.HasValue)
            {
                query = query.Where(p => p.IsDeleted == parameters.IsDeleted.Value);
            }

            return query;
        }

        public static IQueryable<SupplierListDto> ApplySearch(
            this IQueryable<SupplierListDto> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var term = searchTerm.Trim();

            return query.Where(s =>
                (s.CompanyName != null && s.CompanyName.Contains(term)) ||
                (s.ContactName != null && s.ContactName.Contains(term)) ||
                (s.ContactTitle != null && s.ContactTitle.Contains(term)) ||
                (s.Address != null && s.Address.Contains(term)) ||
                (s.City != null && s.City.Contains(term)) ||
                (s.Region != null && s.Region.Contains(term)) ||
                (s.PostalCode != null && s.PostalCode.Contains(term)) ||
                (s.Country != null && s.Country.Contains(term)) ||
                (s.Phone != null && s.Phone.Contains(term)) ||
                (s.Fax != null && s.Fax.Contains(term)) ||
                (s.HomePage != null && s.HomePage.Contains(term)));
        }

        public static IQueryable<SupplierListDto> ApplySorting(
            this IQueryable<SupplierListDto> query,
            string? orderBy,
            bool descending)
        {
            var key = orderBy?.Trim().ToLowerInvariant();

            return key switch
            {
                "supplierid" => descending
                    ? query.OrderByDescending(c => c.SupplierID)
                    : query.OrderBy(c => c.SupplierID),

                "companyname" => descending
                    ? query.OrderByDescending(c => c.CompanyName)
                    : query.OrderBy(c => c.CompanyName),

                "contacttitle" => descending
                    ? query.OrderByDescending(c => c.ContactTitle)
                    : query.OrderBy(c => c.ContactTitle),

                "address" => descending
                    ? query.OrderByDescending(c => c.Address)
                    : query.OrderBy(c => c.Address),

                "city" => descending
                    ? query.OrderByDescending(c => c.City)
                    : query.OrderBy(c => c.City),

                "region" => descending
                    ? query.OrderByDescending(c => c.Region)
                    : query.OrderBy(c => c.Region),

                "postalcode" => descending
                    ? query.OrderByDescending(c => c.PostalCode)
                    : query.OrderBy(c => c.PostalCode),

                "country" => descending
                    ? query.OrderByDescending(c => c.Country)
                    : query.OrderBy(c => c.Country),

                "phone" => descending
                    ? query.OrderByDescending(c => c.Phone)
                    : query.OrderBy(c => c.Phone),

                "fax" => descending
                    ? query.OrderByDescending(c => c.Fax)
                    : query.OrderBy(c => c.Fax),

                "homepage" => descending
                    ? query.OrderByDescending(c => c.HomePage)
                    : query.OrderBy(c => c.HomePage),

                _ => query.OrderBy(c => c.SupplierID)
            };
        }
    }
}
