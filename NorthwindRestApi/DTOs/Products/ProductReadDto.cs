using NorthwindRestApi.DTOs.Customers;
using NorthwindRestApi.DTOs.Order_Details;
using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Products
{
    public class ProductReadDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public int? SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public int? CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? QuantityPerUnit { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public string? ImageLink { get; set; }
        public decimal? VatRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? VatAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? PriceWithVat { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? StockValueWithVat { get; set; }

        public List<Order_DetailReadDto> OrderDetails { get; set; } = new();
        public int? OrderDetailsCount { get; set; }
        public List<CustomerListDto> Customers { get; set; } = new();
        public int? CustomersCount { get; set; }
    }
}
