using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Application.Abstractions;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Роли сотрудников
/// </summary>
public class RolesController(IRoleService roleService) : BaseController
{
    /// <summary>
    /// Получить все доступные роли сотрудников
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoleResponse>>> Get(CancellationToken ct)
    {
        var roles = await roleService.Get(ct);

        var rolesModels = roles.Select(Mapper.ToRoleResponse).ToList();

        return Ok(rolesModels);
    }
}
