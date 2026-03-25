using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Valger.Models;

public partial class EquipmentRentalDbContext : DbContext
{
    public EquipmentRentalDbContext()
    {
    }

    public EquipmentRentalDbContext(DbContextOptions<EquipmentRentalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<EquipmentType> EquipmentTypes { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<PickupPoint> PickupPoints { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EquipmentRentalDB;TrustServerCertificate=True;Integrated Security=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__Equipmen__3447447948D4F6D2");

            entity.HasIndex(e => e.Article, "IX_Equipment_Article");

            entity.HasIndex(e => e.Article, "UQ__Equipmen__4943444A2F8205BE").IsUnique();

            entity.Property(e => e.Article).HasMaxLength(50);
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Photo).HasMaxLength(500);
            entity.Property(e => e.RentalCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RentalUnit).HasMaxLength(50);

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("FK_Equipment_Manufacturers");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_Equipment_Suppliers");

            entity.HasOne(d => d.Type).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_Equipment_Types");
        });

        modelBuilder.Entity<EquipmentType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Equipmen__516F03B500B32C17");

            entity.HasIndex(e => e.TypeName, "UQ__Equipmen__D4E7DFA88EE3A836").IsUnique();

            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("PK__Manufact__357E5CC19830105D");

            entity.HasIndex(e => e.ManufacturerName, "UQ__Manufact__3B9CDE2E219DCD13").IsUnique();

            entity.Property(e => e.ManufacturerName).HasMaxLength(100);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF4A31B74E");

            entity.HasIndex(e => e.OrderNumber, "IX_Orders_OrderNumber");

            entity.Property(e => e.OrderNumber).HasColumnType("decimal(10, 0)");
            entity.Property(e => e.RentalStartDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasDefaultValue(1);

            entity.HasOne(d => d.ClientUser).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Users");

            entity.HasOne(d => d.Equipment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.EquipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Equipment");

            entity.HasOne(d => d.PickupPoint).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PickupPointId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_PickupPoints");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Statuses");
        });

        modelBuilder.Entity<PickupPoint>(entity =>
        {
            entity.HasKey(e => e.PointId).HasName("PK__PickupPo__40A977E1CA6FA358");

            entity.Property(e => e.Address).HasMaxLength(500);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A502332BA");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160C89EA462").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Statuses__C8EE2063644053F9");

            entity.HasIndex(e => e.StatusName, "UQ__Statuses__05E7698A1C07F098").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B412E69CE2");

            entity.HasIndex(e => e.SupplierName, "UQ__Supplier__9C5DF66F9BC7BFFA").IsUnique();

            entity.Property(e => e.SupplierName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CDECD4F55");

            entity.HasIndex(e => e.Login, "IX_Users_Login");

            entity.HasIndex(e => e.Login, "UQ__Users__5E55825B0F7BC403").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
