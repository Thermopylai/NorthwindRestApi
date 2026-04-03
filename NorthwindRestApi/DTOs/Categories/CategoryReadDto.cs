using NorthwindRestApi.DTOs.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindRestApi.DTOs.Categories
{
    public class CategoryReadDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal VatRate { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public List<ProductListDto> Products { get; set; } = new();
        public int? ProductCount { get; set; }
    }
}
