namespace NorthwindRestApi.DTOs.Orders
{
    public class OrderQueryParameters
    {
        // Filter parameters
        public int? EmployeeId { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int? ShipVia { get; set; }
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
