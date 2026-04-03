using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindRestApi.DTOs.Order_Details
{
    public class Order_DetailReadDto
    {
        [Key]
        public int OrderID { get; set; }

        [Key]
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public int? CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public int? SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public string? CustomerID { get; set; }
        public string? CustomerCompanyName { get; set; }
        public int? EmployeeID { get; set; }
        public string? EmployeeFullName { get; set; }
        public int? ShipVia { get; set; }
        public string? ShipViaCompanyName { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public float Discount { get; set; }

        public bool IsDeleted { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? TotalPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? PriceWithDiscount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? VatRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? VatAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? PriceWithVat { get; set; }
    }
}
