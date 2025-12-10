using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPaymentMethodRepository
    {
        Task<PaymentMethod?> GetByIdAsync(int id);
        Task<List<PaymentMethod>> GetAllAsync(bool includeInactive = false);
        Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod);
        Task UpdateAsync(PaymentMethod paymentMethod);
        Task SaveChangesAsync();
    }
}
