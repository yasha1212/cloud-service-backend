using CloudService.DAL.Extensions;
using CloudService.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudService.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
		public DbSet<Storage> Storages { get; set; }

		public DbSet<FileInfo> FileInfos { get; set; }

		public DbSet<FolderInfo> FolderInfos { get; set; }

		public DbSet<Extension> Extensions { get; set; }

		public DbSet<SharingRule> SharingRules { get; set; }

		public DbSet<RefreshToken> RefreshTokens { get; set; }


		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

			modelBuilder.Seed();
		}
	}
}
