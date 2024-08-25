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
    //[Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/wausers")]
    public class WhatsappUsersController : ControllerBase
    {
        private readonly IWhatsappUserService _whatsappUsersService;
        private readonly IMessageLogService _messageLogService;

        public WhatsappUsersController(IWhatsappUserService whatsappUsersService, IMessageLogService messageLogService)
        {
            _whatsappUsersService = whatsappUsersService;
            _messageLogService = messageLogService;
        }


        /// <summary>
        /// Endpoint to get a whatsapp user by whatsapp id
        /// </summary>
        /// <param name="waId"></param>
        /// <returns></returns>
        [HttpGet("{waId}", Name = nameof(GetWhatsappUserByWaId))]
        [ProducesResponseType(typeof(SuccessResponse<WhatsappUserDto>), 200)]
        public async Task<IActionResult> GetWhatsappUserByWaId(string waId)
        {
            var response = await _whatsappUsersService.GetWhatsappUserByWaId(waId);

            return Ok(response);
        }

        

        /// <summary>
        /// Endpoint to get a list of whatsapp users
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetWhatsappUsers))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<WhatsappUserDto>>), 200)]
        public async Task<IActionResult> GetWhatsappUsers([FromQuery] ResourceParameter parameter)
        {
            var response = await _whatsappUsersService.GetWhatsappUsers(parameter, nameof(WhatsappUserDto), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a list of message logs by the whatsapp id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("{id}/message-logs", Name = nameof(GetMessageLogsByWhatsappId))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<MessageLogDto>>), 200)]
        public async Task<IActionResult> GetMessageLogsByWhatsappId([FromRoute] Guid id,  [FromQuery] ResourceParameter parameter)
        {
            var response = await _messageLogService.GetMessageLogsByWaId(id, parameter, nameof(MessageLogDto), Url);

            return Ok(response);
        }


        /// <summary>
        /// Endpoint to get a list of message logs by the whatsapp phone number
        /// </summary>
        /// <param name="waId"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("{waId}/message-log", Name = nameof(GetMessageLogsByWhatsappPhoneNumber))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<MessageLogDto>>), 200)]
        public async Task<IActionResult> GetMessageLogsByWhatsappPhoneNumber([FromRoute] string waId, [FromQuery] ResourceParameter parameter)
        {
            var response = await _messageLogService.GetMessageLogsByPhoneNumber(waId, parameter, nameof(MessageLogDto), Url);

            return Ok(response);
        }
    }
}
