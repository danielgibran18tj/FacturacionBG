using Application.Common.Interfaces;
using Application.DTOs.PaymentMethod;
using Application.DTOs.Product;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ILogger<PaymentMethodService> _logger;
        private readonly IMapper _mapper;

        public PaymentMethodService(
            IPaymentMethodRepository paymentMethodRepository,
            IMapper mapper,
            ILogger<PaymentMethodService> logger)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaymentMethodDto> GetByIdAsync(int id)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Metodo de pago no encontrado");

            return _mapper.Map<PaymentMethodDto>(paymentMethod);
        }

        public async Task<List<PaymentMethodDto>> GetAllAsync()
        {
            var paymentMethods = await _paymentMethodRepository.GetAllAsync();
            return _mapper.Map<List<PaymentMethodDto>>(paymentMethods);
        }

    }
}
