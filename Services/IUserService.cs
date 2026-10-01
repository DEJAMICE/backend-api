using SafeSignal.Api.Domain.Entities;
using SafeSignal.Api.DTOs.Auth;
using SafeSignal.Api.DTOs.Users;

namespace SafeSignal.Api.Services;

public enum ChangePasswordResult
{
    Success,
    UserNotFound,
    InvalidCurrentPassword
}

/// <summary>
/// Contrato del Servicio de Usuarios y Perfiles.
/// </summary>
public interface IUserService
{
    /// <summary>Crea un usuario nuevo. Lanza DuplicateResourceException si el correo ya existe.</summary>
    Task<User> CreateAsync(RegisterRequest request);
    Task<User?> GetByEmailAsync(string email);
    Task<UserResponse?> GetProfileAsync(Guid id);
    Task<UserResponse?> UpdateProfileAsync(Guid id, UpdateUserRequest request);
    Task<ChangePasswordResult> ChangePasswordAsync(Guid id, ChangePasswordRequest request);
    Task<UserResponse?> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionRequest request);
    Task<bool> DeleteAsync(Guid id);
}
