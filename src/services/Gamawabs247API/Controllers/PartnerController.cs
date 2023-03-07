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
    [Route("api/v{version:apiVersion}/partner")]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;
        private readonly ILogger<PartnerController> _logger;
        public PartnerController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

       
        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<PartnerDto>), (int)HttpStatusCode.Created)]
        [Authorize]//for admin usage
        public async Task<IActionResult> CreatePartner([FromBody] CreatePartnerDto dto)
        {
            try
            {
                var result = await _partnerService.Create(dto);
                return CreatedAtAction(nameof(GetPartnerById), new { id = result.Data.Id }, dto);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<PartnerDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(SuccessResponse<PartnerDto>), (int)HttpStatusCode.BadRequest)]
        [Authorize]//for admin usage
        public async Task<IActionResult> Put([FromQuery] Guid id, [FromBody] UpdatePartnerDto dto)
        {
            try
            {
                var update = await _partnerService.Update(id,dto);
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
                await _partnerService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        [HttpGet(Name = nameof(GetAllPartners))]
        [ProducesResponseType(typeof(IEnumerable<PartnerDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetAllPartners([FromQuery] ResourceParameter parameter)
        {
            try
            {
                return Ok(await _partnerService.GetAll(parameter, nameof(GetAllPartners), Url));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        [HttpGet("{id}", Name = nameof(GetPartnerById))]
        [ProducesResponseType(typeof(SuccessResponse<PartnerDto>), 200)]
        public async Task<IActionResult> GetPartnerById(Guid id)
        {
            try
            {
                return Ok(await _partnerService.GetById(id));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

    }
}
