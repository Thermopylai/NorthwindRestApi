using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.DTOs.Orders;

namespace NorthwindRestApi.Extensions
{
    public static class OrderQueryableExtensions
    {
        public static IQueryable<OrderListDto> ApplyFilter(
            this IQueryable<OrderListDto> query,
            OrderQueryParameters parameters)
        {
            query = query.IgnoreQueryFilters();

            if (parameters.EmployeeId.HasValue)
            {
                query = query.Where(o => o.EmployeeID == parameters.EmployeeId.Value);
            }

            if (parameters.Start.HasValue)
            {
                var startDate = DateTime.SpecifyKind(
                    parameters.Start.Value.Date,
                    DateTimeKind.Unspecified);

                query = query.Where(o =>
                    o.OrderDate.HasValue &&
                    o.OrderDate.Value >= startDate);
            }

            if (parameters.End.HasValue)
            {
                var endExclusive = DateTime.SpecifyKind(
                    parameters.End.Value.Date.AddDays(1),
                    DateTimeKind.Unspecified);

                query = query.Where(o =>
                    o.OrderDate.HasValue &&
                    o.OrderDate.Value < endExclusive);
            }

            if (parameters.ShipVia.HasValue)
            {
                query = query.Where(o => o.ShipVia == parameters.ShipVia.Value);
            }

            if (parameters.IsDeleted.HasValue)
            {
                query = query.Where(o => o.IsDeleted == parameters.IsDeleted.Value);
            }

            return query;
        }

        public static IQueryable<OrderListDto> ApplySearch(
            this IQueryable<OrderListDto> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var term = searchTerm.Trim();

            return query.Where(o =>
                (o.CustomerID != null && o.CustomerID.Contains(term)) ||
                (o.CustomerCompanyName != null && o.CustomerCompanyName.Contains(term)) ||
                (o.EmployeeFullName != null && o.EmployeeFullName.Contains(term)) ||
                (o.ShipViaCompanyName != null && o.ShipViaCompanyName.Contains(term)) ||
                (o.ShipName != null && o.ShipName.Contains(term)) ||
                (o.ShipAddress != null && o.ShipAddress.Contains(term)) ||
                (o.ShipCity != null && o.ShipCity.Contains(term)) ||
                (o.ShipRegion != null && o.ShipRegion.Contains(term)) ||
                (o.ShipPostalCode != null && o.ShipPostalCode.Contains(term)) ||
                (o.ShipCountry != null && o.ShipCountry.Contains(term)));
        }

        public static IQueryable<OrderListDto> ApplySorting(
            this IQueryable<OrderListDto> query,
            string? orderBy,
            bool descending)
        {
            var key = orderBy?.Trim().ToLowerInvariant();

            return key switch
            {
                "orderid" => descending
                    ? query.OrderByDescending(o => o.OrderID)
                    : query.OrderBy(o => o.OrderID),

                "customerid" => descending
                    ? query.OrderByDescending(o => o.CustomerID)
                    : query.OrderBy(o => o.CustomerID),

                "customercompanyname" => descending
                    ? query.OrderByDescending(o => o.CustomerCompanyName)
                    : query.OrderBy(o => o.CustomerCompanyName),

                "employeeid" => descending
                    ? query.OrderByDescending(o => o.EmployeeID)
                    : query.OrderBy(o => o.EmployeeID),

                "employeefullname" => descending
                    ? query.OrderByDescending(o => o.EmployeeFullName)
                    : query.OrderBy(o => o.EmployeeFullName),

                "orderdate" => descending
                    ? query.OrderByDescending(o => o.OrderDate)
                    : query.OrderBy(o => o.OrderDate),

                "requireddate" => descending
                    ? query.OrderByDescending(o => o.RequiredDate)
                    : query.OrderBy(o => o.RequiredDate),

                "shippeddate" => descending
                    ? query.OrderByDescending(o => o.ShippedDate)
                    : query.OrderBy(o => o.ShippedDate),

                "shipvia" => descending
                    ? query.OrderByDescending(o => o.ShipVia)
                    : query.OrderBy(o => o.ShipVia),

                "shipviacompanyname" => descending
                    ? query.OrderByDescending(o => o.ShipViaCompanyName)
                    : query.OrderBy(o => o.ShipViaCompanyName),

                "freight" => descending
                    ? query.OrderByDescending(o => o.Freight)
                    : query.OrderBy(o => o.Freight),

                "shipname" => descending
                    ? query.OrderByDescending(o => o.ShipName)
                    : query.OrderBy(o => o.ShipName),

                "shipaddress" => descending
                    ? query.OrderByDescending(o => o.ShipAddress)
                    : query.OrderBy(o => o.ShipAddress),

                "shipcity" => descending
                    ? query.OrderByDescending(o => o.ShipCity)
                    : query.OrderBy(o => o.ShipCity),

                "shipregion" => descending
                    ? query.OrderByDescending(o => o.ShipRegion)
                    : query.OrderBy(o => o.ShipRegion),

                "shippostalcode" => descending
                    ? query.OrderByDescending(o => o.ShipPostalCode)
                    : query.OrderBy(o => o.ShipPostalCode),

                "shipcountry" => descending
                    ? query.OrderByDescending(o => o.ShipCountry)
                    : query.OrderBy(o => o.ShipCountry),

                _ => query.OrderBy(o => o.OrderID)
            };
        }
    }
}
