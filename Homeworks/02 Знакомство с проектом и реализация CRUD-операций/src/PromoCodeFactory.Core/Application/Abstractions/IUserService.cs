namespace PromoCodeFactory.Core.Application.Abstractions;

public interface IUserService
{
    Task<List<Employee>> Get(CancellationToken ct);
}
