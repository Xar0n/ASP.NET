namespace PromoCodeFactory.Core.Application.Abstractions;

public interface IUserService
{
    Task<List<Employee>> Get(CancellationToken ct);

    Task<Employee?> GetById(Guid id, CancellationToken ct);

    Task Create(Employee employee, CancellationToken ct);
}
