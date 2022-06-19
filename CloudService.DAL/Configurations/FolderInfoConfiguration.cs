using CloudService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudService.DAL.Configurations
{
    public class FolderInfoConfiguration : IEntityTypeConfiguration<FolderInfo>
    {
        public void Configure(EntityTypeBuilder<FolderInfo> builder)
        {
            builder.ToTable("FolderInfos");

            builder.HasKey(e => e.Id);
        }
    }
}
