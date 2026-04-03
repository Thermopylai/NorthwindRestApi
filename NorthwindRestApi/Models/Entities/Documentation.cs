using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NorthwindRestApi.Models.Entities;

[Table("Documentation")]
public partial class Documentation
{
    [Key]
    public int DocumentationID { get; set; }

    [StringLength(255)]
    public string AvailableRoute { get; set; } = null!;

    [StringLength(20)]
    public string Method { get; set; } = null!;

    public string Description { get; set; } = null!;
}
