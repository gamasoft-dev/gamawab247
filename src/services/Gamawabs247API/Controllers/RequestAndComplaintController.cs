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
    [Route("api/v{version:apiVersion}/request-and-complaint")]
    public class RequestAndComplaintController : ControllerBase
    {
        private readonly IRequestAndComplaintService _requestAndComplaintService;
        private readonly ILogger<PartnerController> _logger;
        public RequestAndComplaintController(IRequestAndComplaintService requestAndComplaintService)
        {
            _requestAndComplaintService = requestAndComplaintService;
        }

       
        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<RequestAndComplaintDto>), (int)HttpStatusCode.Created)]
        [Authorize]//for admin usage
        public async Task<IActionResult> CreateRequestOrComplaint([FromBody] CreateRequestAndComplaintDto model)
        {
            try
            {
                var result = await _requestAndComplaintService.CreateRequestAndComplaint(model);
                return CreatedAtAction(nameof(GetRequestOrComplaint), new { id = result.Data.Id }, model);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<RequestAndComplaintDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(SuccessResponse<RequestAndComplaintDto>), (int)HttpStatusCode.BadRequest)]
        [Authorize]//for admin usage
        public async Task<IActionResult> Put([FromQuery] Guid id, [FromBody] UpdateRequestAndComplaintDto model)
        {
            try
            {
                var requestOrComplaint = await _requestAndComplaintService.UpdateRequestAndComplaint(id,model);
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
                await _requestAndComplaintService.DeleteRequestAndComplaint(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        [HttpGet(Name = nameof(GetAllRequestAndComplaint))]
        [ProducesResponseType(typeof(IEnumerable<RequestAndComplaintDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetAllRequestAndComplaint([FromQuery] ResourceParameter parameter)
        {
            try
            {
                return Ok(await _requestAndComplaintService.GetAllRequestAndComplaint(parameter, nameof(GetAllRequestAndComplaint), Url));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        [HttpGet("business/{id}",Name = nameof(GetAllRequestAndComplaintByBusinessId))]
        [ProducesResponseType(typeof(IEnumerable<RequestAndComplaintDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetAllRequestAndComplaintByBusinessId([FromRoute] Guid businessId,[FromQuery] ResourceParameter parameter)
        {
            try
            {
                return Ok(await _requestAndComplaintService.GetAllRequestAndComplaintByBusinessId(businessId, parameter, nameof(GetAllRequestAndComplaint), Url));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        [HttpGet("{id}", Name = nameof(GetRequestOrComplaint))]
        [ProducesResponseType(typeof(SuccessResponse<RequestAndComplaintDto>), 200)]
        public async Task<IActionResult> GetRequestOrComplaint(Guid id)
        {
            try
            {
                return Ok(await _requestAndComplaintService.GetRequestAndComplaint(id));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

        [HttpGet("ticket/{id}", Name = nameof(GetRequestOrComplaintByTicketId))]
        [ProducesResponseType(typeof(SuccessResponse<RequestAndComplaintDto>), 200)]
        public async Task<IActionResult> GetRequestOrComplaintByTicketId([FromRoute] string ticketId)
        {
            try
            {
                return Ok(await _requestAndComplaintService.GetRequestAndComplaintByTicketId(ticketId));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
        }

    }
}
