using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Application.Services;

public class UserService(
    IRepository<Employee> employeeRepository,
    IRepository<Role> roleRepository
) : IUserService
{
    public async Task<List<Employee>> Get(CancellationToken ct)
    {
        var employees = await employeeRepository.GetAll(ct);
        var employeesModels = employees.ToList();

        return employeesModels;
    }

    public Task<Employee?> GetById(Guid id, CancellationToken ct)
    {
        return employeeRepository.GetById(id, ct);
    }

}
