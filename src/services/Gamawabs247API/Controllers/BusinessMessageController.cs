using Application.DTOs.CreateDialogDtos;
using Application.Exceptions;
using Application.Helpers.InboundMessageHelper;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    public partial class BusinessesController : ControllerBase
    {
        private readonly IBusinessMessageFactory _businessMessageFactory;
        private readonly IBusinessSettingsService _businessSettingsService;
        private readonly IBusinessService _businessService;
        private readonly ILogger<BusinessesController> _logger;
        private readonly IInboundMessageService _inboundMessageService;
        private readonly IMessageProcessor _messageProcessor;
        private readonly IMessageTypeResolver _messageTypeResolver;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public BusinessesController(
            IBusinessSettingsService businessSettingsService,
            IBusinessMessageFactory businessMessageFactory,
            IBusinessService businessService,
            IInboundMessageService messageService,
            IMapper mapper,
            ILogger<BusinessesController> logger,
            IMessageProcessor messageProcessor, IMessageTypeResolver messageTypeResolver, IUserService userService)
        {
            _businessSettingsService = businessSettingsService;
            _businessService = businessService;
            _logger = logger;
            _mapper = mapper;
            _inboundMessageService = messageService;
            _messageProcessor = messageProcessor;
            _messageTypeResolver = messageTypeResolver;
            _businessMessageFactory = businessMessageFactory;
            _userService = userService;
        }

        /// <summary>
        /// This is the endpoint that receives messages from whatsapp api. (callback url per business)
        /// This receives all the messages irrespective of the message type.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost("{id}/message")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> ProcessMessage([FromRoute] Guid id, [FromBody] dynamic message)
        {
            try{

                await _messageProcessor.ValidateInboundMessage(id, message);

                IInboundMessageService inboundService = _inboundMessageService.ResolveBaseMessageType(message);
                inboundService.ResolveInteractiveMessageType(message);

                await inboundService.ReceiveAndProcessMessage(id, message);

                return Ok();
            }
            catch(ProcessCancellationException ex)
            {
                _logger.LogInformation(ex.Message, ex);
                return Ok("Process was cancelled because of redundant request or " +
                    "absense of required params but endpoint performed its core function");
            }            
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        /// <summary>
        /// Endpoint to get business message by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BusinessMessageDto<dynamic>), 200)]
        public async Task<IActionResult> GetBusinessMessageById<T>(Guid id)
        {
            var response = await _businessMessageFactory.GetBusinessMessageImpl(null)
                ?.GetBusinessMessageById(id)!;
            
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create button message
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("button-message")]
        [ProducesResponseType(typeof(BusinessMessageDto<ButtonMessageDto>), 200)]
        public async Task<IActionResult> CreateButtonMessageDto(CreateBusinessMessageDto<CreateButtonMessageDto> model)
        {
            var payload = ConvertToBaseCreateDto(model);

            var response = await _businessMessageFactory.GetBusinessMessageImpl(EMessageType.Button.ToString().ToLower())
                .CreateMessage(payload);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create list message
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("list-message")]
        [ProducesResponseType(typeof(BusinessMessageDto<TextMessageDto>), 200)]
        public async Task<IActionResult> CreateListMessageDto(CreateBusinessMessageDto<CreateListMessageDto> model)
        {
            var payload = ConvertToBaseCreateDto(model);
            var response = await _businessMessageFactory.GetBusinessMessageImpl(EMessageType.List.ToString().ToLower())
                .CreateMessage(payload);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create text message
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("text-message")]
        [ProducesResponseType(typeof(BusinessMessageDto<TextMessageDto>), 200)]
        public async Task<IActionResult> CreateTextMessageDto(CreateBusinessMessageDto<CreateTextMessageDto> model)
        {
            var payload = ConvertToBaseCreateDto(model);
            var response = await _businessMessageFactory.GetBusinessMessageImpl(EMessageType.Text.ToString().ToLower())
                .CreateMessage(payload);

            return Ok(response);
        }

        // /// <summary>
        // /// Endpoint to create list message
        // /// </summary>
        // /// <param name="model"></param>
        // /// <returns></returns>
        // [HttpPost("update-list-message")]
        // [ProducesResponseType(typeof(BusinessMessageDto<TextMessageDto>), 200)]
        // public async Task<IActionResult> UpdateListMessageDto(Guid businessMessageId, UpdateBusinessMessageDto<UpdateListMessageDto> model)
        // {
        //     var response = await _businessMessageRepo.UpdateListMessage(businessMessageId,model);
        //
        //     return Ok(response);
        // }
        //
        // /// <summary>
        // /// Endpoint to create text message
        // /// </summary>
        // /// <param name="model"></param>
        // /// <returns></returns>
        // [HttpPost("update-text-message")]
        // [ProducesResponseType(typeof(BusinessMessageDto<TextMessageDto>), 200)]
        // public async Task<IActionResult> UpdateTextMessageDto(Guid businessMessageId, UpdateBusinessMessageDto<UpdateTextMessageDto> model)
        // {
        //     var response = await _businessMessageRepo.UpdateTextMessage(businessMessageId, model);
        //
        //     return Ok(response);
        // }

        private CreateBusinessMessageDto<BaseCreateMessageDto> ConvertToBaseCreateDto(
            CreateBusinessMessageDto<CreateButtonMessageDto> model)
        {
            var payload = _mapper.Map<BaseCreateMessageDto>(model.MessageTypeObject);

            var resultDto = _mapper.Map<CreateBusinessMessageDto<BaseCreateMessageDto>>(model);
            return resultDto;
        }
        private CreateBusinessMessageDto<BaseCreateMessageDto> ConvertToBaseCreateDto(
            CreateBusinessMessageDto<CreateListMessageDto> model)
        {
            var resultDto = _mapper.Map<CreateBusinessMessageDto<BaseCreateMessageDto>>(model);
            return resultDto;
        }
        
        private CreateBusinessMessageDto<BaseCreateMessageDto> ConvertToBaseCreateDto(
            CreateBusinessMessageDto<CreateTextMessageDto> model)
        {
            var resultDto = _mapper.Map<CreateBusinessMessageDto<BaseCreateMessageDto>>(model);
            return resultDto;
        }
    }
}