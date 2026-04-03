using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Orders
{
    public class OrderListDto
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
    }
}
