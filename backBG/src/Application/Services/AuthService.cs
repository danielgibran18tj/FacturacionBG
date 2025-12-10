using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Settings;
using Application.DTOs;
using Application.DTOs.Auth;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IRoleRepository roleRepository,
            IJwtService jwtService,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _roleRepository = roleRepository;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
        }

        // ------------------------------------------------------------
        // LOGIN
        // ------------------------------------------------------------
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            // Obtener roles del usuario
            var roles = user.UserRoles?.Select(r => r.Role.Name).ToList() ?? new();

            // Actualizar último login
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Generar tokens
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = user.ToDto(roles)
            };
        }

        // ------------------------------------------------------------
        // REFRESH TOKEN
        // ------------------------------------------------------------
        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var oldToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (oldToken == null || !oldToken.IsActive)
                throw new UnauthorizedAccessException("Refresh token inválido o expirado");

            var user = oldToken.User;
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Usuario inactivo");

            oldToken.IsRevoked = true;
            oldToken.RevokedAt = DateTime.UtcNow;

            // Generar nuevos tokens
            var roles = user.UserRoles?.Select(r => r.Role.Name).ToList() ?? new();

            var newAccess = _jwtService.GenerateAccessToken(user, roles);
            var newRefresh = _jwtService.GenerateRefreshToken();

            oldToken.ReplacedByToken = newRefresh;

            var newRefreshEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefresh,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            };

            await _refreshTokenRepository.UpdateAsync(oldToken);
            await _refreshTokenRepository.AddAsync(newRefreshEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = newAccess,
                RefreshToken = newRefresh,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = user.ToDto(roles)
            };
        }

        // ------------------------------------------------------------
        // LOGOUT
        // ------------------------------------------------------------
        public async Task<bool> RevokeTokenAsync(string token)
        {
            var rt = await _refreshTokenRepository.GetByTokenAsync(token);
            if (rt == null || rt.IsRevoked)
                return false;

            rt.IsRevoked = true;
            rt.RevokedAt = DateTime.UtcNow;

            await _refreshTokenRepository.UpdateAsync(rt);
            await _refreshTokenRepository.SaveChangesAsync();

            return true;
        }

        // ------------------------------------------------------------
        // REGISTER
        // ------------------------------------------------------------
        public async Task<UserDto> RegisterAsync(RegisterRequestDto request)
        {
            if (await _userRepository.ExistsByUsernameAsync(request.Username))
                throw new InvalidOperationException("El nombre de usuario ya está en uso");

            if (await _userRepository.ExistsByEmailAsync(request.Email))
                throw new InvalidOperationException("El email ya está registrado");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // --------------------------------------------------------
            // Asignar roles por defecto (ADMIN, VENDEDOR, CLIENTE)
            // --------------------------------------------------------
            var rolesToAssign = request.Roles?.ToList() ?? new List<string> { "User" };

            foreach (var roleName in rolesToAssign)
            {
                var role = await _roleRepository.GetByNameAsync(roleName);
                if (role != null)
                    await _roleRepository.AddUserRoleAsync(user.Id, role.Id);
            }
            await _roleRepository.SaveChangesAsync();

            var finalRoles = rolesToAssign;

            return user.ToDto(finalRoles);
        }
    }
}
