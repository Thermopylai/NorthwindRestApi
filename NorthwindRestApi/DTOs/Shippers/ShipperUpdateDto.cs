using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Shippers
{
    public class ShipperUpdateDto
    {
        [StringLength(40)]
        public string CompanyName { get; set; }

        [StringLength(24)]
        public string? Phone { get; set; }

        public int? RegionID { get; set; }

        public bool IsDeleted { get; set; }
    }
}
