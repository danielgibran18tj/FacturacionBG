using Application.Common.Interfaces;
using Application.DTOs.SystemSettings;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/system-settings")]
    [Authorize]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingService _service;
        private readonly ILogger<SystemSettingsController> _logger;

        public SystemSettingsController(
            ISystemSettingService service,
            ILogger<SystemSettingsController> logger)
        {
            _service = service;
            _logger = logger;
        }


        /// Obtener todas las configuraciones del sistema
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<SystemSettingDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }


        /// Obtener IVA configurado
        [HttpGet("iva")]
        [AllowAnonymous]
        public async Task<ActionResult<decimal>> GetIva()
        {
            return Ok(await _service.GetIvaAsync());
        }


        /// Actualizar una configuración
        [HttpPut("{key}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(
            string key,
            [FromBody] UpdateSystemSettingDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Value))
                return BadRequest("El valor no puede estar vacío.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = int.TryParse(userIdClaim, out var id) ? id : null;

            await _service.SetValueAsync(key, dto.Value, userId);

            _logger.LogInformation("Configuración {Key} actualizada por usuario {UserId}", key, userId);

            return NoContent();
        }

    }
}
