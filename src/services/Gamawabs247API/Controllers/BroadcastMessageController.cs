using System;
using System.Net;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gamawabs247API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/messagebroadcast")]
  
    public class BroadcastMessageController:ControllerBase
    {
        private readonly IBroadcastMessageService _broadcastMessage;
        private readonly ILogger<BroadcastMessageController> _logger;
        public BroadcastMessageController(IBroadcastMessageService broadcastMessage, ILogger<BroadcastMessageController> logger)
        {
            _broadcastMessage = broadcastMessage;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<BroadcastMessageDto>), (int)HttpStatusCode.Created)]

        public async Task<IActionResult> CreateBroadCastMessage(CreateBroadcastMessageDto model)
        {
            try
            {
                var result = await _broadcastMessage.CreateBroadcastMessage(model);
                return Ok(result);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<BroadcastMessageDto>), (int)HttpStatusCode.NoContent)]

        public async Task<IActionResult> UpdateBroadcastMessage([FromQuery] Guid id, [FromBody] UpdateBroadcastMessageDto model)
        {
            try
            {
                var result = await _broadcastMessage.UpdateBroadcastMessage(id, model);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<BroadcastMessageDto>), (int)HttpStatusCode.NoContent)]

        public async Task<IActionResult> DeleteBroadcastMessage([FromQuery] Guid id)
        {
            var result = await _broadcastMessage.DeleteBroadcastMessage(id);
            return NoContent();
        }

        [HttpGet(Name = nameof(GetAllBroadCastMessage))]
        [ProducesResponseType(typeof(SuccessResponse<BroadcastMessageDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllBroadCastMessage([FromQuery] ResourceParameter parameter)
        {
            try
            {
                var result = await _broadcastMessage
                .GetAllBroadcastMessage(parameter, nameof(GetAllBroadCastMessage), Url);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }


        [HttpGet("{id}", Name = nameof(GetBroadcastMessageById))]
        [ProducesResponseType(typeof(SuccessResponse<BroadcastMessageDto>), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> GetBroadcastMessageById([FromQuery] Guid id)
        {
            try
            {
                var result = await _broadcastMessage.GetBroadcastMessageById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        [HttpGet("business/{id}", Name = nameof(GetBroadcastMessageByBusinessId))]
        [ProducesResponseType(typeof(SuccessResponse<BroadcastMessageDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBroadcastMessageByBusinessId([FromQuery] Guid id, ResourceParameter parameter)
        {
            try
            {
                var result = await _broadcastMessage
                    .GetBroadcastMessageByBusinessId(id, parameter, nameof(GetBroadcastMessageByBusinessId), Url);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }
    }
}
