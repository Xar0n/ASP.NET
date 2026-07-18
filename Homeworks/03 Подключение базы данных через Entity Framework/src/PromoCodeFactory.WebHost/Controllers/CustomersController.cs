using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Application.Abstractions;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Customers;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Клиенты
/// </summary>
public class CustomersController(ICustomerService customerService) : BaseController
{
    /// <summary>
    /// Получить данные всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> Get(CancellationToken ct)
    {
        var customers = await customerService.GetAll(ct);
        return Ok(customers.Select(CustomersMapper.ToCustomerShortResponse));
    }

    /// <summary>
    /// Получить данные клиента по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {
        var customer = await customerService.GetById(id, ct);
        if (customer is null)
            return NotFound();

        var promoCodeIds = customer.CustomerPromoCodes.Select(cpc => cpc.PromoCodeId).Distinct();
        var promoCodes = await customerService.GetPromoCodes(promoCodeIds, ct);

        return Ok(CustomersMapper.ToCustomerResponse(customer, promoCodes));
    }

    /// <summary>
    /// Создать клиента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerShortResponse>> Create([FromBody] CustomerCreateRequest request, CancellationToken ct)
    {
        try
        {
            var customer = await customerService.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PreferenceIds,
                ct);

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id },
                CustomersMapper.ToCustomerShortResponse(customer));
        }
        catch (EntityNotFoundException ex)
        {
            if (ex.EntityType == typeof(Preference))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Предпочтение не найдено",
                    Detail = $"Предпочтение с Id {ex.EntityId} не найдено."
                });
            }
            else
                return NotFound();
        }
    }

    /// <summary>
    /// Обновить клиента
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerShortResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerShortResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] CustomerUpdateRequest request,
        CancellationToken ct)
    {
        try
        {
            var customer = await customerService.Update(
                id,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PreferenceIds,
                ct);

            return Ok(CustomersMapper.ToCustomerShortResponse(customer));
        }
        catch (EntityNotFoundException ex)
        {
            if (ex.EntityType == typeof(Preference))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Предпочтение не найдено",
                    Detail = $"Предпочтение с Id {ex.EntityId} не найдено."
                });
            }
            else
                return NotFound();
        }
    }

    /// <summary>
    /// Удалить клиента
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await customerService.Delete(id, ct);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}
