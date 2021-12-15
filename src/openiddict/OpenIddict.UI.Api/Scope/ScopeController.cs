using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Api
{
  [Route("scopes")]
  public class ScopeController : OpenIddictApiController
  {
    private readonly IScopeApiService _service;

    public ScopeController(IScopeApiService service)
    {
      _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ScopeViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScopesAsync()
    {
      var result = await _service.GetScopesAsync();

      return Ok(result);
    }

    [HttpGet("names")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScopeNamesAsync()
    {
      var result = await _service.GetScopeNamesAsync();

      return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ScopeViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(string id)
    {
      if (id == null) return BadRequest();

      var result = await _service.GetAsync(id);
      if (result == null) return NotFound();

      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] ScopeViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _service.CreateAsync(model);

      return Created($"scopes/{result}", result);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync([FromBody] ScopeViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      await _service.UpdateAsync(model);

      return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
      if (id == null) return BadRequest();

      await _service.DeleteAsync(id);

      return NoContent();
    }
  }
}