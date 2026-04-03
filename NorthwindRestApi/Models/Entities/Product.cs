using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NorthwindRestApi.Models.Entities;

[Index("CategoryID", Name = "CategoriesProducts")]
[Index("CategoryID", Name = "CategoryID")]
[Index("ProductName", Name = "ProductName")]
[Index("SupplierID", Name = "SupplierID")]
[Index("SupplierID", Name = "SuppliersProducts")]
public partial class Product
{
    [Key]
    public int ProductID { get; set; }

    [StringLength(40)]
    public string ProductName { get; set; } = null!;

    public int? SupplierID { get; set; }

    public int? CategoryID { get; set; }

    [StringLength(20)]
    public string? QuantityPerUnit { get; set; }

    [Column(TypeName = "money")]
    public decimal? UnitPrice { get; set; }

    public short? UnitsInStock { get; set; }

    public short? UnitsOnOrder { get; set; }

    public short? ReorderLevel { get; set; }

    public bool Discontinued { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ImageLink { get; set; }

    [ForeignKey("CategoryID")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<Order_Detail> Order_Details { get; set; } = new List<Order_Detail>();

    [ForeignKey("SupplierID")]
    [InverseProperty("Products")]
    public virtual Supplier? Supplier { get; set; }
}
