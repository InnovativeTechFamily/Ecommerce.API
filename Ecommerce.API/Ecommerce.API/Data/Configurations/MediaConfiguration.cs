using Ecommerce.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.API.Data.Configurations
{
    public class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id).IsRequired();
            builder.Property(m => m.PublicId).IsRequired();
            builder.Property(m => m.Url).IsRequired();
            builder.Property(m => m.Folder).IsRequired();
            builder.Property(m => m.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Composite index for EntityType + EntityId lookups
            builder.HasIndex(m => new { m.EntityType, m.EntityId });

            // Index for Folder lookups
            builder.HasIndex(m => m.Folder);
        }
    }
}
