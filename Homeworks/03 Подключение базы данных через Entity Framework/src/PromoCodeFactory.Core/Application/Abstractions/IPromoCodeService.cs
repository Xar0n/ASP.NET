using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.Core.Application.Abstractions;

public interface IPromoCodeService
{
    Task<IReadOnlyCollection<PromoCode>> GetAll(CancellationToken ct);

    Task<PromoCode?> GetById(Guid id, CancellationToken ct);

    Task<PromoCode> Create(
        string code,
        string serviceInfo,
        string partnerName,
        DateTimeOffset beginDate,
        DateTimeOffset endDate,
        Guid partnerManagerId,
        Guid preferenceId,
        CancellationToken ct);

    Task Apply(Guid promoCodeId, Guid customerId, CancellationToken ct);
}
