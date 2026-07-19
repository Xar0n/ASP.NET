using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Application.Abstractions;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;

namespace PromoCodeFactory.Core.Application.Services;

public class PromoCodeService(
    IRepository<PromoCode> promoCodeRepository,
    IRepository<Customer> customerRepository,
    IRepository<CustomerPromoCode> customerPromoCodeRepository,
    IRepository<Employee> employeeRepository,
    IRepository<Preference> preferenceRepository
) : IPromoCodeService
{
    public Task<IReadOnlyCollection<PromoCode>> GetAll(CancellationToken ct)
    {
        return promoCodeRepository.GetAll(true, ct);
    }

    public Task<PromoCode?> GetById(Guid id, CancellationToken ct)
    {
        return promoCodeRepository.GetById(id, true, ct);
    }

    public async Task<PromoCode> Create(
        string code,
        string serviceInfo,
        string partnerName,
        DateTimeOffset beginDate,
        DateTimeOffset endDate,
        Guid partnerManagerId,
        Guid preferenceId,
        CancellationToken ct)
    {
        var partnerManager = await employeeRepository.GetById(partnerManagerId, false, ct);
        if (partnerManager is null)
            throw new EntityNotFoundException<Employee>(partnerManagerId);

        var preference = await preferenceRepository.GetById(preferenceId, false, ct);
        if (preference is null)
            throw new EntityNotFoundException<Preference>(preferenceId);

        var customersWithPreference = await customerRepository.GetWhere(
            c => c.Preferences.Any(p => p.Id == preferenceId),
            ct: ct);

        var promoCodeId = Guid.NewGuid();
        var promoCode = new PromoCode
        {
            Id = promoCodeId,
            Code = code,
            ServiceInfo = serviceInfo,
            PartnerName = partnerName,
            BeginDate = beginDate,
            EndDate = endDate,
            PartnerManager = partnerManager,
            Preference = preference,
            CustomerPromoCodes = customersWithPreference.Select(c => new CustomerPromoCode
            {
                Id = Guid.NewGuid(),
                CustomerId = c.Id,
                PromoCodeId = promoCodeId,
                CreatedAt = DateTimeOffset.UtcNow,
                AppliedAt = null
            }).ToList()
        };

        await promoCodeRepository.Add(promoCode, ct);
        return promoCode;
    }

    public async Task Apply(Guid promoCodeId, Guid customerId, CancellationToken ct)
    {
        var promoCode = await promoCodeRepository.GetById(promoCodeId, false, ct);
        if (promoCode is null)
            throw new EntityNotFoundException<PromoCode>(promoCodeId);

        var customerPromoCodes = await customerPromoCodeRepository.GetWhere(
            cpc => cpc.PromoCodeId == promoCodeId && cpc.CustomerId == customerId,
            false, ct);

        var customerPromoCode = customerPromoCodes.FirstOrDefault();
        if (customerPromoCode is null)
            throw new EntityNotFoundException<CustomerPromoCode>(customerId);

        if (customerPromoCode.AppliedAt is not null)
            throw new InvalidOperationException("Промокод уже был применен этим клиентом.");

        customerPromoCode.AppliedAt = DateTimeOffset.UtcNow;
        await customerPromoCodeRepository.Update(customerPromoCode, ct);
    }
}
