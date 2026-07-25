using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.Partners;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetLimitTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<PartnerPromoCodeLimit>> _partnerLimitsRepositoryMock;
    private readonly PartnersController _sut;

    public SetLimitTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _partnerLimitsRepositoryMock = new Mock<IRepository<PartnerPromoCodeLimit>>();
        _sut = new PartnersController(_partnersRepositoryMock.Object, _partnerLimitsRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = new PartnerPromoCodeLimitCreateRequest(EndAt: DateTime.UtcNow.AddDays(2), Limit: 5);
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ActionResult<PartnerPromoCodeLimitResponse>>();
        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value;
        problemDetails.Title.Should().Be("Partner not found");
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = new PartnerPromoCodeLimitCreateRequest(EndAt: DateTime.UtcNow.AddDays(2), Limit: 5);
        var partner = CreatePartner(partnerId, false);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Act
        var actionResult = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOfType<ActionResult<PartnerPromoCodeLimitResponse>>();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        var unprocessableEntityObjectResult = (UnprocessableEntityObjectResult)actionResult.Result;
        unprocessableEntityObjectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)unprocessableEntityObjectResult.Value;
        problemDetails.Title.Should().Be("Partner blocked");
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partnerPromoCodeLimitId = Guid.NewGuid();
        var request = new PartnerPromoCodeLimitCreateRequest(EndAt: DateTime.UtcNow.AddDays(2), Limit: 5);
        var partner = CreatePartner(partnerId, true);
        var partnerPromoCodeLimit = CreatePartnerPromoCodeLimit(partnerPromoCodeLimitId, canceledAt: DateTime.UtcNow);
        partner.PartnerLimits.Add(partnerPromoCodeLimit);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var actionResult = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOfType<ActionResult<PartnerPromoCodeLimitResponse>>();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)actionResult.Result;
        createdAtActionResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>();
        var partnerPromoCodeLimitResponse = (PartnerPromoCodeLimitResponse)createdAtActionResult.Value;
        partnerPromoCodeLimitResponse.EndAt.Should().Be(request.EndAt);
        partnerPromoCodeLimitResponse.Limit.Should().Be(request.Limit);
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var activeLimitId = Guid.NewGuid();
        var request = new PartnerPromoCodeLimitCreateRequest(EndAt: DateTime.UtcNow.AddDays(2), Limit: 5);
        var partner = CreatePartner(partnerId, true);
        var activeLimit = CreatePartnerPromoCodeLimit(activeLimitId, canceledAt: null);
        partner.PartnerLimits.Add(activeLimit);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var actionResult = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOfType<ActionResult<PartnerPromoCodeLimitResponse>>();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<CreatedAtActionResult>();
        activeLimit.CanceledAt.Should().NotBeNull();
        _partnersRepositoryMock.Verify(
            r => r.Update(partner, It.IsAny<CancellationToken>()),
            Times.Once);
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var activeLimitId = Guid.NewGuid();
        var request = new PartnerPromoCodeLimitCreateRequest(EndAt: DateTime.UtcNow.AddDays(2), Limit: 5);
        var partner = CreatePartner(partnerId, true);
        var activeLimit = CreatePartnerPromoCodeLimit(activeLimitId, canceledAt: null);
        partner.PartnerLimits.Add(activeLimit);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Partner>(partnerId));

        // Act
        var actionResult = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        actionResult.Should().BeOfType<ActionResult<PartnerPromoCodeLimitResponse>>();
        actionResult.Result.Should().NotBeNull();
        actionResult.Result.Should().BeOfType<NotFoundResult>();
    }


    private static Partner CreatePartner(
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

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => isActive)
            .RuleFor(p => p.Manager, employee)
            .RuleFor(p => p.PartnerLimits, partnerPromoCodeLimits == null ?
                new List<PartnerPromoCodeLimit>() : partnerPromoCodeLimits)
            .Generate();

        return partner;
    }

    public static PartnerPromoCodeLimit CreatePartnerPromoCodeLimit(
        Guid id,
        DateTimeOffset? canceledAt = null,
        DateTimeOffset? createdAt = null,
        DateTimeOffset? endAt = null,
        int? issuedCount = null,
        int? limit = null)
    {
        var partnerPromoCodeLimit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.Id, _ => id)
            .RuleFor(l => l.CanceledAt, _ => canceledAt)
            .RuleFor(l => l.CreatedAt, _ => createdAt == null ?
                DateTimeOffset.UtcNow.AddDays(-1) : createdAt)
            .RuleFor(l => l.EndAt, _ => endAt == null ?
                DateTimeOffset.UtcNow.AddDays(30) : endAt)
            .RuleFor(l => l.IssuedCount, f => issuedCount  == null ?
                f.Random.Int(1, 100) : issuedCount)
            .RuleFor(l => l.Limit, f => limit == null ?
                f.Random.Int(101, 200) : limit)
            .Generate();

        return partnerPromoCodeLimit;
    }
}
