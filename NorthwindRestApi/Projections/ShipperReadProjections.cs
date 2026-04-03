using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.DTOs.Shippers;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class ShipperReadProjections
    {
        public static IQueryable<ShipperReadDto> Build(
            IQueryable<Shipper> shippers,
            IQueryable<OrderReadDto> orders)
        {
            return shippers
                .Select(s => new ShipperReadDto
                {
                    ShipperID = s.ShipperID,
                    CompanyName = s.CompanyName,
                    Phone = s.Phone,
                    RegionID = s.RegionID,
                    RegionDescription = s.Region != null ? s.Region.RegionDescription : string.Empty,
                    IsDeleted = s.IsDeleted,

                    Orders = orders
                        .Where(o => o.ShipVia == s.ShipperID)
                        .OrderBy(o => o.OrderID)
                        .ToList(),
                    OrderCount = orders
                        .Count(o => o.ShipVia == s.ShipperID)
                });
        }
    }
}
