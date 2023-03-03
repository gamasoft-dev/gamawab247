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

/// <summary>
/// TODO: Implement Complaint and Ticketing System
/// </summary>
public class TextBusinessMessageService:  
    IBusinessMessageMgtService<BaseCreateMessageDto, BaseInteractiveDto>
{
    #region Setup
    private readonly IRepository<Business> _businessRepo;
    private readonly IRepository<BusinessMessage> _businessMessageRepo;
    private readonly IRepository<TextMessage> _textMessageRepo;
    private readonly IRepository<BusinessConversation> _businessConversationRepo;
    private readonly IRepository<BusinessMessageSettings> _businessSettingRepo;
    private readonly IMapper _mapper;
    private readonly IOutboundMesageService _outboundMesageService;
    
    public TextBusinessMessageService(
        IRepository<Business> businessRepo,
        IRepository<BusinessMessage> businessMessageRepo,
        IRepository<TextMessage> textMessageRepo, 
        IRepository<BusinessConversation> businessConversationRepo, 
        IRepository<BusinessMessageSettings> businessSettingRepo, 
        IMapper mapper,
        IOutboundMesageService outboundMesageService)
    {
        _businessRepo = businessRepo;
        _businessMessageRepo = businessMessageRepo;
        _textMessageRepo = textMessageRepo;
        _businessConversationRepo = businessConversationRepo;
        _businessSettingRepo = businessSettingRepo;
        _mapper = mapper;
        _outboundMesageService = outboundMesageService;
    }
    #endregion
    
    // get the message type => text
    public string GetMessageType => EMessageType.Text.ToString().ToLower();
    
    /// <summary>
    /// Create text based business message 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="RestException"></exception>
    public async Task<SuccessResponse<BusinessMessageDto<BaseInteractiveDto>>> 
        CreateMessage(CreateBusinessMessageDto<BaseCreateMessageDto> model)
    {
        //model.MessageType = EMessageType.Text.ToString();
        
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
        
        var bizMessage = _mapper.Map<BusinessMessage>(model);
        bizMessage.MessageType = EMessageType.Text.ToString();
        bizMessage.BusinessId = model.BusinessId;

        await _businessMessageRepo.AddAsync(bizMessage);

        if (model?.MessageTypeObject != null)
        {
            var textMessage = _mapper.Map<TextMessage>(model.MessageTypeObject);
            textMessage.BusinessMessageId = bizMessage.Id;
            await _textMessageRepo.AddAsync(textMessage);

            bizMessage.InteractiveMessageId = textMessage.Id;

        }

        await _businessMessageRepo.SaveChangesAsync();

        BusinessMessageDto<BaseInteractiveDto> businessMessage = await RetrieveBusinessMessageById(bizMessage.Id);

        return new SuccessResponse<BusinessMessageDto<BaseInteractiveDto>>
        {
            Data = businessMessage,
            Message = "Successfully created",
            Success = true
        };
    }

    /// <summary>
    /// Get the next business message 
    /// </summary>
    /// <param name="interactiveMessage"></param>
    /// <param name="businessId"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<(SuccessResponse<BusinessMessage> response, bool isTriggerForm)> GetNextBusinessMessageByOptionId(BaseInteractiveDto interactiveMessage, 
        Guid businessId, string option, Guid? formId =null)
    {
        BusinessMessage businessMessage = null;
        bool isTriggerForm = false;

        if (interactiveMessage is not null && interactiveMessage.IsResponsePermitted)
        {
            var responseLookOutKeys = interactiveMessage.KeyResponses?.Split(',');
            if (responseLookOutKeys is not null 
                && !string.IsNullOrWhiteSpace(option) 
                && responseLookOutKeys.Contains(option.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                businessMessage = await _businessMessageRepo.FirstOrDefault(x=>
                    x.BusinessId == businessId && x.Position == interactiveMessage.NextMessagePosition && x.BusinessFormId == formId);
                
                if (businessMessage is null)
                    throw new BusinessMessageException(
                        "Your text response is not in the list of expected responses ", null);
            }
        }

        if (formId != null)
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
    /// Get the business message of type text.
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

    public async Task<BaseInteractiveDto> GetInteractiveMessageById(Guid interactiveMessageId, string messageType)
    {
        TextMessageDto interactiveMessageDto = null;
        if (string.Equals(messageType, EMessageType.Text.ToString(), StringComparison.CurrentCultureIgnoreCase))
        {
            TextMessage textMessage = await _textMessageRepo
                .FirstOrDefault(x => x.Id == interactiveMessageId);

            if (textMessage is not null)
            {
                interactiveMessageDto = _mapper.Map<TextMessageDto>(textMessage);
                interactiveMessageDto.IsResponsePermitted = textMessage.IsResponsePermitted;
                interactiveMessageDto.KeyResponses = textMessage.KeyResponses;
            }
        }
        else
        {
            throw new InvalidOperationException($"The resolved implementation cannot handle this Message Type {messageType}");
        }
        return interactiveMessageDto;
    }

    /// <summary>
    /// Send a text-based message to 360 over Http
    /// </summary>
    /// <param name="waId"></param>
    /// <param name="request"></param>
    /// <param name="inboundMessage"></param>
    /// <returns></returns>
    public async Task<SuccessResponse<bool>> HttpSendBusinessMessage(string waId,
        BusinessMessageDto<BaseInteractiveDto> request, InboundMessage inboundMessage)
    {
        return await _outboundMesageService.HttpSendTextMessage(wa_Id: waId, model: request, 
            inboundMessage: inboundMessage);
    }

    #region  Reuseables
    // retrieve a business message by Id
    async Task<BusinessMessageDto<BaseInteractiveDto>> RetrieveBusinessMessageById(Guid id)
    {
        //var businessMessageResponse = new BusinessMessageDto<T>();
        var businessMessage = await _businessMessageRepo.FirstOrDefault(x => x.Id == id);

        if (businessMessage is null)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.BusinessNotFound);

        var textMessage =
            await _textMessageRepo.FirstOrDefault(x => x.Id == businessMessage.InteractiveMessageId);

        var mappedBusiness = new BusinessMessageDto<BaseInteractiveDto>
        {
            Id = businessMessage.Id,
            BusinessId = businessMessage.BusinessId,
            MessageType = businessMessage.MessageType,
            Name = businessMessage.Name,
            Position = businessMessage.Position,
            RecipientType = businessMessage.RecipientType,
            MessageTypeObject = textMessage != null ? _mapper.Map<TextMessageDto>(textMessage): default,
            ShouldTriggerFormProcessing = businessMessage.ShouldTriggerFormProcessing,
            BusinessFormId = businessMessage.BusinessFormId,
            BusinessConversationId = businessMessage.BusinessConversationId
        };

        return mappedBusiness;
    }
    
    #endregion
}