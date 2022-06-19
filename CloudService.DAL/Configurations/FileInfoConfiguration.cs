using CloudService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudService.DAL.Configurations
{
    public class FileInfoConfiguration : IEntityTypeConfiguration<FileInfo>
    {
        public void Configure(EntityTypeBuilder<FileInfo> builder)
        {
            builder.ToTable("FileInfos");

            builder.HasKey(e => e.Id);
        }
    }
}
