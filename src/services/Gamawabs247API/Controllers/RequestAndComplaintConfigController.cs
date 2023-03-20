using Application.DTOs.RequestAndComplaintDtos;
using Application.Helpers;
using Application.Services.Implementations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gamawabs247API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/request-and-complaint-config")]
    public class RequestAndComplaintConfigController : ControllerBase
    {
        private readonly IRequestAndComplaintConfigService _requestAndComplaintConfigService;
        private readonly ILogger<RequestAndComplaintController> _logger;
        public RequestAndComplaintConfigController(IRequestAndComplaintConfigService requestAndComplaintConfigService, ILogger<RequestAndComplaintController> logger)
        {
            _requestAndComplaintConfigService = requestAndComplaintConfigService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<RequestAndComplaintConfigDto>), (int)HttpStatusCode.Created)]
        [Authorize]//for admin usage
        public async Task<IActionResult> CreateRequestOrComplaintConfig([FromBody] CreateRequestAndComplaintConfigDto model)
        {
            try
            {
                var result = await _requestAndComplaintConfigService.CreateRequestAndComplaintConfig(model);
                return CreatedAtAction(nameof(GetRequestOrComplaintConfig), new { id = result.Data.Id }, model);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<RequestAndComplaintConfigDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(SuccessResponse<RequestAndComplaintConfigDto>), (int)HttpStatusCode.BadRequest)]
        [Authorize]//for admin usage
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateRequestAndComplaintConfigDto model)
        {
            try
            {
                var requestOrComplaint = await _requestAndComplaintConfigService.UpdateRequestAndComplaintConfig(id, model);
                return Ok(requestOrComplaint);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<bool>), (int)HttpStatusCode.NoContent)]
        [Authorize]//for admin usage
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _requestAndComplaintConfigService.DeleteRequestAndComplaintConfig(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        [HttpGet(Name = nameof(GetAllRequestAndComplaintConfig))]
        [ProducesResponseType(typeof(IEnumerable<RequestAndComplaintConfigDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetAllRequestAndComplaintConfig([FromQuery] ResourceParameter parameter)
        {
            try
            {
                return Ok(await _requestAndComplaintConfigService.GetAllRequestAndComplaintConfig(parameter, nameof(GetAllRequestAndComplaintConfig), Url));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        [HttpGet("business/{id}", Name = nameof(GetAllRequestAndComplaintConfigByBusinessId))]
        [ProducesResponseType(typeof(IEnumerable<RequestAndComplaintConfigDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetAllRequestAndComplaintConfigByBusinessId([FromRoute] Guid id, [FromQuery] ResourceParameter parameter)
        {
            try
            {
                return Ok(await _requestAndComplaintConfigService.GetAllRequestAndComplaintConfigByBusinessId(id, parameter, nameof(GetAllRequestAndComplaintConfig), Url));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        [HttpGet("{id}", Name = nameof(GetRequestOrComplaintConfig))]
        [ProducesResponseType(typeof(SuccessResponse<RequestAndComplaintConfigDto>), 200)]
        public async Task<IActionResult> GetRequestOrComplaintConfig(Guid id)
        {
            try
            {
                return Ok(await _requestAndComplaintConfigService.GetRequestAndComplaintConfig(id));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }
    }
}
