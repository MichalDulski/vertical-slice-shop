using System.Reflection;
using API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options){}
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        // new BasketConfiguration().Configure(builder.Entity<Basket>());
        // new ProductConfiguration().Configure(builder.Entity<Product>());
    }

    public DbSet<Basket> Baskets { get; set; }
    public DbSet<Product> Products { get; set; }
}