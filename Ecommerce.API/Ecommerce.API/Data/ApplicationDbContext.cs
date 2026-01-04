
using Ecommerce.API.Data.Configurations;
using Ecommerce.API.Entities;
using Ecommerce.API.Entities.Event;
using Ecommerce.API.Entities.Products;
using Ecommerce.API.Entities.Shops;
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
        public DbSet<Shop> Shops { get; set; }           // Add Shops
        public DbSet<ShopTransaction> ShopTransactions { get; set; }  // Add Transactions
        public DbSet<Event> Events { get; set; }

        public DbSet<Media> Media { get; set; }
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

            // SHOP configuration (new)
            modelBuilder.Entity<Shop>(entity =>
            {
                // Unique email index (like mongoose unique)
                entity.HasIndex(s => s.Email).IsUnique();

                // Required fields validation
                entity.Property(s => s.Name).IsRequired();
                entity.Property(s => s.Address).IsRequired();
                entity.Property(s => s.PhoneNumber).IsRequired();
                entity.Property(s => s.AvatarPublicId).IsRequired();
                entity.Property(s => s.AvatarUrl).IsRequired();

                // Password min length
                entity.Property(s => s.PasswordHash);

                // Defaults
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(s => s.Role).HasDefaultValue("Seller");
                entity.Property(s => s.AvailableBalance).HasDefaultValue(0);

                // Precision for balance
                entity.Property(s => s.AvailableBalance).HasPrecision(18, 2);

                // One-to-many: Shop -> Transactions
                entity.HasMany(s => s.Transactions)
                      .WithOne(t => t.Shop)
                      .HasForeignKey(t => t.ShopId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Product foreign key (assuming ShopId in Product)
                entity.HasMany(s => s.Products)  // Navigation property if exists
                      .WithOne(p => p.Shop)      // Navigation property if exists
                      .HasForeignKey(p => p.ShopId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SHOP TRANSACTION configuration (new)
            modelBuilder.Entity<ShopTransaction>(entity =>
            {
                entity.Property(t => t.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Precision for amount
                entity.Property(t => t.Amount).HasPrecision(18, 2);
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

            // EVENT configuration
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Description)
                      .IsRequired();

                entity.Property(e => e.Category)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Status)
                      .HasDefaultValue("Running");

                entity.Property(e => e.OriginalPrice)
                      .HasPrecision(18, 2);

                entity.Property(e => e.DiscountPrice)
                      .HasPrecision(18, 2);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.ShopId);
                entity.HasIndex(e => e.Status);
            });

            // other entity configurations...

            modelBuilder.ApplyConfiguration(new MediaConfiguration());
            // or: modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        }
    }
}
