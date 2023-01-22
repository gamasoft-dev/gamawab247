using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.OutboundMessageRequests;
using Application.DTOs.CreateDialogDtos;
using Newtonsoft.Json;
using Domain.Entities;
using Domain.Enums;
using Domain.Entities.FormProcessing;
using System;

// TODO: Complete and resolve errors around message log integration on message sending.
namespace Application.Services.Implementations
{
    public class OutboundMessageService : IOutboundMesageService
    {
        private readonly IHttpService _httpService;
        private readonly IMapper _mapper;
        private readonly IRepository<OutboundMessage> _outboundRepo;
        private readonly IBusinessSettingsService _businessSettingsService;
        private readonly Dialog360Settings _dialog360Settings;
        private readonly IWhatsappUserService _whatsappUserService;
        private readonly IBusinessService _businessService;
        private readonly IMessageLogService _messageLogService;

        public OutboundMessageService(IHttpService httpService,
            IOptions<Dialog360Settings> options,
            IMapper mapper, IRepository<OutboundMessage> outboundRepo,
            IBusinessSettingsService businessSettingsService, IWhatsappUserService whatsappUserService,
            IBusinessService businessService, IMessageLogService messageLogService)
        {
            _httpService = httpService;
            _dialog360Settings = options.Value;
            _mapper = mapper;
            _outboundRepo = outboundRepo;
            _businessSettingsService = businessSettingsService;
            _whatsappUserService = whatsappUserService;
            _businessService = businessService;
            _messageLogService = messageLogService;
        }

        //ToDo needs review
        public async Task<SuccessResponse<bool>> HttpSendReplyButtonMessage(string wa_Id,
            BusinessMessageDto<BaseInteractiveDto> model, InboundMessage inboundMessage)
        {
            var businessSettings = await _businessSettingsService.GetByBusinessId(model.BusinessId);
            var whatsappUser = await _whatsappUserService.GetWhatsappUserByWaId(wa_Id);
            var business = await _businessService.GetBusinessByBusinessId(model.BusinessId);

            if (businessSettings is null)
                throw new RestException(System.Net.HttpStatusCode.NotFound,
                    "No Business Settings avalible to send message for this business to user(s)");

            if (_dialog360Settings is null)
                throw new RestException(System.Net.HttpStatusCode.InternalServerError,
                    "The enviroment variable for 360Dialog could not be retrieved from settings");

            if (model.MessageTypeObject is not null)
            {
                //model.MessageTypeObject.ButtonAction.Buttons[0].Reply.Id = waUniqueId;
                var requestObject = ToWhatsAppModel(wa_Id, model);

                //var textMessageJsonObject = JsonConvert.SerializeObject(requestObject);

                const string endpoint = "v1/messages";
                var url = $"{_dialog360Settings.BaseUrl}/{endpoint}";

                //  retrieve api key from business settings.
                IDictionary<string, string> dictNew = new Dictionary<string, string>();
                dictNew.Add(_dialog360Settings?.AuthorizationName, businessSettings?.Data.ApiKey);

                var header = new RequestHeader(dictNew);

                var httpResult = await _httpService.Post<OutboundMessageResponseDto, ReplyButtonMessageRequest>
                    (url: url, header: header, request: requestObject);

                var buttonOutboundMessage = _mapper.Map<OutboundMessage>(model);

                // determining if the message sent without an error.
                if (httpResult.Data.Errors is null /*&& !string.IsNullOrEmpty(httpResult.Data.Meta.ApiStatus)*/)
                {
                    buttonOutboundMessage.WhatsAppMessageId = httpResult.Data?.Messages?.FirstOrDefault()?.Id;
                    buttonOutboundMessage.BusinessMessageId = model.Id;
                    buttonOutboundMessage.BusinessId = model.BusinessId;
                    buttonOutboundMessage.IsFirstMessageSent = model.Position == 1;
                    buttonOutboundMessage.RecipientWhatsappId = wa_Id;
                    buttonOutboundMessage.Id = new System.Guid();
                    // finally save result to the db.
                    await SaveOutboundMessage(buttonOutboundMessage);

                    var messageLog = new CreateMessageLogDto()
                    {
                        MessageDirection = EMessageDirection.Outbound,
                        MessageType = EMessageType.List,
                        MessageBody = requestObject.interactive.body.text,
                        From = business.Data.Name,
                        To = wa_Id,
                        BusinessId = model.BusinessId,
                        WhatsappUserId = whatsappUser.Data.Id,
                        IsRead = false,
                        RequestResponseData = JsonConvert.SerializeObject(requestObject)
                    };
                    await _messageLogService.CreateMessageLog(messageLog);
                }
            }
            return null;
        }

