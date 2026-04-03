using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindRestApi.DTOs.Orders
{
    public class OrderCreateDto
    {
        [StringLength(5)]
        public string? CustomerID { get; set; }

        public int? EmployeeID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? OrderDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RequiredDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ShippedDate { get; set; }

        public int? ShipVia { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? Freight { get; set; }

        [StringLength(40)]
        public string? ShipName { get; set; }

        [StringLength(60)]
        public string? ShipAddress { get; set; }

        [StringLength(15)]
        public string? ShipCity { get; set; }

        [StringLength(15)]
        public string? ShipRegion { get; set; }

        [StringLength(10)]
        public string? ShipPostalCode { get; set; }

        [StringLength(15)]
        public string? ShipCountry { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
