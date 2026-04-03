using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Regions
{
    public class RegionCreateDto
    {
        [Required]
        [StringLength(50)]
        public string RegionDescription { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
