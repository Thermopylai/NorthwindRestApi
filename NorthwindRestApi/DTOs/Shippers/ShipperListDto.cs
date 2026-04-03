namespace NorthwindRestApi.DTOs.Shippers
{
    public class ShipperListDto
    {
        public int ShipperID { get; set; }

        public string CompanyName { get; set; }

        public string? Phone { get; set; }

        public int? RegionID { get; set; }

        public string RegionDescription { get; set; }

        public bool IsDeleted { get; set; }
    }
}
