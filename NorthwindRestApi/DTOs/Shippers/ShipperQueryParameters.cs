namespace NorthwindRestApi.DTOs.Shippers
{
    public class ShipperQueryParameters
    {
        // Filter parameters
        public int? RegionID { get; set; }
        public bool? IsDeleted { get; set; }

        // Search parameter
        public string? SearchTerm { get; set; }

        // Sorting parameters
        public string? OrderBy { get; set; }
        public bool Descending { get; set; } = false;

        // Pagination parameters
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
