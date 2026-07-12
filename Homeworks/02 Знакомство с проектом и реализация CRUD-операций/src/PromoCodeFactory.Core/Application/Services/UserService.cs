using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Exceptions;

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

    public Task Create(Employee employee, CancellationToken ct)
    {
        return employeeRepository.Add(employee, ct);
    }

    public async Task<Employee> Update(
        Guid id,
        string firstName,
        string lastName,
        string email,
        Guid roleId,
        CancellationToken ct)
    {
        var employee = await employeeRepository.GetById(id, ct);
        if (employee is null)
            throw new EntityNotFoundException<Employee>(id);

        var role = await roleRepository.GetById(roleId, ct);
        if (role is null)
            throw new EntityNotFoundException<Role>(roleId);

        employee.FirstName = firstName;
        employee.LastName = lastName;
        employee.Email = email;
        employee.Role = role;

        await employeeRepository.Update(employee, ct);
        return employee;
    }

    public Task Delete(Guid id, CancellationToken ct)
    {
        return employeeRepository.Delete(id, ct);
    }

}
