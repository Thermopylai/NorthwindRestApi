using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.DTOs.Customers;

namespace NorthwindRestApi.Extensions
{
    public static class CustomerQueryableExtensions
    {
        public static IQueryable<CustomerReadDto> ApplyFilter(
                this IQueryable<CustomerReadDto> query,
                CustomerQueryParameters parameters)
        {
            query = query.IgnoreQueryFilters();

            if (!string.IsNullOrWhiteSpace(parameters.Country))
            {
                query = query.Where(p => p.Country != null &&
                                p.Country.Contains(parameters.Country));
            }

            if (!string.IsNullOrWhiteSpace(parameters.City))
            {
                query = query.Where(p => p.City != null &&
                                p.City.Contains(parameters.City));
            }

            if (!string.IsNullOrWhiteSpace(parameters.CompanyName))
            {
                query = query.Where(p => p.CompanyName != null &&
                                p.CompanyName.Contains(parameters.CompanyName));
            }

            if (parameters.IsDeleted.HasValue)
            {
                query = query.Where(p => p.IsDeleted == parameters.IsDeleted.Value);
            }

            return query;
        }

        public static IQueryable<CustomerReadDto> ApplySearch(
            this IQueryable<CustomerReadDto> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var term = searchTerm.Trim();

            return query.Where(c =>
                (c.CustomerID != null && c.CustomerID.Contains(term)) ||
                (c.CompanyName != null && c.CompanyName.Contains(term)) ||
                (c.ContactName != null && c.ContactName.Contains(term)) ||
                (c.ContactTitle != null && c.ContactTitle.Contains(term)) ||
                (c.Address != null && c.Address.Contains(term)) ||
                (c.City != null && c.City.Contains(term)) ||
                (c.Region != null && c.Region.Contains(term)) ||
                (c.PostalCode != null && c.PostalCode.Contains(term)) ||
                (c.Country != null && c.Country.Contains(term)) ||
                (c.Phone != null && c.Phone.Contains(term)) ||
                (c.Fax != null && c.Fax.Contains(term)));
        }

        public static IQueryable<CustomerReadDto> ApplySorting(
            this IQueryable<CustomerReadDto> query,
            string? orderBy,
            bool descending)
        {
            var key = orderBy?.Trim().ToLowerInvariant();

            return key switch
            {
                "customerid" => descending
                    ? query.OrderByDescending(c => c.CustomerID)
                    : query.OrderBy(c => c.CustomerID),

                "companyname" => descending
                    ? query.OrderByDescending(c => c.CompanyName)
                    : query.OrderBy(c => c.CompanyName),

                "contactname" => descending
                    ? query.OrderByDescending(c => c.ContactName)
                    : query.OrderBy(c => c.ContactName),

                "contacttitle" => descending
                    ? query.OrderByDescending(c => c.ContactTitle)
                    : query.OrderBy(c => c.ContactTitle),

                "address" => descending
                    ? query.OrderByDescending(o => o.Address)
                    : query.OrderBy(c => c.Address),

                "city" => descending
                    ? query.OrderByDescending(o => o.City)
                    : query.OrderBy(c => c.City),

                "region" => descending
                    ? query.OrderByDescending(o => o.Region)
                    : query.OrderBy(c => c.Region),

                "postalcode" => descending
                    ? query.OrderByDescending(o => o.PostalCode)
                    : query.OrderBy(c => c.PostalCode),

                "country" => descending
                    ? query.OrderByDescending(o => o.Country)
                    : query.OrderBy(c => c.Country),

                "homephone" => descending
                    ? query.OrderByDescending(o => o.Phone)
                    : query.OrderBy(c => c.Phone),

                "fax" => descending
                    ? query.OrderByDescending(o => o.Fax)
                    : query.OrderBy(c => c.Fax),

                _ => query.OrderBy(c => c.CustomerID)
            };
        }
    }
}
