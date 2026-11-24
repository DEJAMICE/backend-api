using System.Security.Claims;
using Microsoft.FeatureManagement.FeatureFilters;

namespace SafeSignal.Api.Services.Experiments;

/// <summary>
/// Resuelve el contexto de targeting (usuario/visitante y grupos) para asignación estable entre plataformas.
/// </summary>
public class SecuraNetTargetingContextAccessor : ITargetingContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SecuraNetTargetingContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ValueTask<TargetingContext> GetContextAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            return new ValueTask<TargetingContext>(new TargetingContext
            {
                UserId = "anonymous",
                Groups = Array.Empty<string>()
            });
        }

        var user = context.User;
        var authenticatedId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? user.FindFirstValue("sub");
        var email = user.FindFirstValue(ClaimTypes.Email)
                    ?? user.FindFirstValue("email");

        var clientId = context.Request.Headers["X-Client-Id"].FirstOrDefault()
                       ?? context.Request.Query["clientId"].FirstOrDefault();

        var targetingUserId = !string.IsNullOrWhiteSpace(authenticatedId)
            ? authenticatedId
            : (!string.IsNullOrWhiteSpace(clientId) ? clientId : "anonymous");

        var groups = new List<string>();
        if (!string.IsNullOrWhiteSpace(authenticatedId))
        {
            groups.Add("authenticated");
        }
        else
        {
            groups.Add("visitor");
        }

        if (!string.IsNullOrWhiteSpace(email) &&
            (email.EndsWith("@securanet.pe", StringComparison.OrdinalIgnoreCase) ||
             email.Equals("demo@safesignal.pe", StringComparison.OrdinalIgnoreCase)))
        {
            groups.Add("equipo-interno");
        }

        return new ValueTask<TargetingContext>(new TargetingContext
        {
            UserId = targetingUserId,
            Groups = groups
        });
    }
}
