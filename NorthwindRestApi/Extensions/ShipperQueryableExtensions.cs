using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.DTOs.Shippers;

namespace NorthwindRestApi.Extensions
{
    public static class ShipperQueryableExtensions
    {
        public static IQueryable<ShipperListDto> ApplyFilter(
                this IQueryable<ShipperListDto> query,
                ShipperQueryParameters parameters)
        {
            if (parameters.RegionID.HasValue)
            {
                query = query.Where(p => p.RegionID == parameters.RegionID.Value);
            }

            if (parameters.IsDeleted.HasValue)
            {
                query = query.IgnoreQueryFilters().Where(p => p.IsDeleted == parameters.IsDeleted.Value);
            }

            return query;
        }

        public static IQueryable<ShipperListDto> ApplySearch(
            this IQueryable<ShipperListDto> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var term = searchTerm.Trim();

            return query.Where(c =>
                (c.CompanyName != null && c.CompanyName.Contains(term)) ||
                (c.Phone != null && c.Phone.Contains(term)) ||
                (c.RegionDescription != null && c.RegionDescription.Contains(term)));
        }

        public static IQueryable<ShipperListDto> ApplySorting(
            this IQueryable<ShipperListDto> query,
            string? orderBy,
            bool descending)
        {
            var key = orderBy?.Trim().ToLowerInvariant();

            return key switch
            {
                "shipperid" => descending
                    ? query.OrderByDescending(c => c.ShipperID)
                    : query.OrderBy(c => c.ShipperID),

                "companyname" => descending
                    ? query.OrderByDescending(c => c.CompanyName)
                    : query.OrderBy(c => c.CompanyName),

                "phone" => descending
                    ? query.OrderByDescending(c => c.Phone)
                    : query.OrderBy(c => c.Phone),

                "regionid" => descending
                    ? query.OrderByDescending(c => c.RegionID)
                    : query.OrderBy(c => c.RegionID),

                "regiondescription" => descending
                    ? query.OrderByDescending(c => c.RegionDescription)
                    : query.OrderBy(c => c.RegionDescription),

                _ => query.OrderBy(c => c.ShipperID)
            };
        }
    }
}
