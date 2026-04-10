using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.DTOs.Customers;
using NorthwindRestApi.DTOs.Employees;
using NorthwindRestApi.DTOs.Orders;

namespace NorthwindRestApi.Extensions
{
    public static class EmployeeQueryableExtensions
    {
        public static IQueryable<EmployeeReadDto> ApplyFilter(
            this IQueryable<EmployeeReadDto> query,
            EmployeeQueryParameters parameters)
        {
            query = query.IgnoreQueryFilters();

            if (parameters.Start.HasValue)
            {
                var startDate = DateTime.SpecifyKind(
                    parameters.Start.Value.Date,
                    DateTimeKind.Unspecified);

                query = query.Where(o =>
                    o.HireDate.HasValue &&
                    o.HireDate.Value >= startDate);
            }

            if (parameters.End.HasValue)
            {
                var endExclusive = DateTime.SpecifyKind(
                    parameters.End.Value.Date.AddDays(1),
                    DateTimeKind.Unspecified);

                query = query.Where(o =>
                    o.HireDate.HasValue &&
                    o.HireDate.Value < endExclusive);
            }

            if (parameters.IsDeleted.HasValue)
            {
                query = query.Where(p => p.IsDeleted == parameters.IsDeleted.Value);
            }

            if (parameters.ReportsTo.HasValue)
            {
                query = query.Where(p => p.ReportsTo == parameters.ReportsTo.Value);
            }

            return query;
        }
        public static IQueryable<EmployeeReadDto> ApplySearch(
            this IQueryable<EmployeeReadDto> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var term = searchTerm.Trim();

            return query.Where(e =>
                (e.LastName != null && e.LastName.Contains(term)) ||
                (e.FirstName != null && e.FirstName.Contains(term)) ||
                (e.Title != null && e.Title.Contains(term)) ||
                (e.TitleOfCourtesy != null && e.TitleOfCourtesy.Contains(term)) ||
                (e.Address != null && e.Address.Contains(term)) ||
                (e.City != null && e.City.Contains(term)) ||
                (e.Region != null && e.Region.Contains(term)) ||
                (e.PostalCode != null && e.PostalCode.Contains(term)) ||
                (e.Country != null && e.Country.Contains(term)) ||
                (e.HomePhone != null && e.HomePhone.Contains(term)) ||
                (e.Extension != null && e.Extension.Contains(term)) ||
                (e.ReportsToFullName != null && e.ReportsToFullName.Contains(term)));
        }

        public static IQueryable<EmployeeReadDto> ApplySorting(
            this IQueryable<EmployeeReadDto> query,
            string? orderBy,
            bool descending)
        {
            var key = orderBy?.Trim().ToLowerInvariant();

            return key switch
            {
                "employeeid" => descending
                    ? query.OrderByDescending(e => e.EmployeeID)
                    : query.OrderBy(e => e.EmployeeID),

                "lastname" => descending
                    ? query.OrderByDescending(e => e.LastName)
                    : query.OrderBy(e => e.LastName),

                "firstname" => descending
                    ? query.OrderByDescending(e => e.FirstName)
                    : query.OrderBy(e => e.FirstName),

                "title" => descending
                    ? query.OrderByDescending(e => e.Title)
                    : query.OrderBy(e => e.Title),

                "titleofcourtesy" => descending
                    ? query.OrderByDescending(e => e.TitleOfCourtesy)
                    : query.OrderBy(e => e.TitleOfCourtesy),

                "birthdate" => descending
                    ? query.OrderByDescending(e => e.BirthDate)
                    : query.OrderBy(e => e.BirthDate),

                "hiredate" => descending
                    ? query.OrderByDescending(e => e.HireDate)
                    : query.OrderBy(e => e.HireDate),

                "address" => descending
                    ? query.OrderByDescending(e => e.Address)
                    : query.OrderBy(e => e.Address),  

                "city" => descending
                    ? query.OrderByDescending(e => e.City)
                    : query.OrderBy(e => e.City),

                "region" => descending
                    ? query.OrderByDescending(e => e.Region)
                    : query.OrderBy(e => e.Region),

                "postalcode" => descending
                    ? query.OrderByDescending(e => e.PostalCode)
                    : query.OrderBy(e => e.PostalCode),

                "country" => descending
                    ? query.OrderByDescending(e => e.Country)
                    : query.OrderBy(e => e.Country),

                "homephone" => descending
                    ? query.OrderByDescending(e => e.HomePhone)
                    : query.OrderBy(e => e.HomePhone),

                "extension" => descending
                    ? query.OrderByDescending(e => e.Extension)
                    : query.OrderBy(e => e.Extension),

                "notes" => descending
                    ? query.OrderByDescending(e => e.Notes)
                    : query.OrderBy(e => e.Notes),

                "reportsto" => descending
                    ? query.OrderByDescending(e => e.ReportsTo)
                    : query.OrderBy(e => e.ReportsTo),

                "reporstofullname" => descending
                    ? query.OrderByDescending(e => e.ReportsToFullName)
                    : query.OrderBy(e => e.ReportsToFullName),

                "photopath" => descending
                    ? query.OrderByDescending(e => e.PhotoPath)
                    : query.OrderBy(e => e.PhotoPath),

                _ => query.OrderBy(e => e.EmployeeID)
            };
        }
    }
}
