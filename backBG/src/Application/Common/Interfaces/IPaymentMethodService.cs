using Application.DTOs.PaymentMethod;

namespace Application.Common.Interfaces
{
    public interface IPaymentMethodService
    {
        Task<PaymentMethodDto> GetByIdAsync(int id);

        Task<List<PaymentMethodDto>> GetAllAsync();
    }
}
