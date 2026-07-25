using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.Helpers;

internal static class TestDataFactory
{
    public static Partner CreatePartner(
        Guid partnerId,
        bool isActive,
        ICollection<PartnerPromoCodeLimit>? partnerPromoCodeLimits = null)
    {
        var role = new AutoFaker<Role>()
            .RuleFor(r => r.Id, _ => Guid.NewGuid())
            .Generate();

        var employee = new AutoFaker<Employee>()
            .RuleFor(e => e.Id, _ => Guid.NewGuid())
            .RuleFor(e => e.Role, role)
            .Generate();

        return new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => isActive)
            .RuleFor(p => p.Manager, employee)
            .RuleFor(p => p.PartnerLimits, partnerPromoCodeLimits ?? new List<PartnerPromoCodeLimit>())
            .Generate();
    }

    public static PartnerPromoCodeLimit CreatePartnerPromoCodeLimit(
        Guid id,
        DateTimeOffset? canceledAt = null,
        DateTimeOffset? createdAt = null,
        DateTimeOffset? endAt = null,
        int? issuedCount = null,
        int? limit = null,
        Partner? partner = null)
    {
        var faker = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.Id, _ => id)
            .RuleFor(l => l.CanceledAt, _ => canceledAt)
            .RuleFor(l => l.CreatedAt, _ => createdAt ?? DateTimeOffset.UtcNow.AddDays(-1))
            .RuleFor(l => l.EndAt, _ => endAt ?? DateTimeOffset.UtcNow.AddDays(30))
            .RuleFor(l => l.IssuedCount, f => issuedCount ?? f.Random.Int(1, 100))
            .RuleFor(l => l.Limit, f => limit ?? f.Random.Int(101, 200));

        if (partner is not null)
            faker.RuleFor(l => l.Partner, _ => partner);

        return faker.Generate();
    }

    public static Partner CreatePartnerWithLimit(
        Guid partnerId,
        Guid limitId,
        bool isActive,
        DateTimeOffset? canceledAt = null)
    {
        var limits = new List<PartnerPromoCodeLimit>();
        var partner = CreatePartner(partnerId, isActive, limits);
        limits.Add(CreatePartnerPromoCodeLimit(limitId, canceledAt: canceledAt, partner: partner));
        return partner;
    }
}
