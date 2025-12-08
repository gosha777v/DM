using Microsoft.EntityFrameworkCore;
using Valger.Models;

namespace Valger.Data
{
    public partial class Context : DbContext
    {
        // Убираем конструктор с Load() - он не нужен
        public Context() { }

        public Context(DbContextOptions<Context> options) : base(options) { }

        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<EquipmentType> EquipmentTypes { get; set; }
        public virtual DbSet<Manufacturer> Manufacturers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        
        public virtual DbSet<PickupPoint> PickupPoints { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Меняем на ваш LocalDB
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\MSSQLLocalDB;Database=EquipmentRentalDB;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Убираем HasName если они мешают
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.EquipmentId);
                entity.HasIndex(e => e.Article).IsUnique();
                entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");
                entity.Property(e => e.Article).HasMaxLength(20);
                entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");
                entity.Property(e => e.Name).HasMaxLength(200);
                entity.Property(e => e.Photo).HasMaxLength(100);
                entity.Property(e => e.RentalCost).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.RentalUnit).HasMaxLength(20);
                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Manufacturer)
                    .WithMany(p => p.Equipment)
                    .HasForeignKey(d => d.ManufacturerId)
                    .HasConstraintName("FK_Equipment_Manufacturers");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Equipment)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_Equipment_Suppliers");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Equipment)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_Equipment_Types");
            });

            modelBuilder.Entity<EquipmentType>(entity =>
            {
                entity.HasKey(e => e.TypeId);
                entity.Property(e => e.TypeId).HasColumnName("TypeID");
                entity.Property(e => e.TypeName).HasMaxLength(50);
            });

            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.HasKey(e => e.ManufacturerId);
                entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");
                entity.Property(e => e.ManufacturerName).HasMaxLength(100);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.ClientUserId).HasColumnName("ClientUserID");
                entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");
                entity.Property(e => e.OrderNumber).HasColumnType("decimal(10, 1)");
                entity.Property(e => e.PickupPointId).HasColumnName("PickupPointID");
                entity.Property(e => e.ReceiptCode).HasColumnType("decimal(10, 1)");
                entity.Property(e => e.RentalStartDate).HasColumnType("datetime");
                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.HasOne(d => d.ClientUser)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ClientUserId)
                    .HasConstraintName("FK_Orders_ClientUsers");

                entity.HasOne(d => d.Equipment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.EquipmentId)
                    .HasConstraintName("FK_Orders_Equipment");

                entity.HasOne(d => d.PickupPoint)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PickupPointId)
                    .HasConstraintName("FK_Orders_PickupPoints");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_Orders_Statuses");
            });

            

            modelBuilder.Entity<PickupPoint>(entity =>
            {
                entity.HasKey(e => e.PointId);
                entity.Property(e => e.PointId).HasColumnName("PointID");
                entity.Property(e => e.Address).HasMaxLength(255);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierId);
                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
                entity.Property(e => e.SupplierName).HasMaxLength(100);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Login).IsUnique();
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Login).HasMaxLength(100);
                entity.Property(e => e.Password).HasMaxLength(50);
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Users_Roles");
            });
        }
    }
}