        public async Task<SuccessResponse<bool>> HttpSendListMessage(string wa_Id, BusinessMessageDto<BaseInteractiveDto> model,
            InboundMessage inboundMessage)
        {
            var isSuccess = false;
            var businessSettings = await _businessSettingsService.GetByBusinessId(model.BusinessId);
            var whatsappUser = await _whatsappUserService.GetWhatsappUserByWaId(wa_Id);
            var business = await _businessService.GetBusinessByBusinessId(model.BusinessId);


            if (businessSettings is null)
                throw new RestException(System.Net.HttpStatusCode.NotFound,
                    "No Business Settings avalible to send message for this business to user(s)");

            if (_dialog360Settings is null)
                throw new RestException(System.Net.HttpStatusCode.InternalServerError,
                    "The enviroment variable for 360Dialog could not be retrieved from settings");

            if (model.MessageTypeObject is not null)
            {
             //  var sections = _mapper.Map<List<Sections>>(model.MessageTypeObject.ListAction.Sections);
              var sections = new List<Sections>();
              if (model?.MessageTypeObject?.ListAction?.Sections != null)
                  foreach (var section in model?.MessageTypeObject?.ListAction?.Sections)
                  {
                      var rows = new List<Rows>();
                      foreach (var row in section.Rows)
                      {
                          rows.Add(new Rows()
                          {
                              id = row.Id,
                              description = row.Description,
                              title = row.Title,
                          });
                      }
                      sections.Add(new Sections()
                      {
                          title = section.Title,
                          rows = rows
                      });
                     
                  }

              var listMessagePayLoad = new ListMessageInteractiveDto()
                {
                    to = wa_Id,
                    recipienttype = "individual",
                    type = "interactive",
                    interactive = new Interactive()
                    {
                        type = "list",
                        header = new()
                        {
                            type = "text",
                            text = model.MessageTypeObject.Header
                        },
                        footer = new()
                        {
                            text = model.MessageTypeObject.Footer,
                        },
                        body = new()
                        {
                            text = model.MessageTypeObject.Body ?? model?.MessageTypeObject?.Header,
                        },
                        action = new DTOs.OutboundMessageRequests.Action()
                        {
                            button = model?.MessageTypeObject?.ListAction?.Button,
                            sections = sections
                        }
                    }
                };

                var textMessageJsonObject = JsonConvert.SerializeObject(listMessagePayLoad);

                const string endpoint = "v1/messages";
                var url = $"{_dialog360Settings.BaseUrl}/{endpoint}";

                //  retrieve api key from business settings.
                IDictionary<string, string> dictNew = new Dictionary<string, string>();
                dictNew.Add(_dialog360Settings?.AuthorizationName, businessSettings?.Data.ApiKey);

                var header = new RequestHeader(dictNew);

                var httpResult = await _httpService.Post<OutboundMessageResponseDto, ListMessageInteractiveDto>
                    (url: url, header: header, request: listMessagePayLoad);

                var listOutboundMessage = _mapper.Map<OutboundMessage>(model);

                // determinng if the message sent without an error.
                if (httpResult.Data is not null /*&& !string.IsNullOrEmpty(httpResult.Data.Meta.ApiStatus)*/)
                {
                    listOutboundMessage.WhatsAppMessageId = httpResult.Data?.Messages?.FirstOrDefault()?.Id;
                    listOutboundMessage.BusinessMessageId = model.Id;
                    listOutboundMessage.IsFirstMessageSent = model.Position == 1;
                    listOutboundMessage.BusinessId = model.BusinessId;
                    listOutboundMessage.RecipientWhatsappId = wa_Id;
                    isSuccess = true;
                    listOutboundMessage.Id = new System.Guid();
                    // finally save result to the db.
                    await SaveOutboundMessage(listOutboundMessage);

                    // save message log
                    var messageLog = new CreateMessageLogDto()
                    {
                        MessageDirection = EMessageDirection.Outbound,
                        MessageType = EMessageType.List,
                        MessageBody = listMessagePayLoad.interactive.body.text,
                        From = business.Data.Name,
                        To = wa_Id,
                        BusinessId = model.BusinessId,
                        WhatsappUserId = whatsappUser.Data.Id,
                        IsRead = false,
                        RequestResponseData = JsonConvert.SerializeObject(listMessagePayLoad)
                    };
                    await _messageLogService.CreateMessageLog(messageLog);
                }
            }

            return new SuccessResponse<bool>()
            {
                Data = isSuccess,
                Message = "Delivery sent to Whatsapp successfully"
            };
        }

