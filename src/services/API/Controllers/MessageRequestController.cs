using Application.DTOs;
using Application.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/{version:apiVersion}/request")]    
    public class MessageRequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly ILogger<MessageRequestController> _logger;
        public MessageRequestController(IRequestService requestService, ILogger<MessageRequestController> logger)
        {
            _requestService = requestService;
            _logger = logger;
        }

        /// <summary>
        /// Handles messagw request for a business or industry
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequestDto),201)]
        [Authorize]
        public async Task<IActionResult> SaveRequestMessage(RequestDto dto)
        {
            try
            {
                var message = await _requestService.CreateRequestConversations(dto);
                return CreatedAtAction(null, message);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a business message request by business id
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        [HttpGet("business/{businessId}")]
        [ProducesResponseType(typeof(IEnumerable<GetRequestDto>),200)]
        [Authorize]
        public async Task<IActionResult> GetRequestByBusinessId(Guid businessId)
        {
            try
            {
                var get = await _requestService.GetAllByBusinessId(businessId);
                return Ok(get);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Gets business message request by industry Id
        /// </summary>
        /// <param name="industryId"></param>
        /// <returns></returns>
        [HttpGet("industry/{industryId}")]
        [ProducesResponseType(typeof(IEnumerable<GetRequestDto>), 200)]
        [Authorize]
        public async Task<IActionResult> GetRequestByIndustryId(Guid industryId)
        {
            try
            {
                var get = await _requestService.GetAllByIndustryId(industryId);
                return Ok(get);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the message by request id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetRequestDto), 200)]
        [Authorize]
        public async Task<IActionResult> GetRequestById(Guid id)
        {
            try
            {
                var get = await _requestService.GetByRequestId(id);
                return Ok(get);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Get all business message options
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetRequestDto), 200)]
        [Authorize]
        public async Task<IActionResult> GetAllRequestOptionsByRequestId(Guid id)
        {
            try
            {
                var get = await _requestService.GetAllRequestOptionsByRequestId(id);
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
