using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ISystemSettingRepository
    {
        Task<SystemSetting?> GetByKeyAsync(string key);
        Task<List<SystemSetting>> GetAllAsync();
        Task<string?> GetValueAsync(string key);
        Task<T?> GetValueAsync<T>(string key) where T : struct;
        Task SetValueAsync(string key, string value, int? updatedBy = null);
        Task<SystemSetting> AddAsync(SystemSetting setting);
        Task UpdateAsync(SystemSetting setting);
        Task SaveChangesAsync();
    }
}
