using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface ISystemSettingService
    {
        Task<decimal> GetIvaAsync();
        Task<string> GetCurrencySymbolAsync();
        Task<List<SystemSetting>> GetAllAsync();
        Task SetValueAsync(string key, string value, int? updatedBy = null);
    }
}
