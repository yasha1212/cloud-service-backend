using CloudService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudService.DAL.Configurations
{
    public class ExtensionConfiguration : IEntityTypeConfiguration<Extension>
    {
        public void Configure(EntityTypeBuilder<Extension> builder)
        {
            builder.ToTable("Extensions");

            builder.HasKey(e => e.Id);
        }
    }
}
