using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/messagelogs")]
    public class MessageLogsController : ControllerBase
    {
        private readonly IMessageLogService _messageLogService;

        public MessageLogsController(IMessageLogService messageLogService)
        {
            _messageLogService = messageLogService;
        }

        /// <summary>
        /// Endpoint to delete a message log by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = nameof(DeleteMessageLogById))]
        [ProducesResponseType(typeof(SuccessResponse<bool>), 200)]
        public async Task<IActionResult> DeleteMessageLogById(Guid id)
        {
            var response = await _messageLogService.DeleteMessageLogById(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a message log by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetMessageLogById))]
        [ProducesResponseType(typeof(SuccessResponse<MessageLogDto>), 200)]
        public async Task<IActionResult> GetMessageLogById(Guid id)
        {
            var response = await _messageLogService.GetMessageLogById(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a list of all message logs
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetMessageLogs))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<MessageLogDto>>), 200)]
        public async Task<IActionResult> GetMessageLogs([FromQuery] ResourceParameter parameter)
        {
            var response = await _messageLogService.GetMessageLogs(parameter, nameof(GetMessageLogs), Url);

            return Ok(response);
        }
    }
}
