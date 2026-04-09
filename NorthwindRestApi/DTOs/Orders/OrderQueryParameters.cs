namespace NorthwindRestApi.DTOs.Orders
{
    public class OrderQueryParameters
    {
        // Filter parameters
        public int? EmployeeId { get; set; }
        public string? CustomerId { get; set; } = null;
        public int? ProductId { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int? ShipVia { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }
        public string? ShipCountry { get; set; }
        public string? ShipCity { get; set; }

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
