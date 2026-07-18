using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.Core.Application.Abstractions;

public interface ICustomerService
{
    Task<IReadOnlyCollection<Customer>> GetAll(CancellationToken ct);

    Task<Customer?> GetById(Guid id, CancellationToken ct);

    Task<IReadOnlyCollection<PromoCode>> GetPromoCodes(IEnumerable<Guid> promoCodeIds, CancellationToken ct);

    Task<Customer> Create(
        string firstName,
        string lastName,
        string email,
        Guid[] preferenceIds,
        CancellationToken ct);

    Task<Customer> Update(
        Guid id,
        string firstName,
        string lastName,
        string email,
        Guid[] preferenceIds,
        CancellationToken ct);

    Task Delete(Guid id, CancellationToken ct);
}
