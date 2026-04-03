using NorthwindRestApi.DTOs.Orders;
using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Shippers
{
    public class ShipperReadDto
    {
        public int ShipperID { get; set; }
                
        public string CompanyName { get; set; }
                
        public string? Phone { get; set; }

        public int? RegionID { get; set; }

        public string RegionDescription { get; set; }

        public bool IsDeleted { get; set; }

        public List<OrderReadDto> Orders { get; set; } = new();
        public int? OrderCount { get; set; }
    }
}
