namespace SafeSignal.Api.Services.Security;

/// <summary>
/// Configuración JWT enlazada desde la sección "Jwt" de appsettings.json.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "SafeSignal.Api";
    public string Audience { get; set; } = "SafeSignal.Clients";

    /// <summary>Clave simétrica HS256. Debe tener al menos 32 caracteres.</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 120;
}
