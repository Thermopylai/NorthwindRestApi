using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NorthwindRestApi.DTOs.Products
{
    public class ProductUpdateDto
    {
        [StringLength(40)]
        public string ProductName { get; set; }
        public int? SupplierID { get; set; }
        public int? CategoryID { get; set; }

        [StringLength(20)]
        public string? QuantityPerUnit { get; set; }

        [Range(0, 100000)]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        [StringLength(100)]
        [Unicode(false)]
        public string? ImageLink { get; set; }
    }
}
