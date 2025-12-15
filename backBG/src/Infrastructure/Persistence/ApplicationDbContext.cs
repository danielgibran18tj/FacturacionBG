using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<Role> Roles { get; set; }


    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Invoice> Invoices { get; set; }
    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
    public virtual DbSet<InvoicePaymentMethod> InvoicePaymentMethods { get; set; }
    public virtual DbSet<SystemSetting> SystemSettings { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");

            entity.HasIndex(e => e.DocumentNumber, "IX_Customers_DocumentNumber");
            entity.HasIndex(e => e.Email, "IX_Customers_Email");
            entity.HasIndex(e => e.FullName, "IX_Customers_FullName");
            entity.HasIndex(e => e.UserId, "IX_Customers_UserId");
            entity.HasIndex(e => e.DocumentNumber, "UQ_Customers_DocumentNumber").IsUnique();
            entity.HasIndex(e => e.UserId, "UQ_Customers_UserId_Filtered")
                .IsUnique()
                .HasFilter("([UserId] IS NOT NULL)");

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DocumentNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.User)
                .WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Customers_Users");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoices");

            entity.HasIndex(e => e.CustomerId, "IX_Invoices_CustomerId");
            entity.HasIndex(e => e.InvoiceDate, "IX_Invoices_InvoiceDate");
            entity.HasIndex(e => e.InvoiceNumber, "IX_Invoices_InvoiceNumber");
            entity.HasIndex(e => e.SellerId, "IX_Invoices_SellerId");
            entity.HasIndex(e => e.Status, "IX_Invoices_Status");
            entity.HasIndex(e => e.InvoiceNumber, "UQ_Invoices_InvoiceNumber").IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxIva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("TaxIVA");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Customers");

            entity.HasOne(d => d.Seller).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Users");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.ToTable("InvoiceDetails");

            entity.HasIndex(e => e.InvoiceId, "IX_InvoiceDetails_InvoiceId");
            entity.HasIndex(e => e.ProductId, "IX_InvoiceDetails_ProductId");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_InvoiceDetails_Invoices");

            entity.HasOne(d => d.Product).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceDetails_Products");
        });

        modelBuilder.Entity<InvoicePaymentMethod>(entity =>
        {
            entity.ToTable("InvoicePaymentMethods");

            entity.HasIndex(e => e.InvoiceId, "IX_InvoicePaymentMethods_InvoiceId");

            entity.HasIndex(e => e.PaymentMethodId, "IX_InvoicePaymentMethods_PaymentMethodId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoicePaymentMethods)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_InvoicePaymentMethods_Invoices");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.InvoicePaymentMethods)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoicePaymentMethods_PaymentMethods");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.ToTable("PaymentMethods");

            entity.HasIndex(e => e.Name, "UQ_PaymentMethods_Name").IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            entity.HasIndex(e => e.Code, "IX_Products_Code");
            entity.HasIndex(e => e.IsActive, "IX_Products_IsActive");
            entity.HasIndex(e => e.Name, "IX_Products_Name");
            entity.HasIndex(e => e.Code, "UQ_Products_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Stock).HasDefaultValue(0);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");

            entity.HasIndex(e => e.ExpiresAt, "IX_RefreshTokens_ExpiresAt");
            entity.HasIndex(e => e.Token, "IX_RefreshTokens_Token");
            entity.HasIndex(e => e.UserId, "IX_RefreshTokens_UserId");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            entity.Property(e => e.Token).HasMaxLength(500).IsRequired();
            entity.Property(e => e.IsRevoked).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");

            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.IsActive, "IX_Users_IsActive");

            entity.HasIndex(e => e.Username, "IX_Users_Username");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Users_Username").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");

            entity.HasIndex(e => e.RoleId, "IX_UserRoles_RoleId");
            entity.HasIndex(e => e.UserId, "IX_UserRoles_UserId");
            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UQ_UserRoles_User_Role").IsUnique();
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.ToTable("SystemSettings");

            entity.HasIndex(e => e.SettingKey, "IX_SystemSettings_Key");
            entity.HasIndex(e => e.SettingKey, "UQ_SystemSettings_Key").IsUnique();

            entity.Property(e => e.DataType).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.SettingKey).HasMaxLength(100).IsRequired();
            entity.Property(e => e.SettingValue).HasMaxLength(500).IsRequired();
            entity.Property(e => e.IsSystem).HasDefaultValue(false);
        });

    }
}
