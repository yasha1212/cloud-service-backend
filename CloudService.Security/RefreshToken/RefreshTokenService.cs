using CloudService.DAL;
using CloudService.Impl.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.Security.RefreshToken
{
    public class RefreshTokenService : BaseService, IRefreshTokenService
    {
        public RefreshTokenService(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task AddAsync(Entities.RefreshToken token)
        {
            token.Id = Guid.NewGuid().ToString();

            DbContext.RefreshTokens.Add(token);

            await DbContext.SaveChangesAsync();
        }

        public Entities.RefreshToken Find(string refreshToken)
        {
            return DbContext.RefreshTokens
                .AsNoTracking()
                .Include(e => e.User)
                .FirstOrDefault(e => e.Token == refreshToken);
        }

        public async Task UpdateAsync(Entities.RefreshToken token)
        {
            DbContext.RefreshTokens.Update(token);

            await DbContext.SaveChangesAsync();
        }

        public async Task RevokeAsync(string refreshToken)
        {
            var dbRefreshToken = Find(refreshToken);

            dbRefreshToken.IsRevoked = true;

            DbContext.RefreshTokens.Update(dbRefreshToken);

            await DbContext.SaveChangesAsync();
        }
    }
}
