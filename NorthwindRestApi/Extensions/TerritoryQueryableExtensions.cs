using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.DTOs.Territories;

namespace NorthwindRestApi.Extensions
{
    public static class TerritoryQueryableExtensions
    {
        public static IQueryable<TerritoryReadDto> ApplyFilter(
                this IQueryable<TerritoryReadDto> query,
                TerritoryQueryParameters parameters)
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

        public static IQueryable<TerritoryReadDto> ApplySearch(
            this IQueryable<TerritoryReadDto> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var term = searchTerm.Trim();

            return query.Where(c =>
                (c.TerritoryDescription != null && c.TerritoryDescription.Contains(term)) ||
                (c.RegionDescription != null && c.RegionDescription.Contains(term)));
        }

        public static IQueryable<TerritoryReadDto> ApplySorting(
            this IQueryable<TerritoryReadDto> query,
            string? orderBy,
            bool descending)
        {
            var key = orderBy?.Trim().ToLowerInvariant();

            return key switch
            {
                "territoryid" => descending
                    ? query.OrderByDescending(c => c.TerritoryID)
                    : query.OrderBy(c => c.TerritoryID),

                "territorydescription" => descending
                    ? query.OrderByDescending(c => c.TerritoryDescription)
                    : query.OrderBy(c => c.TerritoryDescription),

                "regionid" => descending
                    ? query.OrderByDescending(c => c.RegionID)
                    : query.OrderBy(c => c.RegionID),

                "regiondescription" => descending
                    ? query.OrderByDescending(c => c.RegionDescription)
                    : query.OrderBy(c => c.RegionDescription),

                _ => query.OrderBy(c => c.TerritoryID)
            };
        }
    }
}
