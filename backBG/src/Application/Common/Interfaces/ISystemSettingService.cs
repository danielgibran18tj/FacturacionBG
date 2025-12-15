using Application.DTOs.SystemSettings;
using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface ISystemSettingService
    {
        Task<decimal> GetIvaAsync();
        Task<List<SystemSettingDto>> GetAllAsync();
        Task SetValueAsync(string key, string value, int? updatedBy = null);
    }
}
