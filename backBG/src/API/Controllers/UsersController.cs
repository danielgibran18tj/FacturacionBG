using Application.Common.Interfaces;
using Application.DTOs;
using Application.DTOs.Auth;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Usuario no encontrado. Id: {UserId}", id);
                return NotFound(ex.Message);
            }
        }

        // GET: api/users/by-username/john
        [HttpGet("by-username/{username}")]
        public async Task<ActionResult<User>> GetByUsername(string username)
        {
            try
            {
                var user = await _userService.GetByUsernameAsync(username);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Usuario no encontrado. Username: {Username}", username);
                return NotFound(ex.Message);
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] RegisterRequestDto request)
        {
            try
            {
                var createdUser = await _userService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdUser.Id },
                    createdUser);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear usuario");
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Datos inválidos en creación de usuario");
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/users/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto user)
        {
            try
            {
                await _userService.UpdateAsync(id, user);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Usuario no encontrado para actualizar. Id: {UserId}", id);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflicto al actualizar usuario. Id: {UserId}", id);
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] PageRequestDto request, bool activeOnly = true)
        {
            var result = await _userService.GetPagedAsync(request, activeOnly);
            return Ok(result);
        }
    }
}
