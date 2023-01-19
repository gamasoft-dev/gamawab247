using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.DTOs.CreateDialogDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.DialogMessageEntitties.ValueObjects;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.Implementations.BusinessMessageImpls;

public class ButtonBusinessMessageService:  
    IBusinessMessageMgtService<BaseCreateMessageDto, BaseInteractiveDto>
{
    #region Setup
    private readonly IRepository<Business> _businessRepo;
    private readonly IRepository<BusinessMessage> _businessMessageRepo;
    private readonly IRepository<ReplyButtonMessage> _replyButtonMessageRepo;
    private readonly IRepository<BusinessMessageSettings> _businessSettingRepo;
    private readonly IMapper _mapper;
    private readonly IOutboundMesageService _outboundMesageService;
    
    public ButtonBusinessMessageService(IRepository<Business> businessRepo,
        IRepository<BusinessMessage> businessMessageRepo,
        IRepository<ReplyButtonMessage> replyButtonMessageRepo, 
        IRepository<ListMessage> listMessageRepo, IRepository<TextMessage> textMessageRepo,
        IRepository<BusinessConversation> businessConversationRepo, 
        IOutboundMesageService outboundMesageService,
        IRepository<BusinessMessageSettings> businessSettingRepo, IMapper mapper)
    {
        _businessRepo = businessRepo;
        _businessMessageRepo = businessMessageRepo;
        _replyButtonMessageRepo = replyButtonMessageRepo;
        _businessSettingRepo = businessSettingRepo;
        _mapper = mapper;
        _outboundMesageService = outboundMesageService;
    }
    #endregion
    
    // get the concrete message type which is as well the implementation type
    public string GetMessageType => EMessageType.Button.ToString().ToLower();

    /// <summary>
    /// Create a button based Business Message.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="RestException"></exception>
    public  async Task<SuccessResponse<BusinessMessageDto<BaseInteractiveDto>>> 
        CreateMessage(CreateBusinessMessageDto<BaseCreateMessageDto> model)
    {
        #region Validations
        var checkBizId = await _businessRepo.ExistsAsync(x => x.Id == model.BusinessId);
        if (!checkBizId)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.IncorrectId);

        if (await _businessMessageRepo.ExistsAsync(
                x => x.BusinessId == model.BusinessId && x.Position == model.Position))
            throw new RestException(HttpStatusCode.BadRequest, "Cannot create duplicate business message at same position");
        
        var checkBizMessId = await _businessSettingRepo.FirstOrDefault(x => x.BusinessId == model.BusinessId);
        if(checkBizMessId == null)
            throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.BusinessSettingsNotFound);
        
        #endregion
        
        //var bizMessage = _mapper.Map<BusinessMessage>(model);
        var bizMessage = new BusinessMessage
        {
            MessageType = EMessageType.Button.ToString(),
            Position = model.Position,
            Name = model.Name,
            RecipientType = model.RecipientType,
            BusinessId = model.BusinessId
        };

        //var replyBizMessage = _mapper.Map<ReplyButtonMessage>(model.MessageTypeObject);
        ReplyButtonMessage replyBizMessage = new ReplyButtonMessage
        {
            Body = model.MessageTypeObject.Body,
            ButtonAction = new Domain.Entities.DialogMessageEntitties.ValueObjects.ButtonAction
            {
                Buttons = _mapper.Map<List<Button>>(model.MessageTypeObject.ButtonAction.Buttons)
            },
            Footer = model.MessageTypeObject.Footer,
            Header = model.MessageTypeObject.Header
        };
        //bizMessage.BusinessId = model.BusinessId;

        await _businessMessageRepo.AddAsync(bizMessage);
        replyBizMessage.BusinessMessageId = bizMessage.Id;
        await _replyButtonMessageRepo.AddAsync(replyBizMessage);
        bizMessage.InteractiveMessageId = replyBizMessage.Id;
        
        await _replyButtonMessageRepo.SaveChangesAsync();

        BusinessMessageDto<BaseInteractiveDto> businessMessage = await RetrieveBusinessMessageById(bizMessage.Id);

        return new SuccessResponse<BusinessMessageDto<BaseInteractiveDto>>
        {
            Data = businessMessage,
            Message = "Successfully created",
            Success = true
        };
    }
    
    /// <summary>
    /// Get the interactive object using by interactiveMessage Id
    /// </summary>
    /// <param name="interactiveMessageId"></param>
    /// <param name="messageType"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<BaseInteractiveDto> GetInteractiveMessageById(Guid interactiveMessageId,
        string messageType)
    {
        ButtonMessageDto interactiveMessageDto = null;
        if (string.Equals(messageType, EMessageType.Button.ToString(), StringComparison.CurrentCultureIgnoreCase))
        {
            var replyButtonMessage = await _replyButtonMessageRepo
                .FirstOrDefault(x => x.Id == interactiveMessageId);
            
            if(replyButtonMessage is not null)
                interactiveMessageDto = _mapper.Map<ButtonMessageDto>(replyButtonMessage);
        }
        else
        {
            throw new InvalidOperationException($"The resolved implementation cannot handle this Message Type {messageType}");
        }
        return interactiveMessageDto;
    }

    /// <summary>
    /// Send a button business based message.
    /// </summary>
    /// <param name="waId"></param>
    /// <param name="request"></param>
    /// <param name="inboundMessage"></param>
    /// <returns></returns>
    public async Task<SuccessResponse<bool>> HttpSendBusinessMessage(string waId,
        BusinessMessageDto<BaseInteractiveDto> request, InboundMessage inboundMessage)
    {
        return await _outboundMesageService.HttpSendReplyButtonMessage(wa_Id: waId, model: request,
            inboundMessage: inboundMessage);
    }

    /// <summary>
    /// Get a business message type based on message option (list and or button message options)
    /// </summary>
    /// <param name="interactiveMessage"></param>
    /// <param name="businessId"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<(SuccessResponse<BusinessMessage> response, bool isTriggerForm)> GetNextBusinessMessageByOptionId(BaseInteractiveDto interactiveMessage,
        Guid businessId, string option, Guid? formId = null)
    {
        var button = interactiveMessage?.ButtonAction?.Buttons?.FirstOrDefault(x => x.reply.id == option);
        var businessMessage = new BusinessMessage();
        bool isTriggerForm = false;


        if (button is not null) 
            businessMessage = await _businessMessageRepo.FirstOrDefault(x=>
                x.BusinessId == businessId && (x.Position == button.NextBusinessMessagePosition));
       
        if (businessMessage is null)
            throw new BusinessMessageException(
                "There is no corresponding businessMessage for the configured Next Position", null);

        var resp = new SuccessResponse<BusinessMessage>()
        {
            Data = businessMessage,
            Success = true,
            Message = "Successfully retrieved"
        };
        return (resp, isTriggerForm);
    }

    /// <summary>
    /// Get a business message by it's id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<SuccessResponse<BusinessMessageDto<BaseInteractiveDto>>> GetBusinessMessageById(Guid id)
    {
        var businessMessage = await RetrieveBusinessMessageById(id);
        return new SuccessResponse<BusinessMessageDto<BaseInteractiveDto>>
        {
            Data = businessMessage,
            Message = "Successfully retrieved",
            Success = true
        };
    }

    #region  Reuseables
    // retrieve a business message by Id
    async Task<BusinessMessageDto<BaseInteractiveDto>> RetrieveBusinessMessageById(Guid id)
    {
        //var businessMessageResponse = new BusinessMessageDto<T>();
        var businessMessage = await _businessMessageRepo.FirstOrDefault(x => x.Id == id);

        if (businessMessage is null)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.BusinessNotFound);

        var replyButton =
            await _replyButtonMessageRepo.FirstOrDefault(x => x.Id == businessMessage.InteractiveMessageId);
        if (replyButton is null)
        {
            return default;
        }

        var replyButtonMessage = _mapper.Map<ButtonMessageDto>(replyButton);

        var mappedBusiness = new BusinessMessageDto<BaseInteractiveDto>
        {
            Id = businessMessage.Id,
            BusinessId = businessMessage.BusinessId,
            MessageType = businessMessage.MessageType,
            Name = businessMessage.Name,
            Position = businessMessage.Position,
            RecipientType = businessMessage.RecipientType,
            MessageTypeObject = replyButtonMessage,
            BusinessFormId = businessMessage.BusinessFormId,
            BusinessConversationId = businessMessage.BusinessConversationId,
            ShouldTriggerFormProcessing = businessMessage.ShouldTriggerFormProcessing,
            FollowParentMessageId = businessMessage.FollowParentMessageId,
            HasFollowUpMessage = businessMessage.HasFollowUpMessage
        };

        return mappedBusiness;
    }
    
    #endregion
   }