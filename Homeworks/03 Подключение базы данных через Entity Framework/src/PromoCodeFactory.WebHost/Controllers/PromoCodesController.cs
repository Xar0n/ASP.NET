using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Application.Abstractions;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Промокоды
/// </summary>
public class PromoCodesController(IPromoCodeService promoCodeService) : BaseController
{
    /// <summary>
    /// Получить все промокоды
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PromoCodeShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PromoCodeShortResponse>>> Get(CancellationToken ct)
    {
        var promoCodes = await promoCodeService.GetAll(ct);
        return Ok(promoCodes.Select(PromoCodesMapper.ToPromoCodeShortResponse));
    }

    /// <summary>
    /// Получить промокод по id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> GetById(Guid id, CancellationToken ct)
    {
        var promoCode = await promoCodeService.GetById(id, ct);
        if (promoCode is null)
            return NotFound();

        return Ok(PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
    }

    /// <summary>
    /// Создать промокод и выдать его клиентам с указанным предпочтением
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PromoCodeShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeShortResponse>> Create(PromoCodeCreateRequest request, CancellationToken ct)
    {
        try
        {
            var promoCode = await promoCodeService.Create(
                request.Code,
                request.ServiceInfo,
                request.PartnerName,
                request.BeginDate,
                request.EndDate,
                request.PartnerManagerId,
                request.PreferenceId,
                ct);

            return CreatedAtAction(
                nameof(GetById),
                new { id = promoCode.Id },
                PromoCodesMapper.ToPromoCodeShortResponse(promoCode));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = $"{ex.EntityType.Name} не найдено",
                Detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Применить промокод (отметить, что клиент использовал промокод)
    /// </summary>
    [HttpPost("{id:guid}/apply")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Apply(
        [FromRoute] Guid id,
        [FromBody] PromoCodeApplyRequest request,
        CancellationToken ct)
    {
        try
        {
            await promoCodeService.Apply(id, request.CustomerId, ct);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = $"{ex.EntityType.Name} not found",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Cannot apply promo code",
                Detail = ex.Message
            });
        }
    }
}
