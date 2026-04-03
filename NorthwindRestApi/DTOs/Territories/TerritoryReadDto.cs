using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Territories
{
    public class TerritoryReadDto
    {
        public string TerritoryID { get; set; }
                
        public string TerritoryDescription { get; set; }

        public int RegionID { get; set; }

        public string RegionDescription { get; set; }

        public bool IsDeleted { get; set; }
    }
}
