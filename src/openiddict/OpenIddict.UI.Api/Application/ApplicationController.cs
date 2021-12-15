using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Api
{
  [Route("application")]
  public class ApplicationController : OpenIddictApiController
  {
    private readonly IApplicationApiService _service;

    public ApplicationController(
      IApplicationApiService service
    )
    {
      _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ApplicationViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync()
    {
      var result = await _service.GetApplicationsAsync();

      return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApplicationViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(string id)
    {
      if (id == null) return BadRequest();

      var result = await _service.GetAsync(id);
      if (result == null) return NotFound();

      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] ApplicationViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _service.CreateAsync(model);

      return Created($"clients/{result}", result);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync([FromBody] ApplicationViewModel model)
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

    [HttpGet("options")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApplicationOptionsViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOptionsAsync()
    {
      var result = await _service.GetOptionsAsync();

      return Ok(result);
    }
  }
}
