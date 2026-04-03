namespace NorthwindRestApi.DTOs.Order_Details
{
    public class Order_DetailQueryParameters
    {
        // Filter parameters
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? CategoryId {  get; set; }
        public int? SupplierId { get; set; }
        public int? EmployeeId {  get; set; }
        public int? ShipVia { get; set; }
        public decimal? VatRate { get; set; }
        public decimal? MinTotalPrice { get; set; }
        public decimal? MaxTotalPrice { get; set; }
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
