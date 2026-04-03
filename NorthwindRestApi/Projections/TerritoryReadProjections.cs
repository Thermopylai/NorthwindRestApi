using NorthwindRestApi.DTOs.Territories;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class TerritoryReadProjections
    {
        public static IQueryable<TerritoryReadDto> Build(IQueryable<Territory> territories)
        {
            return territories
                .Select(t => new TerritoryReadDto
                {
                    TerritoryID = t.TerritoryID,
                    TerritoryDescription = t.TerritoryDescription,
                    RegionID = t.RegionID,
                    RegionDescription = t.Region != null ? t.Region.RegionDescription : string.Empty,
                    IsDeleted = t.IsDeleted
                });
        }
    }
}
