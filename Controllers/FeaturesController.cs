using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace SafeSignal.Api.Controllers;

/// <summary>
/// Expone los feature flags de experimentación evaluados para el usuario o visitante actual (TS01).
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/features")]
[Produces("application/json")]
public class FeaturesController : ControllerBase
{
    private readonly IFeatureManager _featureManager;

    public FeaturesController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    /// <summary>
    /// Obtiene el mapa de feature flags activos/inactivos evaluados para la sesión o visitante.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IDictionary<string, bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatures()
    {
        var flags = new Dictionary<string, bool>(StringComparer.Ordinal);
        await foreach (var name in _featureManager.GetFeatureNamesAsync())
        {
            flags[name] = await _featureManager.IsEnabledAsync(name);
        }

        return Ok(flags);
    }
}
