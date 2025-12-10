using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ISystemSettingRepository _repository;
        private readonly ILogger<SystemSettingService> _logger;

        private readonly Dictionary<string, string> _cache = new();

        public SystemSettingService(
            ISystemSettingRepository repository,
            ILogger<SystemSettingService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        private async Task<string?> GetRawAsync(string key)
        {
            if (_cache.ContainsKey(key))
                return _cache[key];

            var value = await _repository.GetValueAsync(key);
            if (value != null)
                _cache[key] = value;

            return value;
        }

        // IVA como decimal
        public async Task<decimal> GetIvaAsync()
        {
            var value = await GetRawAsync("TAX_IVA_PERCENT");
            if (decimal.TryParse(value, out var iva))
                return iva;

            return 12; // Valor por defecto
        }

        // Símbolo de moneda
        public async Task<string> GetCurrencySymbolAsync()
        {
            return await GetRawAsync("CURRENCY_SYMBOL") ?? "$";
        }

        public async Task<List<SystemSetting>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task SetValueAsync(string key, string value, int? updatedBy = null)
        {
            _cache[key] = value;
            await _repository.SetValueAsync(key, value, updatedBy);
            await _repository.SaveChangesAsync();
        }
    }
}
