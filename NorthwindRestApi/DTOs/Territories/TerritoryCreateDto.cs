using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Territories
{
    public class TerritoryCreateDto
    {
        [Key]
        [Required]
        [StringLength(20)]
        public string TerritoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string TerritoryDescription { get; set; }

        [Required]
        public int RegionID { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
