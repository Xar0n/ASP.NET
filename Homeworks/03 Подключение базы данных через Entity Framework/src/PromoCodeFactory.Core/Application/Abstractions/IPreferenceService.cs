using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.Core.Application.Abstractions;

public interface IPreferenceService
{
    Task<IReadOnlyCollection<Preference>> GetAll(CancellationToken ct);
}
