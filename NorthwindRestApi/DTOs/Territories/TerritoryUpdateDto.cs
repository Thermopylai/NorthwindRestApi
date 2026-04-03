using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Territories
{
    public class TerritoryUpdateDto
    {
        [StringLength(50)]
        public string TerritoryDescription { get; set; }

        public int RegionID { get; set; }

        public bool IsDeleted { get; set; }
    }
}
