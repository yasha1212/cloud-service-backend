using CloudService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudService.DAL.Configurations
{
    public class SharingRuleConfiguration : IEntityTypeConfiguration<SharingRule>
    {
        public void Configure(EntityTypeBuilder<SharingRule> builder)
        {
            builder.ToTable("SharingRules");

            builder.HasKey(e => new { e.FileId, e.UserId });
        }
    }
}
