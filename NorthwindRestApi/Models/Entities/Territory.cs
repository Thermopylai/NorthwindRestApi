using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NorthwindRestApi.Models.Entities;

public partial class Territory
{
    [Key]
    [StringLength(20)]
    public string TerritoryID { get; set; } = null!;

    [StringLength(50)]
    public string TerritoryDescription { get; set; } = null!;

    public int RegionID { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("RegionID")]
    [InverseProperty("Territories")]
    public virtual Region Region { get; set; } = null!;

    [ForeignKey("TerritoryID")]
    [InverseProperty("Territories")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
