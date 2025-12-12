using Application.DTOs.PaymentMethod;

namespace Application.Common.Interfaces
{
    public interface IPaymentMethodService
    {
        Task<List<PaymentMethodDto>> GetAllAsync();
    }
}
