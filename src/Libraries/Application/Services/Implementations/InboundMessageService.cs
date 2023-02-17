using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.DTOs.InboundMessageDto;
using Domain.Enums;
using Newtonsoft.Json;
using Application.Common.Sessions;
using Application.Services.Interfaces.FormProcessing;
using Application.Cron.ResponseProcessing;

namespace Application.Services.Implementations
{
    public partial class InboundMessageService: IInboundMessageService
    {
        private readonly IRepository<InboundMessage> _inboundMessageRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<InboundMessageService> _logger;
        private readonly ISystemSettingsService _systemSettingService;
        private readonly IBusinessService _businessService;
        private readonly IMessageSessionService _messageSessionService;
        private SystemSettingsConfig _configuration;
        private readonly IWhatsappUserService _whatsappUserService;
        private readonly IMessageLogService _messageLogService;
        private readonly ISessionManagement _sessionManagement;
        private readonly IFormRequestResponseService _formRequestResponseService;
        private readonly IBusinessFormService _businessFormService;
        private readonly IResponsePreProcessingCron _responsePreProcessingCron;
        private readonly IFormValidationStrategyService _formValidationStrategyService;


        private EMessageType _BaseMessageType { get; set; } = EMessageType.UnKnown;
        private EMessageType _InteractiveMessageType { get; set; } = EMessageType.UnKnown;
        private bool _IsInteractiveResolved { get; set; }
        public _360MessageDto _ReceivedMessage { get; set; }
        private Business _Business { get; set; }
        private WhatsappUserDto _WhatsappUser { get; set; }

        public InboundMessageService(IRepository<InboundMessage> repository,
            IMapper mapper,
            ISystemSettingsService systemMessage,
            ILogger<InboundMessageService> logger,
            IOptionsSnapshot<SystemSettingsConfig> options,
            IBusinessService businessSettingsService, 
            IWhatsappUserService whatsappUserService,
            IMessageSessionService messageSessionService,
            IMessageLogService messageLogService,
            ISessionManagement sessionManagement,
            IFormRequestResponseService formRequestResponseService,
            IBusinessFormService businessFormService,
            IResponsePreProcessingCron responsePreProcessingCron,
            IFormValidationStrategyService formValidationStrategyService
            )
        {
            _inboundMessageRepo = repository;
            _mapper = mapper;
            _systemSettingService = systemMessage;
            _logger = logger;
            _configuration = options.Value;
            _businessService = businessSettingsService;
            _messageSessionService = messageSessionService;
            _whatsappUserService = whatsappUserService;
            _messageLogService = messageLogService;
            _sessionManagement = sessionManagement;
            _formRequestResponseService = formRequestResponseService;
            _businessFormService = businessFormService;
            _responsePreProcessingCron = responsePreProcessingCron;
            _formValidationStrategyService = formValidationStrategyService;
        }

        public IInboundMessageService ResolveBaseMessageType(dynamic requestPayload)
        {
            _360MessageDto message = JsonConvert.DeserializeObject<_360MessageDto>(requestPayload.ToString());

            if (message?.messages[0]?.type == EMessageType.Interactive.ToString().ToLower())
                _BaseMessageType = EMessageType.Interactive;

            else if (message.messages[0].type == EMessageType.Text.ToString().ToLower())
                _BaseMessageType = EMessageType.Text;

            else if(message.messages[0].type == "list_reply" || message.messages[0].type == "button_reply")
                _BaseMessageType = EMessageType.Interactive;

            this._ReceivedMessage = message;
            return this;
        }

        public IInboundMessageService ResolveInteractiveMessageType(dynamic requestPayload)
        {
            if (_BaseMessageType == EMessageType.Interactive)
            {
                _360MessageDto requestSerializedPayload = JsonConvert.DeserializeObject<_360MessageDto>(requestPayload.ToString());

                if (requestSerializedPayload?.interactive?.type == EMessageType.List.ToString().ToLower())
                    _InteractiveMessageType = EMessageType.List;
           
                else if (requestSerializedPayload?.interactive?.type == EMessageType.Button.ToString().ToLower())
                    _InteractiveMessageType = EMessageType.Button;

                else if(requestSerializedPayload?.messages.FirstOrDefault()?.interactive?.type == "list_reply")
                    _InteractiveMessageType = EMessageType.List;

                else if(requestSerializedPayload?.messages.FirstOrDefault()?.interactive?.type == "button_reply")
                    _InteractiveMessageType = EMessageType.Button;

                else
                    throw new RestException(HttpStatusCode.BadRequest, 
                        "Could not resolve the right interactive message");
                
                _IsInteractiveResolved = true;
            }
            else if(_BaseMessageType == EMessageType.Text)
            {
                _InteractiveMessageType = EMessageType.Text;
            }

            return this;
        }

        public async Task ReceiveAndProcessMessage(Guid businessId,  dynamic requestData)
        {
            var business = await _businessService.GetBusinessByBusinessId(businessId);

            this._Business = _mapper.Map<Business>(business.Data);
            // check business is not null

            if (_BaseMessageType != EMessageType.UnKnown)
            {
                // get the actual type of the baseMessageType
                // and further resolve the specific type if it's an interactive

                // save message user record to the whatsappUser schema.

                #region upserting user region Handled by Richard
                DefaultMessageNotificationDto defaultMessageWithContact =
                    JsonConvert.DeserializeObject<DefaultMessageNotificationDto>(requestData.ToString());

                if (defaultMessageWithContact is not null && defaultMessageWithContact.Contacts.Any())
                {
                    var messageContact = defaultMessageWithContact.Contacts.FirstOrDefault();
                    UpsertWhatsappUserDto whatsappUser = _mapper.Map<UpsertWhatsappUserDto>(messageContact);
                    var whatsappUserDto = await _whatsappUserService.UpsertWhatsappUser(whatsappUser);
                    this._WhatsappUser = whatsappUserDto.Data;                
                }
                else
                {
                    throw new RestException(HttpStatusCode.BadRequest,
                        "Message Contact information was not received or resolved");
                }

                #endregion

                if (_BaseMessageType == EMessageType.Text)
                {
                    await ReceiveTextBasedMessage(businessId, requestData);
                }
                if (_BaseMessageType == EMessageType.Interactive)
                {
                    if (_InteractiveMessageType == EMessageType.Button)
                    {
                        // call the service method for receipt of business
                        // message for Button and process it as done in Text
                        await ReceiveButtonBasedMessage(businessId, requestData);
                    }
                    else if (_InteractiveMessageType == EMessageType.List)
                    {
                        // call the service method for receipt of business message for List
                        await ReceiveListBasedMessage(businessId, requestData);
                    }
                }
            }
        }

    }
}