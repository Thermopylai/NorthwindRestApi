using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindRestApi.DTOs.Order_Details
{
    public class Order_DetailUpdateDto
    {
        public short Quantity { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public float Discount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
