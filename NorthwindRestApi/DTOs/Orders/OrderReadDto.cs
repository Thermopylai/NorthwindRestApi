using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NorthwindRestApi.DTOs.Order_Details;

namespace NorthwindRestApi.DTOs.Orders
{
    public class OrderReadDto
    {
        public int OrderID { get; set; }
        public string? CustomerID { get; set; }
        public string? CustomerCompanyName { get; set; }
        public int? EmployeeID { get; set; }
        public string? EmployeeFullName { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; }
        public string? ShipViaCompanyName { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public bool IsDeleted { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalVatAmount { get; set; }
        public decimal? TotalAmountWithVat { get; set; }
        public decimal? FinalAmount { get; set; }
        public List<Order_DetailReadDto> OrderDetails { get; set; } = new();
        public int? OrderRowCount { get; set; }
    }
}
