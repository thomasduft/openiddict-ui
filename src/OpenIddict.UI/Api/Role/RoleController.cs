using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace tomware.OpenIddict.UI
{
  [Route("api/roles")]
  [Authorize(Policies.ADMIN_POLICY)]
  public class RoleController : ControllerBase
  {
    private readonly IRoleService service;

    public RoleController(IRoleService service)
    {
      this.service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoleViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRolesAsync()
    {
      var result = await this.service.GetRolesAsync();

      return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoleViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(string id)
    {
      if (id == null) return BadRequest();

      var result = await this.service.GetAsync(id);
      if (result == null) return NotFound();

      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] RoleViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await this.service.CreateAsync(model);

      return Created($"api/role/{result}", result); // this.Json(result)
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync([FromBody] RoleViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      await this.service.UpdateAsync(model);

      return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
      if (id == null) return BadRequest();

      await this.service.DeleteAsync(id);

      return NoContent();
    }
  }
}
