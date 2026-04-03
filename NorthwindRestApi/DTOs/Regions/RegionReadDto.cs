using NorthwindRestApi.DTOs.Shippers;
using NorthwindRestApi.DTOs.Territories;
using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Regions
{
    public class RegionReadDto
    {
        public int RegionID { get; set; }

        public string RegionDescription { get; set; }

        public bool IsDeleted { get; set; }
        public List<TerritoryReadDto> Territories { get; set; } = new();
        public int? TerritoryCount { get; set; }
        public List<ShipperReadDto> Shippers { get; set; } = new();
        public int? ShipperCount { get; set; }
    }
}
