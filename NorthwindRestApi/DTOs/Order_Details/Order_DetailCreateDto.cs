using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindRestApi.DTOs.Order_Details
{
    public class Order_DetailCreateDto
    {
        [Key]
        public int OrderID { get; set; }

        [Key]
        public int ProductID { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
