namespace PromoCodeFactory.Core.Application.Abstractions;

public interface IRoleService
{
    Task<List<Role>> Get(CancellationToken ct);

    Task<Role?> GetById(Guid id, CancellationToken ct);
}
