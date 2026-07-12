using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Application.Services;

public class RoleService(IRepository<Role> roleRepository) : IRoleService
{
    public async Task<List<Role>> Get(CancellationToken ct)
    {
        var roles = await roleRepository.GetAll(ct);
        return roles.ToList();
    }

    public Task<Role?> GetById(Guid id, CancellationToken ct)
    {
        return roleRepository.GetById(id, ct);
    }
}
