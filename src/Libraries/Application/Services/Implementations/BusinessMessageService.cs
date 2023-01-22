using System;
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

namespace Application.Services.Implementations;

public class BusinessMessageService
{
    private readonly IRepository<Business> _businessRepo;
    private readonly IRepository<BusinessMessage> _businessMessageRepo;
    private readonly IRepository<ReplyButtonMessage> _replyButtonMessageRepo;
    private readonly IRepository<ListMessage> _listMessageRepo;
    private readonly IRepository<TextMessage> _textMessageRepo;
    private readonly IRepository<BusinessConversation> _businessConversationRepo;
    private readonly IRepository<BusinessMessageSettings> _businessSettingRepo;
    private readonly IMapper _mapper;

    public BusinessMessageService(
        IRepository<Business> business,
        IRepository<BusinessMessage> businessMessageRepo,
        IRepository<ReplyButtonMessage> replyButtonMessageRepo,
        IRepository<ListMessage> listMessageRepo,
        IRepository<TextMessage> textMessageRepo,
        IRepository<BusinessMessageSettings> businessSetting,
        IMapper mapper, 
        IRepository<BusinessConversation> businessConversationRepo)
    {
        _businessRepo = business;
        _businessMessageRepo = businessMessageRepo;
        _replyButtonMessageRepo = replyButtonMessageRepo;
        _listMessageRepo = listMessageRepo;
        _textMessageRepo = textMessageRepo;
        _businessSettingRepo = businessSetting;
        _mapper = mapper;
        _businessConversationRepo = businessConversationRepo;
    }

    public async Task<SuccessResponse<BusinessMessageDto<ButtonMessageDto>>> 
        CreateButtonMessage(CreateBusinessMessageDto<CreateButtonMessageDto> model)
    {
        //model.MessageType = EMessageType.Button.ToString();
        var checkBizId = await _businessRepo.ExistsAsync(x => x.Id == model.BusinessId);
        if (!checkBizId)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.IncorrectId);

