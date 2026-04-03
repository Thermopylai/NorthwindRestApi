using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NorthwindRestApi.Models.Entities;

public partial class CustomerDemographic
{
    [Key]
    [StringLength(10)]
    public string CustomerTypeID { get; set; } = null!;

    [Column(TypeName = "ntext")]
    public string? CustomerDesc { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("CustomerTypeID")]
    [InverseProperty("CustomerTypes")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
