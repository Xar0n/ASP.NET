using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.Helpers;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.PromoCodes;
using Soenneker.Utils.AutoBogus;
using System.Linq.Expressions;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.PromoCodes;

public class CreateTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodesRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly Mock<IRepository<CustomerPromoCode>> _customerPromoCodesRepositoryMock;
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferencesRepositoryMock;
    private readonly PromoCodesController _sut;

    public CreateTests()
    {
        _promoCodesRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        _customerPromoCodesRepositoryMock = new Mock<IRepository<CustomerPromoCode>>();
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _preferencesRepositoryMock = new Mock<IRepository<Preference>>();
        _sut = new PromoCodesController(_promoCodesRepositoryMock.Object, _customersRepositoryMock.Object,
            _customerPromoCodesRepositoryMock.Object, _partnersRepositoryMock.Object,
            _preferencesRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var promoCodeCreateRequest = CreatePromoCodeCreateRequest(partnerId, preferenceId);
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((Partner?)null));

        // Act
        var result = await _sut.Create(promoCodeCreateRequest, CancellationToken.None);

        // Assert
        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundObjectResult = (NotFoundObjectResult)result.Result;
        notFoundObjectResult.Value.Should().BeOfType<ProblemDetails>();
        var value = (ProblemDetails)notFoundObjectResult.Value;
        value.Title.Should().Be("Partner not found");
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var promoCodeCreateRequest = CreatePromoCodeCreateRequest(partnerId, preferenceId);
        var partner = TestDataFactory.CreatePartner(partnerId, true);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(partner)!);
        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((Preference?)null));

        // Act
        var result = await _sut.Create(promoCodeCreateRequest, CancellationToken.None);

        // Assert
        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundObjectResult = (NotFoundObjectResult)result.Result;
        notFoundObjectResult.Value.Should().BeOfType<ProblemDetails>();
        var value = (ProblemDetails)notFoundObjectResult.Value;
        value.Title.Should().Be("Preference not found");
    }

    [Fact]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partnerPromoCodeLimitId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var promoCodeCreateRequest = CreatePromoCodeCreateRequest(partnerId, preferenceId);
        var partner = TestDataFactory.CreatePartner(partnerId, true);
        var preference = CreatePreference(preferenceId);
        var partnerPromoCodeLimit = TestDataFactory.CreatePartnerPromoCodeLimit(
            partnerPromoCodeLimitId, canceledAt: DateTime.UtcNow);
        partner.PartnerLimits.Add(partnerPromoCodeLimit);
        var customer = CreateCustomer(customerId, preferences: new List<Preference> { preference });
        var customers = new List<Customer> { customer };

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference!);
        _customersRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>()))
            .ReturnsAsync(customers);

        // Act
        var result = await _sut.Create(promoCodeCreateRequest, CancellationToken.None);

        // Assert
        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<ObjectResult>();
        var unprocessableEntityObjectResult = (ObjectResult)result.Result;
        unprocessableEntityObjectResult.StatusCode.Should().Be(422);
        unprocessableEntityObjectResult.Value.Should().BeOfType<ProblemDetails>();
        var value = (ProblemDetails)unprocessableEntityObjectResult.Value;
        value.Title.Should().Be("No active limit");
    }

    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partnerPromoCodeLimitId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var promoCodeCreateRequest = CreatePromoCodeCreateRequest(partnerId, preferenceId);
        var partner = TestDataFactory.CreatePartner(partnerId, true);
        var preference = CreatePreference(preferenceId);
        var partnerPromoCodeLimit = TestDataFactory.CreatePartnerPromoCodeLimit(
            partnerPromoCodeLimitId, issuedCount: 200, limit: 100);
        partner.PartnerLimits.Add(partnerPromoCodeLimit);
        var customer = CreateCustomer(customerId, preferences: new List<Preference> { preference });
        var customers = new List<Customer> { customer };

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(partner)!);
        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(preference)!);
        _customersRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>()))
            .ReturnsAsync(customers);

        // Act
        var result = await _sut.Create(promoCodeCreateRequest, CancellationToken.None);

        // Assert
        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<ObjectResult>();
        var unprocessableEntityObjectResult = (ObjectResult)result.Result;
        unprocessableEntityObjectResult.StatusCode.Should().Be(422);
        unprocessableEntityObjectResult.Value.Should().BeOfType<ProblemDetails>();
        var value = (ProblemDetails)unprocessableEntityObjectResult.Value;
        value.Title.Should().Be("Limit exceeded");
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partnerPromoCodeLimitId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var promoCodeCreateRequest = CreatePromoCodeCreateRequest(partnerId, preferenceId);
        var partner = TestDataFactory.CreatePartner(partnerId, true);
        var preference = CreatePreference(preferenceId);
        var partnerPromoCodeLimit = TestDataFactory.CreatePartnerPromoCodeLimit(
            partnerPromoCodeLimitId, issuedCount: 100, limit: 200);
        partner.PartnerLimits.Add(partnerPromoCodeLimit);
        var customer = CreateCustomer(customerId, preferences: new List<Preference> { preference });
        var customers = new List<Customer> { customer };

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(partner)!);
        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(preference)!);
        _customersRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>()))
            .ReturnsAsync(customers);
        _promoCodesRepositoryMock
            .Setup(r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Create(promoCodeCreateRequest, CancellationToken.None);

        // Assert
        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)result.Result;
        createdAtActionResult.Value.Should().BeOfType<PromoCodeShortResponse>();
        var value = (PromoCodeShortResponse)createdAtActionResult.Value!;
        value.Code.Should().Be(promoCodeCreateRequest.Code);
        value.ServiceInfo.Should().Be(promoCodeCreateRequest.ServiceInfo);
        value.PartnerId.Should().Be(partnerId);
        value.PreferenceId.Should().Be(preferenceId);
        value.BeginDate.Should().Be(promoCodeCreateRequest.BeginDate.UtcDateTime);
        value.EndDate.Should().Be(promoCodeCreateRequest.EndDate.UtcDateTime);

        partnerPromoCodeLimit.IssuedCount.Should().Be(101);

        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.Is<PromoCode>(p =>
                p.Code == promoCodeCreateRequest.Code &&
                p.ServiceInfo == promoCodeCreateRequest.ServiceInfo &&
                p.Partner.Id == partnerId &&
                p.Preference.Id == preferenceId),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _partnersRepositoryMock.Verify(
            r => r.Update(partner, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static Preference CreatePreference(Guid preferenceId)
    {
        return new AutoFaker<Preference>()
            .RuleFor(r => r.Id, _ => preferenceId)
            .RuleFor(r => r.Name, f => f.Lorem.Sentence(1))
            .Generate();
    }

    private static Customer CreateCustomer(
        Guid customerId,
        ICollection<Preference>? preferences = null,
        ICollection<CustomerPromoCode>? customerPromoCodes = null)
    {
        return new AutoFaker<Customer>()
            .RuleFor(r => r.Id, _ => customerId)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.Email, f => f.Internet.Email())
            .RuleFor(r => r.Preferences, _ => preferences ?? new List<Preference>())
            .RuleFor(r => r.CustomerPromoCodes, _ => customerPromoCodes ?? new List<CustomerPromoCode>())
            .Generate();
    }

    private static PromoCodeCreateRequest CreatePromoCodeCreateRequest(Guid partnerId, Guid preferenceId)
    {
        return new AutoFaker<PromoCodeCreateRequest>()
            .RuleFor(r => r.Code, f => f.Random.Replace("???-###"))
            .RuleFor(r => r.ServiceInfo, f => f.Lorem.Sentence(3))
            .RuleFor(r => r.PartnerId, _ => partnerId)
            .RuleFor(r => r.PreferenceId, _ => preferenceId)
            .RuleFor(r => r.BeginDate, _ => DateTimeOffset.UtcNow.AddDays(-1))
            .RuleFor(r => r.EndDate, _ => DateTimeOffset.UtcNow.AddDays(30))
            .Generate();
    }
}
