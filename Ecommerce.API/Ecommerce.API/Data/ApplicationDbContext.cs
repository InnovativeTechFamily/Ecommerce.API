
using Ecommerce.API.Entities.Products;
using Ecommerce.API.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Avatar> Avatars { get; set; }

		public DbSet<Product> Products { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(u => u.Role).HasDefaultValue("user");

                entity.HasMany(u => u.Addresses)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.Avatar)
                    .WithOne(a => a.User)
                    .HasForeignKey<Avatar>(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

           

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.Category);
                entity.HasIndex(p => p.ShopId);
                entity.HasIndex(p => p.Status);

                entity.Property(p => p.OriginalPrice)
                    .HasPrecision(18, 2);

                entity.Property(p => p.DiscountPrice)
                    .HasPrecision(18, 2);
				entity.Property(p => p.CreatedAt)
	                  .HasDefaultValueSql("GETUTCDATE()");
				entity.Property(p => p.UpdatedAt)
				  .HasDefaultValueSql("GETUTCDATE()");
			});
		}
    }
}
