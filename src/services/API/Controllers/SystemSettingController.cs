using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/systemsettings")]
    [ApiController]
    public class SystemSettingController : ControllerBase
    {
        private readonly ISystemSettingsService _systemSettingsService;
        private readonly ILogger<SystemSettingController> _logger;

        public SystemSettingController(ISystemSettingsService systemSettingsService,
            ILogger<SystemSettingController> logger)
        {
            _systemSettingsService = systemSettingsService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<SystemSettingsDto>), 201)]
        [ProducesResponseType(typeof(SuccessResponse<SystemSettingsDto>), 500)]
        [Authorize]
        public async Task<IActionResult> CreateSystemSettings(CreateSystemSetttingsDto dto)
        {
            try
            {
                var result = await _systemSettingsService.CreateUpdateSystemSettings(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(SuccessResponse<SystemSettingsDto>), 200)]
        [ProducesResponseType(typeof(SuccessResponse<SystemSettingsDto>), 500)]
        [Authorize]
        public async Task<IActionResult> GetSystemSettings()
        {
            try
            {
                var get = await _systemSettingsService.GetSystemSettings();
               
                return Ok(get);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }
    }
}
