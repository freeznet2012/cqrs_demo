using CqrsLearning.MediatR.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace CqrsLearning.MediatR.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);

            entity.Property(product => product.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(product => product.Price)
                .HasPrecision(18, 2);

            entity.Property(product => product.IsActive)
                .IsRequired();

            entity.Property(product => product.CreatedAtUtc)
                .IsRequired();
        });
    }
}
