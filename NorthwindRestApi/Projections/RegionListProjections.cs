using NorthwindRestApi.DTOs.Regions;
using NorthwindRestApi.DTOs.Shippers;
using NorthwindRestApi.DTOs.Territories;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class RegionListProjections
    {
        public static IQueryable<RegionListDto> Build(
            IQueryable<Region> regions,
            IQueryable<TerritoryReadDto> territories,
            IQueryable<ShipperListDto> shippers)
        {
            return regions
                .Select(r => new RegionListDto
                {
                    RegionID = r.RegionID,
                    RegionDescription = r.RegionDescription,
                    IsDeleted = r.IsDeleted,

                    Territories = territories
                        .Where(t => t.RegionID == r.RegionID)
                        .OrderBy(t => t.TerritoryID)
                        .ToList(),
                    TerritoryCount = territories
                        .Count(t => t.RegionID == r.RegionID),

                    Shippers = shippers
                        .Where(s => s.RegionID == r.RegionID)
                        .OrderBy(s => s.ShipperID)
                        .ToList(),
                    ShipperCount = shippers
                        .Count(s => s.RegionID == r.RegionID)
                });
        }
    }
}
