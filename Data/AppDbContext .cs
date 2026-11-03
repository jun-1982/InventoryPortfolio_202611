using InventoryPortfolio.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace InventoryPortfolio.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            // テーブル名と在庫数の制約
            entity.ToTable("Products", table =>
            {
                table.HasCheckConstraint(
                    "CK_Products_StockQuantity",
                    "[StockQuantity] >= 0");
            });

            // 主キー
            entity.HasKey(p => p.Id);

            // 商品名
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 在庫数量
            entity.Property(p => p.StockQuantity)
                .IsRequired();
        });
        // 受注テーブル
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders", table =>
            {
                table.HasCheckConstraint(
                    "CK_Orders_Quantity",
                    "[Quantity] > 0");
            });

            entity.HasKey(o => o.Id);

            entity.Property(o => o.Quantity)
                .IsRequired();

            entity.Property(o => o.IsCancelled)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(o => o.CreatedAt)
                .IsRequired();

            // ProductsとOrdersの1対多の関係
            entity.HasOne(o => o.Product)
                .WithMany()
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }
}