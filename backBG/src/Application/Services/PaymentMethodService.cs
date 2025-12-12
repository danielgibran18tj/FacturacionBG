using Application.Common.Interfaces;
using Application.DTOs.PaymentMethod;
using AutoMapper;
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

        public async Task<List<PaymentMethodDto>> GetAllAsync()
        {
            var paymentMethods = await _paymentMethodRepository.GetAllAsync();
            return _mapper.Map<List<PaymentMethodDto>>(paymentMethods);
        }

    }
}
