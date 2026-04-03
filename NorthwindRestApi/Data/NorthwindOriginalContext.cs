using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Data;

public partial class NorthwindOriginalContext : DbContext
{
    public NorthwindOriginalContext(DbContextOptions<NorthwindOriginalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerDemographic> CustomerDemographics { get; set; }

    public virtual DbSet<Documentation> Documentations { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Order_Detail> Order_Details { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Shipper> Shippers { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Territory> Territories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Property(e => e.CustomerID).IsFixedLength();

            entity.HasMany(d => d.CustomerTypes).WithMany(p => p.Customers)
                .UsingEntity<Dictionary<string, object>>(
                    "CustomerCustomerDemo",
                    r => r.HasOne<CustomerDemographic>().WithMany()
                        .HasForeignKey("CustomerTypeID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CustomerCustomerDemo"),
                    l => l.HasOne<Customer>().WithMany()
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CustomerCustomerDemo_Customers"),
                    j =>
                    {
                        j.HasKey("CustomerID", "CustomerTypeID").IsClustered(false);
                        j.ToTable("CustomerCustomerDemo");
                        j.IndexerProperty<string>("CustomerID")
                            .HasMaxLength(5)
                            .IsFixedLength();
                        j.IndexerProperty<string>("CustomerTypeID")
                            .HasMaxLength(10)
                            .IsFixedLength();
                    });
        });

        modelBuilder.Entity<CustomerDemographic>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasKey(e => e.CustomerTypeID).IsClustered(false);

            entity.Property(e => e.CustomerTypeID).IsFixedLength();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(d => d.ReportsToNavigation).WithMany(p => p.InverseReportsToNavigation).HasConstraintName("FK_Employees_Employees");

            entity.HasMany(d => d.Territories).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeTerritory",
                    r => r.HasOne<Territory>().WithMany()
                        .HasForeignKey("TerritoryID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EmployeeTerritories_Territories"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EmployeeTerritories_Employees"),
                    j =>
                    {
                        j.HasKey("EmployeeID", "TerritoryID").IsClustered(false);
                        j.ToTable("EmployeeTerritories");
                        j.IndexerProperty<string>("TerritoryID").HasMaxLength(20);
                    });
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.LoginID).HasName("PK_Login");

            entity.Property(e => e.Password).UseCollation("SQL_Latin1_General_CP1_CS_AS");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Property(e => e.CustomerID).IsFixedLength();
            entity.Property(e => e.Freight).HasDefaultValue(0m, "DF_Orders_Freight");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Employees");

            entity.HasOne(d => d.ShipViaNavigation).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Shippers");
        });

        modelBuilder.Entity<Order_Detail>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasKey(e => new { e.OrderID, e.ProductID }).HasName("PK_Order_Details");

            entity.Property(e => e.Quantity).HasDefaultValue((short)1, "DF_Order_Details_Quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.Order_Details)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Details_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.Order_Details)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Details_Products");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasQueryFilter(e => !e.Discontinued);

            entity.Property(e => e.ReorderLevel).HasDefaultValue((short)0, "DF_Products_ReorderLevel");
            entity.Property(e => e.UnitPrice).HasDefaultValue(0m, "DF_Products_UnitPrice");
            entity.Property(e => e.UnitsInStock).HasDefaultValue((short)0, "DF_Products_UnitsInStock");
            entity.Property(e => e.UnitsOnOrder).HasDefaultValue((short)0, "DF_Products_UnitsOnOrder");

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products).HasConstraintName("FK_Products_Suppliers");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasKey(e => e.RegionID).IsClustered(false);

            entity.Property(e => e.RegionID).ValueGeneratedNever();
            entity.Property(e => e.RegionDescription).IsFixedLength();
        });

        modelBuilder.Entity<Shipper>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(d => d.Region).WithMany(p => p.Shippers).HasConstraintName("FK_Shippers_Region");
        });

        modelBuilder.Entity<Territory>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasKey(e => e.TerritoryID).IsClustered(false);

            entity.Property(e => e.TerritoryDescription).IsFixedLength();

            entity.HasOne(d => d.Region).WithMany(p => p.Territories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Territories_Region");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
