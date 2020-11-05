using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace tomware.OpenIddict.UI.Api
{
  [Route("api/clients")]
  [Authorize(Policies.ADMIN_POLICY)]
  public class ClientController : ControllerBase
  {
    private readonly IClientService _service;

    public ClientController(
      IClientService service
    )
    {
      _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClientViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClientsAsync()
    {
      var result = await _service.GetClientsAsync();

      return Ok(result);
    }

    [HttpGet("{clientId}")]
    [ProducesResponseType(typeof(ClientViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(string clientId)
    {
      if (clientId == null) return BadRequest();

      var result = await _service.GetAsync(clientId);
      if (result == null) return NotFound();

      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody]ClientViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _service.CreateAsync(model);

      return Created($"api/clients/{result}", result); // Json(result)
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync([FromBody]ClientViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      await _service.UpdateAsync(model);

      return NoContent();
    }

    [HttpDelete("{clientId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(string clientId)
    {
      if (clientId == null) return BadRequest();

      await _service.DeleteAsync(clientId);

      return NoContent();
    }
  }
}
