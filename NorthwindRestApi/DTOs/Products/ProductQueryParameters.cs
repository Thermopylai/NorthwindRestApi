namespace NorthwindRestApi.DTOs.Products
{
    public class ProductQueryParameters
    {
        // Filter parameters
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public bool? Discontinued { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? VatRate { get; set; }

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
