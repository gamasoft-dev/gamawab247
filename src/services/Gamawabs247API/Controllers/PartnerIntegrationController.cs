using Application.Helpers;
using Application.Services.Implementations;
using System.Threading.Tasks;
using System;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Net;
using Application.DTOs.PartnerContentDtos;

namespace Gamawabs247API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/partner-integration")]
    public class PartnerIntegrationController : ControllerBase
    {
        private readonly IPartnerContentIntegrationDetailsService _partnerIntegrationService;
        private readonly ILogger<PartnerIntegrationController> _logger;
        public PartnerIntegrationController(IPartnerContentIntegrationDetailsService partnerIntegrationService)
        {
            _partnerIntegrationService = partnerIntegrationService;
        }

        [HttpGet(Name = nameof(GetAllPartnerIntegrations))]
        [ProducesResponseType(typeof(IEnumerable<PartnerContentIntegrationDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetAllPartnerIntegrations([FromQuery] ResourceParameter parameter)
        {
            try
            {
                return Ok(await _partnerIntegrationService.GetAll(parameter, nameof(GetAllPartnerIntegrations), Url));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        [HttpGet("{id}", Name = nameof(GetPartnerIntegrationById))]
        [ProducesResponseType(typeof(SuccessResponse<PartnerDto>), 200)]
        public async Task<IActionResult> GetPartnerIntegrationById(Guid id)
        {
            try
            {
                return Ok(await _partnerIntegrationService.GetById(id));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<PartnerContentIntegrationDto>), (int)HttpStatusCode.Created)]
        [Authorize]//for admin usage
        public async Task<IActionResult> CreatePartnerIntegration([FromBody] CreatePartnerContentIntegrationDto dto)
        {
            try
            {
                var result = await _partnerIntegrationService.Create(dto);
                return CreatedAtAction(nameof(GetPartnerIntegrationById), new { id = result.Data.Id }, dto);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<PartnerContentIntegrationDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(SuccessResponse<PartnerContentIntegrationDto>), (int)HttpStatusCode.BadRequest)]
        [Authorize]//for admin usage
        public async Task<IActionResult> Put([FromQuery] Guid id, [FromBody] UpdatePartnerContentIntegrationDto dto)
        {
            try
            {
                var update = await _partnerIntegrationService.Update(id,dto);
                return Ok(update);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<bool>), (int)HttpStatusCode.OK)]
        [Authorize]//for admin usage
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _partnerIntegrationService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }
    }
}
