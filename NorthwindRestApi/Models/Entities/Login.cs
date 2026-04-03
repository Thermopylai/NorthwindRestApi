using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NorthwindRestApi.Models.Entities;

public partial class Login
{
    [Key]
    public int LoginID { get; set; }

    [StringLength(50)]
    public string UserName { get; set; } = null!;

    [StringLength(100)]
    public string Password { get; set; } = null!;

    [StringLength(100)]
    public string Salt { get; set; } = null!;
}
