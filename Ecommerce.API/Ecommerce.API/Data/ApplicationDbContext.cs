using Ecommerce.API.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Ecommerce.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Avatar> Avatars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(u => u.Role).HasDefaultValue("user");
                entity.Property(u => u.IsActive).HasDefaultValue(false);
            });

            // Address configuration
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithMany(u => u.Addresses)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Avatar configuration (one-to-one with User)
            modelBuilder.Entity<Avatar>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithOne(u => u.Avatar)
                      .HasForeignKey<Avatar>(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
