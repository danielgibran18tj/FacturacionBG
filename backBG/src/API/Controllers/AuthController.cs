using Application.Common.Interfaces;
using Application.DTOs;
using Application.DTOs.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IValidator<RegisterRequestDto> _registerValidator;
        private readonly IValidator<RefreshTokenRequestDto> _refreshTokenValidator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IValidator<LoginRequestDto> loginValidator,
            IValidator<RegisterRequestDto> registerValidator,
            IValidator<RefreshTokenRequestDto> refreshTokenValidator,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _refreshTokenValidator = refreshTokenValidator;
            _logger = logger;
        }

        // Iniciar sesión
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("Intento de login para usuario: {Username}", request.Username);

            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var result = await _authService.LoginAsync(request);

            _logger.LogInformation("Login exitoso para usuario: {Username}", request.Username);

            return Ok(new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Login exitoso",
                Data = result
            });
        }

        // Registrar nuevo usuario
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] RegisterRequestDto request)
        {
            _logger.LogInformation("Intento de registro para usuario: {Username}", request.Username);

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
                
            var validationResult = await _registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var result = await _authService.RegisterAsync(request);

            _logger.LogInformation("Registro exitoso para usuario: {Username}", request.Username);

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
                Data = result
            });
        }

        // Refrescar token de acceso
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            _logger.LogInformation("Intento de refresh token");

            var validationResult = await _refreshTokenValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var result = await _authService.RefreshTokenAsync(request);

            _logger.LogInformation("Refresh token exitoso");

            return Ok(new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Token refrescado exitosamente",
                Data = result
            });
        }

        // Cerrar sesión (revocar token)
        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] RefreshTokenRequestDto request)
        {
            var userClaims = User.Claims;
            var userRol = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            _logger.LogInformation("Intento de logout");

            var result = await _authService.RevokeTokenAsync(request.RefreshToken);

            _logger.LogInformation("Logout exitoso");

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Sesión cerrada exitosamente",
                Data = result
            });
        }

        // Verificar estado y datos del token
        [HttpGet("me")]
        [Authorize]
        public ActionResult<ApiResponse<CurrentUserDto>> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.Identity?.Name;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = User.FindFirst("firstName")?.Value;
            var lastName = User.FindFirst("lastName")?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            var dto = new CurrentUserDto
            {
                UserId = userId,
                Username = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Roles = roles
            };

            return Ok(new ApiResponse<CurrentUserDto>
            {
                Success = true,
                Message = "Token válido",
                Data = dto
            });
        }
    }
}
