using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Settings;
using Application.DTOs.Auth;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IUserRepository userRepository,
            IUserService userService,
            IMapper mapper,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _mapper = mapper;
            _userService = userService;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
        }

        // LOGIN
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userService.GetByUsernameAsync(request.Username);
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

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles;

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = userDto
        };
        }

        // REFRESH TOKEN
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

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles;

            return new LoginResponseDto
            {
                AccessToken = newAccess,
                RefreshToken = newRefresh,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = userDto
        };
        }

        // LOGOUT
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

        // REGISTER
        public async Task<UserDto> RegisterAsync(RegisterRequestDto request)
        {
            return await _userService.CreateAsync(request);
        }

    }
}
