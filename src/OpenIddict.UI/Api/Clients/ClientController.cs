using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace tomware.OpenIddict.UI
{
  [Route("api/clients")]
  [Authorize(Policies.ADMIN_POLICY)]
  public class ClientController : ControllerBase
  {
    private readonly IClientService service;

    public ClientController(
      IClientService service
    )
    {
      this.service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClientViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClientsAsync()
    {
      var result = await this.service.GetClientsAsync();

      return Ok(result);
    }

    [HttpGet("{clientId}")]
    [ProducesResponseType(typeof(ClientViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(string clientId)
    {
      if (clientId == null) return BadRequest();

      var result = await this.service.GetAsync(clientId);
      if (result == null) return NotFound();

      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody]ClientViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await this.service.CreateAsync(model);

      return Created($"api/clients/{result}", result); // this.Json(result)
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync([FromBody]ClientViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      await this.service.UpdateAsync(model);

      return NoContent();
    }

    [HttpDelete("{clientId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(string clientId)
    {
      if (clientId == null) return BadRequest();

      await this.service.DeleteAsync(clientId);

      return NoContent();
    }
  }
}
