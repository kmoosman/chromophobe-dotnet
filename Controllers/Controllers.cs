using Microsoft.AspNetCore.Mvc;
using Chromophobe.Helpers;

namespace Chromophobe.Controllers;

[ApiController]
[Route("/[controller]")]
public class InstitutionsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllInstitutions()
    {
        DatabaseHelper databaseHelper = new DatabaseHelper();
        var institutions = await databaseHelper.GetInstitutions();

        var json = JsonHelper.SerializeWithCamelCase(institutions);

        return Ok(json);
    }
}

[Route("/[controller]")]
public class ProvidersController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProviders()
    {
        DatabaseHelper databaseHelper = new DatabaseHelper();
        var providers = await databaseHelper.GetProviders();

        var json = JsonHelper.SerializeWithCamelCase(providers);

        return Ok(json);
    }
}
