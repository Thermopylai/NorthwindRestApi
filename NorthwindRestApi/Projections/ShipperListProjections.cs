using NorthwindRestApi.DTOs.Shippers;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class ShipperListProjections
    {
        public static IQueryable<ShipperListDto> Build(
            IQueryable<Shipper> shippers)
        {
            return shippers
                .Select(s => new ShipperListDto
                {
                    ShipperID = s.ShipperID,
                    CompanyName = s.CompanyName,
                    Phone = s.Phone,
                    RegionID = s.RegionID,
                    RegionDescription = s.Region != null ? s.Region.RegionDescription : string.Empty,
                    IsDeleted = s.IsDeleted,
                });
        }
    }
}