        var checkBizMessId = await _businessSettingRepo.FirstOrDefault(x => x.BusinessId == model.BusinessId);
        if(checkBizMessId == null)
            throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.BusinessSettingsNotFound);

        var bizMessage = _mapper.Map<BusinessMessage>(model);
        bizMessage.MessageType = EMessageType.Button.ToString();
        var replyBizMessage = _mapper.Map<ReplyButtonMessage>(model.MessageTypeObject);

        bizMessage.BusinessId = model.BusinessId;
        var saveBusinessMessage = await SaveBusinessMessage(bizMessage);
        replyBizMessage.BusinessMessageId = saveBusinessMessage.Id;

        await _replyButtonMessageRepo.AddAsync(replyBizMessage);
        await _replyButtonMessageRepo.SaveChangesAsync();

        saveBusinessMessage.InteractiveMessageId = replyBizMessage.Id;
        await UpdateBusinessMessage(saveBusinessMessage);

        var businessMessage = await RetrieveBusinessMessageById<ButtonMessageDto>(bizMessage.Id);

        return new SuccessResponse<BusinessMessageDto<ButtonMessageDto>>
        {
            Data = businessMessage,
            Message = "Successfully created",
            Success = true
        };
    }

    public async Task<SuccessResponse<BusinessMessageDto<ListMessageDto>>> 
        CreateListMessage(CreateBusinessMessageDto<CreateListMessageDto> model)
    {
        var checkBizId = await _businessRepo.ExistsAsync(x => x.Id == model.BusinessId);
        if (!checkBizId)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.IncorrectId);

        var checkBizMessId = await _businessSettingRepo.FirstOrDefault(x => x.BusinessId == model.BusinessId);
        if (checkBizMessId == null)
            throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.BusinessSettingsNotFound);

        var bizMessage = _mapper.Map<BusinessMessage>(model);
        var listBizMessage = _mapper.Map<ListMessage>(model.MessageTypeObject);

        await _businessMessageRepo.AddAsync(bizMessage);
        listBizMessage.BusinessMessageId = bizMessage.Id;
        await _listMessageRepo.AddAsync(listBizMessage);
        await _listMessageRepo.SaveChangesAsync();

        var businessMessage = await RetrieveBusinessMessageById<ListMessageDto>(bizMessage.Id);
        return new SuccessResponse<BusinessMessageDto<ListMessageDto>>
        {
            Data = businessMessage,
            Message = "Successfully created",
            Success = true
        };
    }

    // Text
    public async Task<SuccessResponse<BusinessMessageDto<TextMessageDto>>> 
        CreateTextMessage(CreateBusinessMessageDto<CreateTextMessageDto> model)
    {
        var checkBizId = await _businessRepo.ExistsAsync(x => x.Id == model.BusinessId);
        if (!checkBizId)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.IncorrectId);

        var checkBizMessId = await _businessSettingRepo.FirstOrDefault(x => x.BusinessId == model.BusinessId);
        if (checkBizMessId == null)
            throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.BusinessSettingsNotFound);

        var bizMessage = _mapper.Map<BusinessMessage>(model);
        var textBizMessage = _mapper.Map<TextMessage>(model);

        await _businessMessageRepo.AddAsync(bizMessage);
        textBizMessage.BusinessMessageId = bizMessage.Id;
        await _textMessageRepo.AddAsync(textBizMessage);
        await _textMessageRepo.SaveChangesAsync();

        var businessMessage = await RetrieveBusinessMessageById<TextMessageDto>(bizMessage.Id);

        return new SuccessResponse<BusinessMessageDto<TextMessageDto>>
        {
            Data = businessMessage,
            Message = "Successfully created",
            Success = true
        };
    }

    public async Task<SuccessResponse<BusinessMessageDto<T>>> GetBusinessMessageById<T>(Guid id)
    {
        var businessMessage = await RetrieveBusinessMessageById<T>(id);
        return new SuccessResponse<BusinessMessageDto<T>>
        {
            Data = businessMessage,
            Message = "Successfully retrieved",
            Success = true
        };
    }
    
    #region Reuseables
    async Task<dynamic> RetrieveBusinessMessageById<T>(Guid id)
    {
        //var businessMessageResponse = new BusinessMessageDto<T>();
        var businessMessage = await _businessMessageRepo.FirstOrDefault(x => x.Id == id);

        if (businessMessage is null)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.BusinessNotFound);
        var mappedBusiness = new BusinessMessageDto<T>()
        {
            Id = businessMessage.Id,
            BusinessId = businessMessage.BusinessId,
            MessageType = businessMessage.MessageType,
            Name = businessMessage.Name,
            Position = businessMessage.Position,
            RecipientType = businessMessage.RecipientType
        };

        var businessMsgType = await GetBusinessType<T>(businessMessage);
        
        if(businessMsgType.GetType() == typeof(ButtonMessageDto))
        {
            mappedBusiness.MessageTypeObject = (T)businessMsgType;
        }
        if (businessMsgType.GetType() == typeof(ListMessageDto))
        {
            mappedBusiness.MessageTypeObject = (T)businessMsgType;
        }
        if (businessMsgType.GetType() == typeof(TextMessageDto))
        {
            mappedBusiness.MessageTypeObject = (T)businessMsgType;
        }
        
        // if it got here , then value could not be resolved.
        return mappedBusiness;
    }
    async Task<object> GetBusinessType<T>(BusinessMessage businessMessage)
    {
        if(businessMessage.MessageType == EMessageType.List.ToString())
        {
            var listType = await _listMessageRepo.FirstOrDefault(x => x.Id == businessMessage.InteractiveMessageId);
            if (listType is null) { return default; }

            var listMessage = _mapper.Map<ListMessageDto>(listType);
            return listType as ListMessage;
        }
        else if (businessMessage.MessageType == EMessageType.Text.ToString())
        {
            var textMessage = await _textMessageRepo.FirstOrDefault(x => x.Id == businessMessage.InteractiveMessageId);
            if (textMessage is null) { return default; }

            var textMessageDto = _mapper.Map<TextMessageDto>(textMessage); 
            return textMessageDto;
        }
        else if (businessMessage.MessageType == EMessageType.Button.ToString())
        {
            var replyButton = await _replyButtonMessageRepo.FirstOrDefault(x => x.Id == businessMessage.InteractiveMessageId);
            if(replyButton is null) { return default; }

            var dto = new ButtonMessageDto
            {

            };


            var replyButtonMessage = _mapper.Map<ButtonMessageDto>(replyButton);
            return replyButtonMessage;
        }
        else
        {
            return default;
        }
    }

    async Task<BusinessMessage> SaveBusinessMessage(BusinessMessage model)
    {
        if (model is null) throw new RestException(System.Net.HttpStatusCode.BadGateway, "Validation failed");

        await _businessMessageRepo.AddAsync(model);
        await _businessMessageRepo.SaveChangesAsync();

        return model;
    }

    async Task UpdateBusinessMessage(BusinessMessage model)
    {
        if (model is null) throw new RestException(System.Net.HttpStatusCode.BadGateway, "Validation failed");

         _businessMessageRepo.Update(model);
        await _businessMessageRepo.SaveChangesAsync();
    }
    #endregion
    
    public async Task<SuccessResponse<bool>> UpdateListMessage(Guid businessMessageId, UpdateBusinessMessageDto<UpdateListMessageDto> model)
    {
        var checkBizMessageId = await _businessMessageRepo.GetByIdAsync(businessMessageId);
        if (checkBizMessageId is null)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.BusinessMessageNotFound);

        var list = await _listMessageRepo.GetByIdAsync(model.MessageTypeObject.Id);
        if (list is null)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.BusinessMessageNotFound);

        var checkBizId = await _businessRepo.ExistsAsync(x => x.Id == model.BusinessId);
        if (!checkBizId)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.IncorrectId);

        var checkBizMessId = await _businessSettingRepo.FirstOrDefault(x => x.BusinessId == model.BusinessId);

        if (/*string.IsNullOrEmpty(model.MessageType) || */model.BusinessId == Guid.Empty || model.Position == 0)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.IncorrectId);

        checkBizMessageId.MessageType = EMessageType.List.ToString();
        checkBizMessageId.Position = model.Position > 0 ? model.Position : checkBizMessageId.Position;
        checkBizMessageId.Name = !string.IsNullOrEmpty(model.Name) ? model.Name : checkBizMessageId.Name;

        list.Header = !string.IsNullOrEmpty(model.MessageTypeObject.Header)
         ? model.MessageTypeObject.Header : list.Header;
        list.Footer = !string.IsNullOrEmpty(model.MessageTypeObject.Footer)
            ? model.MessageTypeObject.Footer : list.Footer;
        list.ButtonMessage = !string.IsNullOrEmpty(model.MessageTypeObject.ListAction.Button)
            ? model.MessageTypeObject.ListAction.Button : list.ButtonMessage;
        list.Body = !string.IsNullOrEmpty(model.MessageTypeObject.Body)
            ? model.MessageTypeObject.Body : list.Body;

        _businessMessageRepo.Update(checkBizMessageId);
        await _businessMessageRepo.SaveChangesAsync();

        _listMessageRepo.Update(list);
        await _listMessageRepo.SaveChangesAsync();

        return new SuccessResponse<bool>
        {
            Success = true,
            Data = true,
            Message = "Successfully updated"
        };
    }

    public async Task<SuccessResponse<bool>> UpdateTextMessage(Guid businessMessageId, UpdateBusinessMessageDto<UpdateTextMessageDto> model)
    {
        var checkBizMessageId = await _businessMessageRepo.GetByIdAsync(businessMessageId);
        if (checkBizMessageId is null)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.BusinessMessageNotFound);

        var text = await _textMessageRepo.GetByIdAsync(model.MessageTypeObject.Id);
        if (text is null)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.TextMessageNotFound);

        var checkBizId = await _businessRepo.ExistsAsync(x => x.Id == model.BusinessId);
        if (!checkBizId)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.BusinessNotFound);

        var checkBizMessId = await _businessSettingRepo.FirstOrDefault(x => x.BusinessId == model.BusinessId);

        if (/*string.IsNullOrEmpty(model.MessageType) || */model.BusinessId == Guid.Empty || model.Position == 0)
            throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.IncorrectId);

        checkBizMessageId.MessageType = EMessageType.Text.ToString();
        checkBizMessageId.Position = model.Position > 0 ? model.Position : checkBizMessageId.Position;
        checkBizMessageId.Name = !string.IsNullOrEmpty(model.Name) ? model.Name : checkBizMessageId.Name;

        text.Body = model.MessageTypeObject.Body;
        text.Footer = model.MessageTypeObject.Footer;
        text.Header = model.MessageTypeObject?.Header;

        _businessMessageRepo.Update(checkBizMessageId);
        await _businessMessageRepo.SaveChangesAsync();

        _textMessageRepo.Update(text);
        await _textMessageRepo.SaveChangesAsync();

        return new SuccessResponse<bool>
        {
            Success = true,
            Data = true,
            Message = "Successfully updated"
        };
    }
}