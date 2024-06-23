﻿using ShopAPI.Model.Products;
using ShopAPI.Model.Users;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ShopAPI.Data;

public class ShopDBContext : DbContext
{
    public ShopDBContext(DbContextOptions<ShopDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserAccount>()
               .HasIndex(u => u.Email)
               .IsUnique();

        builder.Entity<UserAccount>()
               .HasIndex(u => u.UserName)
               .IsUnique();

        builder.Entity<ProductRating>()
               .HasIndex(c => new { c.ProductId, c.VariantId })
               .IsUnique(true);

        builder.Entity<ProductVariant>()
               .Property(e => e.VariantProperties)
               .HasColumnType("text")
               .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<JObject>(v));
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductRating> ProductRatings { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<PackageContent> PackageContent { get; set; }
    public DbSet<PackageRating> PackageRatings { get; set; }
    public DbSet<JwtAccessTable> JwtAccessTables { get; set; }
}