        public async Task<SuccessResponse<bool>> HttpSendTextMessage(string wa_Id, 
            BusinessMessageDto<BaseInteractiveDto> model, InboundMessage inboundMessage)
        {
            var businessSettings = await _businessSettingsService.GetByBusinessId(model.BusinessId);
            var whatsappUser = await _whatsappUserService.GetWhatsappUserByWaId(wa_Id);
            var business = await _businessService.GetBusinessByBusinessId(model.BusinessId);

            if (businessSettings is null)
                throw new RestException(System.Net.HttpStatusCode.NotFound,
                    "No Business Settings available to send message for this business to user(s)");

            if (_dialog360Settings is null)
                throw new RestException(System.Net.HttpStatusCode.InternalServerError,
                    "The environment variable for 360Dialog could not be retrieved from settings");
            
            var msgBody = model.Position == 1
                ? BusinessSettingsUtility.GetFirstMessage(
                    businessSettings.Data.BotName, businessSettings.Data.BotDescription, inboundMessage.WhatsUserName)
                : model?.MessageTypeObject?.Body;

            
            if (model.MessageTypeObject is not null)
            {
                var replyTextMessageRequest = new TextMessageRequestDto()
                {
                    previewUrl = false,
                    recipient_type = "individual",
                    to = wa_Id,
                    type = "text",
                    text = new Text()
                    {
                        body = msgBody
                    }
                };

                //var textMessageJsonObject = JsonConvert.SerializeObject(replyTextMessageRequest);

                const string endpoint = "v1/messages";
                var url = $"{_dialog360Settings.BaseUrl}/{endpoint}";              

                //  retrieve api key from business settings.
                IDictionary<string, string> dictNew = new Dictionary<string, string>();
                dictNew.Add(_dialog360Settings?.AuthorizationName, businessSettings?.Data.ApiKey);

                var header = new RequestHeader(dictNew);

                var httpResult = await _httpService.Post<OutboundMessageResponseDto, TextMessageRequestDto>
                    (url: url, header: header, request: replyTextMessageRequest);

                var textOutboundMessage = _mapper.Map<OutboundMessage>(model);
               
                // determining if the message sent without an error.
                if (httpResult.Data is not null)
                {
                    textOutboundMessage.WhatsAppMessageId = httpResult.Data?.Messages?.FirstOrDefault()?.Id;
                    textOutboundMessage.BusinessMessageId = model.Id;
                    textOutboundMessage.IsFirstMessageSent = model.Position == 1;
                    textOutboundMessage.BusinessId = model.BusinessId;
                    textOutboundMessage.Id = new System.Guid();
                    textOutboundMessage.RecipientWhatsappId = wa_Id;

                    if (model.Position > 0)
                    {
                        // finally save result to the db.
                        await SaveOutboundMessage(textOutboundMessage);
                    }
                    // save message log
                    var messageLog = new CreateMessageLogDto()
                    {
                        MessageDirection = EMessageDirection.Outbound,
                        MessageType = EMessageType.Text,
                        MessageBody = replyTextMessageRequest.text.body,
                        From = business.Data.Name,
                        To = wa_Id,
                        BusinessId = model.BusinessId,
                        WhatsappUserId = whatsappUser.Data.Id,
                        IsRead = false,
                        RequestResponseData = JsonConvert.SerializeObject(replyTextMessageRequest)
                    };
                    await _messageLogService.CreateMessageLog(messageLog);
                }
                
            }
            return null;
        }

