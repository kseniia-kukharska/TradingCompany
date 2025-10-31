using Microsoft.EntityFrameworkCore;
using TradingCompanyDalef.Models;

namespace TradingCompanyDal;

public partial class SoftwareContext : DbContext
{
    public SoftwareContext()
    {
    }

    public SoftwareContext(DbContextOptions<SoftwareContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Software;Integrated Security=True; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerId).HasColumnName("CustomerId");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(25);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId).HasColumnName("OrderId");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerId");
            entity.Property(e => e.StatusId).HasColumnName("StatusId");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Statuses");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(e => e.OrderDetailId)
                .ValueGeneratedOnAdd()
                .HasColumnName("OrderDetailId");
            entity.Property(e => e.OrderId).HasColumnName("OrderId");
            entity.Property(e => e.ProductId).HasColumnName("ProductId");

            entity.HasOne(d => d.OrderDetailNavigation).WithOne(p => p.OrderDetail)
                .HasForeignKey<OrderDetail>(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Products");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId).HasColumnName("ProductId");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(9, 2)");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.Property(e => e.StatusId).HasColumnName("StatusId");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
