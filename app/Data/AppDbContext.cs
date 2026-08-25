using Microsoft.EntityFrameworkCore;
using TawakalApi.app.Models.Entities;

namespace TawakalApi.app.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<PartnerEntity> PartnerEntities { get; set; }
    public DbSet<TransactionEntity> TransactionEntity { get; set; }
    public DbSet<PortalUserEntity> PortalUsers => Set<PortalUserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the relationship between PartnerEntity and PortalUserEntity
        modelBuilder.Entity<PartnerEntity>()
            .HasOne(p => p.CreatedByUser)
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

            
        // --- Configure Self-Referencing PortalUserEntity relationship ---
        modelBuilder.Entity<PortalUserEntity>()
            .HasOne(u => u.CreatedByUser)
            .WithMany() // Or .WithMany(u => u.CreatedUsers) if you want a collection of users created by this admin
            .HasForeignKey(u => u.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents deleting an admin who created other users
    }
}