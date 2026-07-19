using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Application.Abstractions;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;

namespace PromoCodeFactory.Core.Application.Services;

public class CustomerService(
    IRepository<Customer> customerRepository,
    IRepository<Preference> preferenceRepository,
    IRepository<PromoCode> promoCodeRepository
) : ICustomerService
{
    public async Task<IReadOnlyCollection<Customer>> GetAll(CancellationToken ct)
    {
        return await customerRepository.GetAll(withIncludes: true, ct);
    }

    public Task<Customer?> GetById(Guid id, CancellationToken ct)
    {
        return customerRepository.GetById(id, withIncludes: true, ct);
    }

    public Task<IReadOnlyCollection<PromoCode>> GetPromoCodes(
        IEnumerable<Guid> promoCodeIds, 
        CancellationToken ct)
    {
        return promoCodeRepository.GetByRangeId(promoCodeIds, withIncludes: true, ct);
    }

    public async Task<Customer> Create(
        string firstName,
        string lastName,
        string email,
        Guid[] preferenceIds,
        CancellationToken ct)
    {
        var preferences = await preferenceRepository
            .GetByRangeId(preferenceIds, false, ct);

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Preferences = preferences.ToList()
        };

        await customerRepository.Add(customer, ct);
        return customer;
    }

    public async Task<Customer> Update(
        Guid id,
        string firstName,
        string lastName,
        string email,
        Guid[] preferenceIds,
        CancellationToken ct)
    {
        var customer = await customerRepository
            .GetById(id, true, ct);
        if (customer is null)
            throw new EntityNotFoundException<Customer>(id);

        var preferences = await preferenceRepository
            .GetByRangeId(preferenceIds, false, ct);

        customer.FirstName = firstName;
        customer.LastName = lastName;
        customer.Email = email;
        customer.Preferences = preferences.ToList();

        await customerRepository.Update(customer, ct);
        return customer;
    }

    public Task Delete(Guid id, CancellationToken ct)
    {
        return customerRepository.Delete(id, ct);
    }
}
