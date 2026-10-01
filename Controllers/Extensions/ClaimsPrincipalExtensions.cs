using System.Security.Claims;

namespace SafeSignal.Api.Controllers.Extensions;

/// <summary>
/// Utilidades para leer la identidad del usuario autenticado desde el token JWT.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>Devuelve el Id del usuario autenticado (claim "sub"), o null si no es válido.</summary>
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
