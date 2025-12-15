using Application.Common.Interfaces;
using Application.DTOs.SystemSettings;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Application.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ISystemSettingRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<SystemSettingService> _logger;

        private readonly Dictionary<string, string> _cache = new();

        public SystemSettingService(
            ISystemSettingRepository repository,
            IMapper mapper,
            ILogger<SystemSettingService> logger)
        {
            _repository = repository;
            _mapper = mapper;
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
            var value = await GetRawAsync("IVA_PERCENTAGE");
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var iva))
            {
                return iva;
            }

            throw new FormatException($"El valor '{value}' configurado en 'IVA_PERCENTAGE' no es un número válido.");
        }

        public async Task<List<SystemSettingDto>> GetAllAsync()
        {
            var settings = await _repository.GetAllAsync();
            return _mapper.Map<List<SystemSettingDto>>(settings);
        }

        public async Task SetValueAsync(string key, string value, int? updatedBy = null)
        {
            _cache[key] = value;
            await _repository.SetValueAsync(key, value, updatedBy);
            await _repository.SaveChangesAsync();
        }

    }
}
