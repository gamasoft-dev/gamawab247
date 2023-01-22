using System;
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
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.Implementations.BusinessMessageImpls;

public class ListBusinessMessageService: 
    IBusinessMessageMgtService<BaseCreateMessageDto, BaseInteractiveDto>
{
    #region Setup
    private readonly IRepository<Business> _businessRepo;
    private readonly IRepository<BusinessMessage> _businessMessageRepo;
    private readonly IRepository<ReplyButtonMessage> _replyButtonMessageRepo;
    private readonly IRepository<ListMessage> _listMessageRepo;
    private readonly IRepository<BusinessMessageSettings> _businessSettingRepo;
    private readonly IMapper _mapper;
    private readonly IOutboundMesageService _outboundMessageService;
    
    public ListBusinessMessageService(IRepository<Business> businessRepo,
        IRepository<BusinessMessage> businessMessageRepo, 
        IRepository<ReplyButtonMessage> replyButtonMessageRepo, 
        IRepository<ListMessage> listMessageRepo, IRepository<TextMessage> textMessageRepo, 
        IRepository<BusinessConversation> businessConversationRepo, 
        IOutboundMesageService outboundMessageService,
        IRepository<BusinessMessageSettings> businessSettingRepo,
        IMapper mapper)
    {
        _businessRepo = businessRepo;
        _businessMessageRepo = businessMessageRepo;
        _replyButtonMessageRepo = replyButtonMessageRepo;
        _listMessageRepo = listMessageRepo;
        _businessSettingRepo = businessSettingRepo;
        _outboundMessageService = outboundMessageService;
        _mapper = mapper;
    }

    #endregion
    
    // ge message type => list
    public string GetMessageType => EMessageType.List.ToString().ToLower();
    
    /// <summary>
    /// Create a business message of list type
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="RestException"></exception>
    public async Task<SuccessResponse<BusinessMessageDto<BaseInteractiveDto>>> 
        CreateMessage(CreateBusinessMessageDto<BaseCreateMessageDto> model)
    {
        if (model.MessageTypeObject is null)
            throw new RestException(HttpStatusCode.BadRequest,
                "Message Payload is incomplete: Message Type Object cannot not be null");
        
        if (!await _businessRepo.ExistsAsync(x => x.Id == model.BusinessId))
            throw new RestException(System.Net.HttpStatusCode.BadRequest, "Business is not found");

        var businessSettings = await _businessSettingRepo.FirstOrDefault(x => x.BusinessId == model.BusinessId);
        if (businessSettings == null)
            throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.BusinessSettingsNotFound);

        var bizMessage = _mapper.Map<BusinessMessage>(model);
        bizMessage.MessageType = EMessageType.List.ToString().ToLower();
        var listBizMessage = _mapper.Map<ListMessage>(model.MessageTypeObject);

        await _businessMessageRepo.AddAsync(bizMessage);
        listBizMessage.BusinessMessageId = bizMessage.Id;
        await _listMessageRepo.AddAsync(listBizMessage);
        bizMessage.InteractiveMessageId = listBizMessage.Id;

        await _listMessageRepo.SaveChangesAsync();

        var businessMessage = await RetrieveBusinessMessageById(bizMessage.Id);
        return new SuccessResponse<BusinessMessageDto<BaseInteractiveDto>>
        {
            Data = businessMessage,
            Message = "Successfully created",
            Success = true
        };    
    }

    /// <summary>
    /// Get the next business message by an interactive message optionId
    /// </summary>
    /// <param name="interactiveMessage"></param>
    /// <param name="businessId"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<(SuccessResponse<BusinessMessage> response, bool isTriggerForm)>
        GetNextBusinessMessageByOptionId(BaseInteractiveDto interactiveMessage, Guid businessId, string option, Guid? formId = null)
    {
        var sections = interactiveMessage?.ListAction?.Sections;
        RowDto row = null; BusinessMessage businessMessage = new();
        bool isTriggerForm = false;


        foreach (var section in sections!)
        {
            row = section.Rows.FirstOrDefault(x => x.Id == option);
            if (row is null)
                break;
        }

        if (row is not null) 
            businessMessage = await _businessMessageRepo.FirstOrDefault(x=>
                x.BusinessId == businessId && x.Position == row.NextBusinessMessagePosition);
       
        if (businessMessage is null)
            throw new BusinessMessageException(
                "There is no corresponding businessMessage for the configured Next Position", null);

        if(formId != null)
        {
            isTriggerForm = true;
        }

        var resp = new SuccessResponse<BusinessMessage>()
        {
            Data = businessMessage,
            Success = true,
            Message = "Successfully retrieved"
        };
        return (resp, isTriggerForm);
    }

    /// <summary>
    /// Get a list based message by id.
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

    /// <summary>
    /// Get the interactive object using by interactiveMessage Id
    /// </summary>
    /// <param name="interactiveMessageId"></param>
    /// <param name="messageType"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<BaseInteractiveDto> GetInteractiveMessageById(Guid interactiveMessageId, string messageType)
    {
        ListMessageDto interactiveMessageDto = null;
        if (string.Equals(messageType, EMessageType.List.ToString(), StringComparison.CurrentCultureIgnoreCase))
        {
            var listBasedMessage = await _listMessageRepo
                .FirstOrDefault(x => x.Id == interactiveMessageId);
            
            if(listBasedMessage is not null)
                interactiveMessageDto = _mapper.Map<ListMessageDto>(listBasedMessage);
        }
        else
        {
            throw new InvalidOperationException($"The resolved implementation cannot handle this Message Type {messageType}");
        }
        return interactiveMessageDto;
    }

    /// <summary>
    /// Send a business message to Dialog 360 for users
    /// </summary>
    /// <param name="waId"></param>
    /// <param name="request"></param>
    /// <param name="inboundMessage"></param>
    /// <returns></returns>
    public async Task<SuccessResponse<bool>> HttpSendBusinessMessage(string waId, 
        BusinessMessageDto<BaseInteractiveDto> request, InboundMessage inboundMessage)
    {
        return await _outboundMessageService.HttpSendListMessage(wa_Id: waId, model: request, inboundMessage: inboundMessage);
    }


    #region  Reuseables
    // retrieve a business message by Id
    async Task<BusinessMessageDto<BaseInteractiveDto>> RetrieveBusinessMessageById(Guid id)
    {
        var businessMessage = await _businessMessageRepo.FirstOrDefault(x => x.Id == id);

        if (businessMessage is null)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.BusinessNotFound);

        var listMessage =
            await _listMessageRepo.FirstOrDefault(x => x.Id == businessMessage.InteractiveMessageId);
        if (listMessage is null)
        {
            return default;
        }

        var listMessageDto = _mapper.Map<ListMessageDto>(listMessage);

        var mappedBusiness = new BusinessMessageDto<BaseInteractiveDto>
        {
            Id = businessMessage.Id,
            BusinessId = businessMessage.BusinessId,
            MessageType = businessMessage.MessageType,
            Name = businessMessage.Name,
            Position = businessMessage.Position,
            RecipientType = businessMessage.RecipientType,
            MessageTypeObject = listMessageDto,
            BusinessConversationId = businessMessage.BusinessConversationId,
            BusinessFormId = businessMessage.BusinessFormId,
            ShouldTriggerFormProcessing = businessMessage.ShouldTriggerFormProcessing,
            HasFollowUpMessage = businessMessage.HasFollowUpMessage,
            FollowParentMessageId = businessMessage.FollowParentMessageId
        };

        return mappedBusiness;
    }
    
    #endregion
}