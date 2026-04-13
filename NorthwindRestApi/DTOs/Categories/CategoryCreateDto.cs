using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindRestApi.DTOs.Categories
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(15)]
        public string CategoryName { get; set; } = "";

        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public IFormFile? Picture { get; set; }
    }
}
