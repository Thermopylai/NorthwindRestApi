using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NorthwindRestApi.Models.Entities;

public partial class Shipper
{
    [Key]
    public int ShipperID { get; set; }

    [StringLength(40)]
    public string CompanyName { get; set; } = null!;

    [StringLength(24)]
    public string? Phone { get; set; }

    public int? RegionID { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("ShipViaNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("RegionID")]
    [InverseProperty("Shippers")]
    public virtual Region? Region { get; set; }
}
