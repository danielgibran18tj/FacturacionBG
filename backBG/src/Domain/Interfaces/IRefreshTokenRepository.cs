using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken> AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(int userId);
        Task RevokeAllUserTokensAsync(int userId);
        Task SaveChangesAsync();
    }
}