        private async Task SaveOutboundMessage(OutboundMessage model)
        {
            await _outboundRepo.AddAsync(model);

            await _outboundRepo.SaveChangesAsync();
        }
        private static ReplyButtonMessageRequest ToWhatsAppModel(string to, BusinessMessageDto<BaseInteractiveDto> model)
        {
            var buttons = new List<ButtonDto>();
            ReplyButtonMessageRequest requestObject = new ReplyButtonMessageRequest();
            //dynamic requestObject = new ExpandoObject();
            
            foreach (var buttonOption in model.MessageTypeObject.ButtonAction.Buttons)
            {
                buttons.Add(new ButtonDto()
                {
                    type = "reply",
                    reply = new ReplyDto()
                    {
                        id = buttonOption.reply.id, //Whatsapp Message Id
                        title = buttonOption.reply.title
                    },
                });
            }

            requestObject.to = to;
            requestObject.recipient_type = "individual";
            requestObject.type = "interactive";
            requestObject.interactive = new ReplyButtonInteractiveDto();
            requestObject.interactive.type = "button";
            requestObject.interactive.header = new Header();
            requestObject.interactive.header.type = "text";
            requestObject.interactive.header.text = model?.MessageTypeObject?.Header;
            requestObject.interactive.body = new Body();
            requestObject.interactive.body.text = model?.MessageTypeObject?.Body;
            requestObject.interactive.footer = new Footer();
            requestObject.interactive.footer.text = model?.MessageTypeObject?.Footer;

            requestObject.interactive.action = new ActionDto();
            requestObject.interactive.action.buttons = buttons;

            return requestObject;
        }

        public async Task<SuccessResponse<bool>> HttpSendTextMessage(string wa_Id, FormRequestResponse model)
        {
            var businessSettings = await _businessSettingsService.GetByBusinessId(model.BusinessId);
            var whatsappUser = await _whatsappUserService.GetWhatsappUserByWaId(wa_Id);
            var business = await _businessService.GetBusinessByBusinessId(model.BusinessId);

            if (businessSettings is null)
                throw new RestException(System.Net.HttpStatusCode.NotFound,
                    "No Business Settings available to send message for this business to user(s)");

            if (_dialog360Settings is null)
                throw new RestException(System.Net.HttpStatusCode.InternalServerError,
                    "The environment variable for 360Dialog could not be retrieved from settings");


            if (model is not null)
            {
                var replyTextMessageRequest = new TextMessageRequestDto()
                {
                    previewUrl = false,
                    recipient_type = "individual",
                    to = wa_Id,
                    type = "text",
                    text = new Text()
                    {
                        body = model.Message
                    }
                };

                //var textMessageJsonObject = JsonConvert.SerializeObject(replyTextMessageRequest);

                const string endpoint = "v1/messages";
                var url = $"{_dialog360Settings.BaseUrl}/{endpoint}";

                //  retrieve api key from business settings.
                IDictionary<string, string> dictNew = new Dictionary<string, string>();
                dictNew.Add(_dialog360Settings?.AuthorizationName, businessSettings?.Data.ApiKey);

                var header = new RequestHeader(dictNew);

                var httpResult = await _httpService.Post<OutboundMessageResponseDto, TextMessageRequestDto>
                    (url: url, header: header, request: replyTextMessageRequest);

                // determining if the message sent without an error.
                if (httpResult.Data is not null)
                {
                    // save message log
                    var messageLog = new CreateMessageLogDto()
                    {
                        MessageDirection = EMessageDirection.Outbound,
                        MessageType = EMessageType.Text,
                        MessageBody = replyTextMessageRequest.text.body,
                        From = business.Data.Name,
                        To = wa_Id,
                        BusinessId = model.BusinessId,
                        WhatsappUserId = whatsappUser.Data.Id,
                        IsRead = false,
                        RequestResponseData = JsonConvert.SerializeObject(replyTextMessageRequest)
                    };
                    await _messageLogService.CreateMessageLog(messageLog);
                }

            }
            return new SuccessResponse<bool>
            {
                Data = true
            };
        }

        public Task<SuccessResponse<bool>> SendMessage(string messageType, string wa_Id,
            BusinessMessageDto<BaseInteractiveDto> model, InboundMessage message)
        {
            bool tryParseEnum = Enum.TryParse(typeof(EMessageType), messageType, true, out object eMessageType);

            if(!tryParseEnum)
                throw new Exception($"Invalid or null message type passed. Souce:: {nameof(OutboundMessageService.SendMessage)}");

            switch (eMessageType)
            {
                case EMessageType.Button:
                    return HttpSendReplyButtonMessage(wa_Id, model, message);

                case EMessageType.List:
                    return HttpSendListMessage(wa_Id, model, message);

                case EMessageType.Text:
                    return HttpSendTextMessage(wa_Id, model, message);

                default:
                    throw new Exception("No implementation found Souce:: {nameof(OutboundMessageService.SendMessage)}");
            }               
        }
    }
}
