using System.Net;
using System.Threading.Tasks;
using Application.DTOs.InteractiveMesageDto;
using Application.DTOs.InteractiveMesageDto.CreateMessageRequestDto;
using Application.DTOs.OutboundMessageRequests;
using Application.Helpers;
using Application.Services.Interfaces.Interactive_Messages.Manager;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/interactive-message")]
    public class InteractiveMessageController : ControllerBase
    {
        private readonly IInteractiveMessageManager _interactiveMessageManager;
        public InteractiveMessageController(IInteractiveMessageManager interactiveMessageManager)
        {
            _interactiveMessageManager = interactiveMessageManager;
        }
        
        /// <summary>
        /// Endpoint to create an interactive message for reply button.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("reply-button")]
        [ProducesResponseType(typeof(SuccessResponse<GetReplyButtonMessageDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateReplyButtonMessage(CreateReplyButtonMessageDto model)
        {
            var replyButtonService = _interactiveMessageManager.GetInteractiveMessageProvider(EMessageType.Button.ToString());

            var response = await replyButtonService.CreateMessage(model.BusinessId, model);
            return CreatedAtAction(null, response);

        }

        /// <summary>
        /// Creates message for list message type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //    [HttpPost("list-message")]
        //    [ProducesResponseType(typeof(GetListMessageDto), (int)HttpStatusCode.Created)]
        //    public async Task<IActionResult> CreateListMessageType(CreateListMessageDtoObselete model)
        //    {
        //        var replyButtonService = _interactiveMessageManager.GetInteractiveMessageProvider(EMessageType.List.ToString());

        //        var response = await replyButtonService.CreateMessage(model.BusinessId, model);
        //        return CreatedAtAction(null, response);
        //    }
    }
}