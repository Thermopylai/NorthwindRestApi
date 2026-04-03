namespace NorthwindRestApi.DTOs.Categories
{
    public class CategoryListDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal VatRate { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
