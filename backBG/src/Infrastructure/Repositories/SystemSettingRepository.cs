using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SystemSettingRepository> _logger;

        public SystemSettingRepository(ApplicationDbContext context, ILogger<SystemSettingRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SystemSetting?> GetByKeyAsync(string key)
        {
            return await _context.SystemSettings.FirstOrDefaultAsync(x => x.SettingKey == key);
        }

        public async Task<List<SystemSetting>> GetAllAsync()
        {
            return await _context.SystemSettings
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<string?> GetValueAsync(string key)
        {
            return await _context.SystemSettings
                .Where(s => s.SettingKey == key)
                .Select(s => s.SettingValue)
                .FirstOrDefaultAsync();
        }

        public async Task<T?> GetValueAsync<T>(string key) where T : struct
        {
            var result = await GetValueAsync(key);
            if (result == null)
                return null;

            return (T)Convert.ChangeType(result, typeof(T));
        }

        public async Task SetValueAsync(string key, string value, int? updatedBy = null)
        {
            var setting = await GetByKeyAsync(key);

            if (setting == null)
            {
                setting = new SystemSetting
                {
                    SettingKey = key,
                    SettingValue = value,
                    DataType = "string",
                    UpdatedBy = updatedBy
                };

                await _context.SystemSettings.AddAsync(setting);
            }
            else
            {
                setting.SettingValue = value;
                setting.UpdatedAt = DateTime.UtcNow;
                setting.UpdatedBy = updatedBy;

                _context.SystemSettings.Update(setting);
            }
        }

        public async Task<SystemSetting> AddAsync(SystemSetting setting)
        {
            await _context.SystemSettings.AddAsync(setting);
            return setting;
        }

        public Task UpdateAsync(SystemSetting setting)
        {
            setting.UpdatedAt = DateTime.UtcNow;
            _context.SystemSettings.Update(setting);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
