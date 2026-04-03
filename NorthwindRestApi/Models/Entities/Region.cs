using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NorthwindRestApi.Models.Entities;

[Table("Region")]
public partial class Region
{
    [Key]
    public int RegionID { get; set; }

    [StringLength(50)]
    public string RegionDescription { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [InverseProperty("Region")]
    public virtual ICollection<Shipper> Shippers { get; set; } = new List<Shipper>();

    [InverseProperty("Region")]
    public virtual ICollection<Territory> Territories { get; set; } = new List<Territory>();
}
