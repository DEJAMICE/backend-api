using SafeSignal.Api.Domain.Entities;
using SafeSignal.Api.DTOs.Auth;
using SafeSignal.Api.Services.Security;

namespace SafeSignal.Api.Services;

/// <summary>
/// Implementación de la lógica de autenticación: registro, login y emisión de JWT.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _tokenService;

    public AuthService(IUserService userService, IPasswordHasher hasher, IJwtTokenService tokenService)
    {
        _userService = userService;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var user = await _userService.CreateAsync(request);
        return BuildResponse(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userService.GetByEmailAsync(request.Email);
        if (user == null || !_hasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        return BuildResponse(user);
    }

    private AuthResponse BuildResponse(User user)
    {
        var (token, expiresAt) = _tokenService.GenerateToken(user);
        return new AuthResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresAt = expiresAt,
            User = UserService.MapToResponse(user)
        };
    }
}
