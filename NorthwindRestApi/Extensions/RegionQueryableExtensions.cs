using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.DTOs.Regions;

namespace NorthwindRestApi.Extensions
{
    public static class RegionQueryableExtensions
    {
        public static IQueryable<RegionReadDto> ApplyFilter(
                this IQueryable<RegionReadDto> query,
                RegionQueryParameters parameters)
        {
            query = query.IgnoreQueryFilters();

            if (parameters.IsDeleted.HasValue)
            {
                query = query.Where(p => p.IsDeleted == parameters.IsDeleted.Value);
            }

            return query;
        }

        public static IQueryable<RegionReadDto> ApplySearch(
            this IQueryable<RegionReadDto> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var term = searchTerm.Trim();

            return query.Where(c =>
                (c.RegionDescription != null && c.RegionDescription.Contains(term)));
        }

        public static IQueryable<RegionReadDto> ApplySorting(
            this IQueryable<RegionReadDto> query,
            string? orderBy,
            bool descending)
        {
            var key = orderBy?.Trim().ToLowerInvariant();

            return key switch
            {
                "regionid" => descending
                    ? query.OrderByDescending(c => c.RegionID)
                    : query.OrderBy(c => c.RegionID),

                "regiondescription" => descending
                    ? query.OrderByDescending(c => c.RegionDescription)
                    : query.OrderBy(c => c.RegionDescription),

                "territorycount" => descending
                    ? query.OrderByDescending(c => c.TerritoryCount)
                    : query.OrderBy(c => c.TerritoryCount),

                "shippercount" => descending
                    ? query.OrderByDescending(c => c.ShipperCount)
                    : query.OrderBy(c => c.ShipperCount),

                _ => query.OrderBy(c => c.RegionID)
            };
        }
    }
}
