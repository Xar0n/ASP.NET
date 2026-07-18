using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Application.Abstractions;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.Core.Application.Services;

public class PreferenceService(IRepository<Preference> preferenceRepository) : IPreferenceService
{
    public Task<IReadOnlyCollection<Preference>> GetAll(CancellationToken ct)
    {
        return preferenceRepository.GetAll(false, ct);
    }
}
