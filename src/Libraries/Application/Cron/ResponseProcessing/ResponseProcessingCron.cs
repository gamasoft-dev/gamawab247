using Infrastructure.Sessions;
using Application.DTOs;
using Application.DTOs.CreateDialogDtos;
using Application.Helpers;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Application.Services.Interfaces.FormProcessing;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.FormProcessing;
using Domain.Entities.FormProcessing.ValueObjects;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCustomization.Common;

namespace Application.Cron.ResponseProcessing;

public class ResponsePreProcessingCron : IResponsePreProcessingCron
{
    private readonly IRepository<InboundMessage> _inboundMessageRepo;
    private readonly IRepository<OutboundMessage> _outboundMessageRepo;
    private readonly IRepository<BusinessMessage> _businessMessageRepo;
    private readonly SystemSettingsConfig _systemSettings;
    private readonly IBusinessMessageFactory _businessMessageFactory;
    private readonly IOutboundMesageService _outboundMesageService;
    private readonly IMapper _mapper;
    private readonly IUtilService _utilService;
    private readonly IFormRequestResponseService _formRequestResponseService;
    private readonly ISessionManagement _sessionManagement;
    private readonly IBusinessFormService _businessFormService;
    

    private List<BusinessMessageDto<BaseInteractiveDto>> _followUpMessagesWithContent = new();
    private readonly IApiContentIntegrationManager _apiContentIntegrationManager;

    public ResponsePreProcessingCron(IRepository<InboundMessage> inboundMessageRepo,
        IRepository<OutboundMessage> outboundMessageRepo,
        IRepository<BusinessMessage> businessMessageRepo,
        IBusinessMessageFactory businessMessageFactory,
        IMapper mapper,
        IOutboundMesageService outboundMesageService,
        IOptions<SystemSettingsConfig> systemSettings,
        ISessionManagement sessionManagement,
        Application.Services.Interfaces.FormProcessing.IBusinessFormService businessFormService,        IUtilService utilService,
        IFormRequestResponseService formRequestResponseService,
        IApiContentIntegrationManager apiContentIntegrationManager)
    {
        _inboundMessageRepo = inboundMessageRepo;
        _outboundMessageRepo = outboundMessageRepo;
        _businessMessageRepo = businessMessageRepo;
        _systemSettings = systemSettings.Value;
        _mapper = mapper;
        _outboundMesageService = outboundMesageService;
        _businessMessageFactory = businessMessageFactory;
        _utilService = utilService;
        _sessionManagement = sessionManagement;
        _businessFormService = businessFormService;
        _formRequestResponseService = formRequestResponseService;
        _apiContentIntegrationManager = apiContentIntegrationManager;
    }

    public async Task InitiateMessageProcessing()
    {
        var now = DateTime.UtcNow;
       

        // get the list of pending inbound messages for processing.
        var pendingInbounds = await _inboundMessageRepo.Query(
            x => x.ResponseProcessingStatus.ToLower() ==
                 EResponseProcessingStatus.Pending.ToString().ToLower()
                 && x.CreatedAt < now).Skip(0).Take(20).ToListAsync();

        if (pendingInbounds.Any())
        {
            foreach (var pendingInbound in pendingInbounds.Distinct())
            {
                #region Initializations
                bool sendFirstOrDefaultBusinessMessage = false;
                BusinessMessage outBoundBusinessMessage = null;
                OutboundMessage outboundMsg = null;
                #endregion

                try
                {
                    if (!string.IsNullOrWhiteSpace(pendingInbound.ContextMessageId)
                        && !pendingInbound.BusinessIdMessageId.HasValue)
                    {
                        // get the associated out bound message being responded to if it's a reply 
                        outboundMsg = await _outboundMessageRepo.Query(x =>
                            x.WhatsAppMessageId == pendingInbound.ContextMessageId
                            && x.RecipientWhatsappId == pendingInbound.From
                            && x.CreatedAt < now.AddHours(_systemSettings.ConversationValidityPeriod))
                            .OrderByDescending(x => x.CreatedAt)
                            .LastOrDefaultAsync();

                        if (outboundMsg is null)
                            sendFirstOrDefaultBusinessMessage = true;

                        if (outboundMsg is not null)
                        {
                            // get the associated businessMessage
                            outBoundBusinessMessage = await
                                _businessMessageRepo.FirstOrDefault(x => x.Id == outboundMsg.BusinessMessageId);

                            if (outBoundBusinessMessage is null)
                            {
                                pendingInbound.ResponseProcessingStatus =
                                EResponseProcessingStatus.Failed.ToString().ToLower();
                                pendingInbound.UpdatedAt = DateTime.UtcNow;
                                pendingInbound.ErrorMessage = $"No associated business message found based " +
                                                                $"on the BusinessMessageId on the outbound Message," +
                                                                $"source: {nameof(InitiateMessageProcessing)}";

                                await _inboundMessageRepo.SaveChangesAsync();
                                await SendUnResolvedMessage(pendingInbound, string.Empty);
                            }
                        }
                    }
                    else if (pendingInbound.BusinessIdMessageId.HasValue) {
                        sendFirstOrDefaultBusinessMessage = false;
                    }
                    else{
                        sendFirstOrDefaultBusinessMessage = true;
                    }

                    pendingInbound.ResponseProcessingStatus = EResponseProcessingStatus.Processing.ToString().ToLower();
                    pendingInbound.UpdatedAt = DateTime.UtcNow;
                    await _inboundMessageRepo.SaveChangesAsync();

                    // initiate response resolution
                    await ResolveNextMessage(pendingInbound, outboundMsg,
                        outBoundBusinessMessage, sendFirstOrDefaultBusinessMessage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    pendingInbound.ErrorMessage = e.Message;
                    pendingInbound.UpdatedAt = DateTime.UtcNow;
                    pendingInbound.SendAttempt += 1;

                    pendingInbound.ResponseProcessingStatus = EResponseProcessingStatus.Failed.ToString();
                }
                finally
                {
                    await _inboundMessageRepo.SaveChangesAsync();
                }
            }
        }
    }

    private async Task ResolveNextMessage(InboundMessage inboundMsg,
        OutboundMessage outboundMsg = null,
        BusinessMessage outboundBusinessMsg = null,
        bool sendFirstOrDefaultMsg = false)
    {
        BusinessMessage nextBusinessMessageToSend = null;
        List<BusinessMessageDto<BaseInteractiveDto>> allMessages = new();
        BaseInteractiveDto interactiveMessage = null;

        var now = DateTime.UtcNow;
        DateTime? lastGreetingMessageTime = await _outboundMessageRepo.Query(x => x.BusinessId == inboundMessage.BusinessId && x.RecipientWhatsappId == inboundMessage.Wa_Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.UpdatedAt)
            .FirstOrDefaultAsync();

        if (now - lastGreetingMessageTime >= TimeSpan.FromHours(24) && !inboundMessage.CanUseNLPMapping 
            && sendFirstOrDefaultMsg)
        {
            // get the businessMessage for this business at position 1 and send
            nextBusinessMessageToSend = await _businessMessageRepo.FirstOrDefault(x =>
               x.BusinessId == inboundMsg.BusinessId
               && x.Position == 1);
        }
        else
        {
            resolvedBusinessMessage = await _businessMessageRepo.FirstOrDefault(x =>
               x.BusinessId == inboundMessage.BusinessId
               && x.Position == 200);
        }

        // when inboundMessage.CanUseNLPMapping == true.

        #region

        // Detect the language.
        // If language is not english based on response from language detector api
        // Note inbound language.
        // inteprete language to english for further processing.
        // Check the messagepostion phrases mapper.
        // -- extract the intent section of the message using pre-built message intent extraction function.
        // 
        // Does it contain key intent phrases of the user sent message
        // If it does, get the position and  use it to retrieve business message for that position
        // Pass message to the message to be sent method.

        //if (inboundMessage.CanUseNLPMapping)
        //{
        //    var utteranceIntents = await _utilService.IntentExtractor(inboundMessage.Body);
        //    var wordMappingsResult = await _messagePositionPhraseMapService.GetMessagePositionByPhrase(utteranceIntents,
        //        businessConversationId: null, businessId: inboundMessage.BusinessId);

        //    if(wordMappingsResult.Data > 0)
        //    {
        //       // get message for business at position returned.
        //    }
        //}


        #endregion

        if (outboundMsg is not null && outboundBusinessMsg is not null)
        {
            // get the business and retrieve the associate next message based on inbound message option selected by user
            if (outboundBusinessMsg.InteractiveMessageId != null)
            {
                interactiveMessage = await _businessMessageFactory
                   .GetBusinessMessageImpl(outboundBusinessMsg.MessageType)
                   .GetInteractiveMessageById(outboundBusinessMsg.InteractiveMessageId.Value,
                       outboundBusinessMsg.MessageType);

                if (interactiveMessage is not null && inboundMsg.MsgOptionId is not null)
                {
                    var (response, isTriggerForm) = await _businessMessageFactory
                        .GetBusinessMessageImpl(outboundBusinessMsg.MessageType)
                        .GetNextBusinessMessageByOptionId(interactiveMessage, outboundBusinessMsg.BusinessId,
                            inboundMsg.MsgOptionId, outboundBusinessMsg.BusinessFormId);

                    nextBusinessMessageToSend = response?.Data;
                }
            }
        }
        else if (inboundMsg.BusinessIdMessageId.HasValue)
        {
            nextBusinessMessageToSend = await _businessMessageRepo
                .FirstOrDefault(x => x.Id == inboundMsg.BusinessIdMessageId);
        }
        else
        {
            // resolvedBusinessMessage remains null and this would trigger delivery of unresolved message
        }

        if (nextBusinessMessageToSend is not null)
        {
            if (nextBusinessMessageToSend.InteractiveMessageId != null)
            {
                var nextBusinessMsg = await _businessMessageFactory
                    .GetBusinessMessageImpl(nextBusinessMessageToSend.MessageType)
                    .GetBusinessMessageById(nextBusinessMessageToSend.Id);

                if (nextBusinessMsg.Data is not null)
                    allMessages.Add(nextBusinessMsg.Data);
            }

            else if (nextBusinessMessageToSend.ShouldRetrieveContentAtRuntime)
            {
                var message = await GetExternalContentAndReturnBusinessMsg(
                    nextBusinessMessageToSend, inboundMsg.Wa_Id);

                allMessages.Add(message);
            }
            if (nextBusinessMessageToSend.FollowParentMessageId.HasValue)
            {
                await GetFollowUpMessage(nextBusinessMessageToSend.FollowParentMessageId.Value, inboundMsg.Wa_Id);

                if (_followUpMessagesWithContent.Any())
                    allMessages.AddRange(_followUpMessagesWithContent);

            }

            _followUpMessagesWithContent.Clear();
        }
        else {
            await SendUnResolvedMessage(inboundMsg, "Could not resolve your desired response at this time, " +
                "you could end session by typing 'end' or go back to the initial menu");
        }

        // make a call to send message
        await SendResolvedResponse(inboundMsg.From, allMessages, inboundMsg, false, inboundMsg.To);

        inboundMsg.ResponseProcessingStatus = EResponseProcessingStatus.Sent.ToString();
    }

    private async Task SendUnResolvedMessage(InboundMessage inboundMessage, string Message)
    {
        List<BusinessMessageDto<BaseInteractiveDto>> msgPayload = new List<BusinessMessageDto<BaseInteractiveDto>>();
        var payload = new BusinessMessageDto<BaseInteractiveDto>
        {
            BusinessId = inboundMessage.BusinessId,
            MessageType = EMessageType.Text.ToString(),
            Name = inboundMessage.Name,
            Position = 0,
            MessageTypeObject = new BaseInteractiveDto
            {
                Body = !string.IsNullOrWhiteSpace(Message)
                ?Message
                : $"Oops, we can't process your request at the moment." +
                $"\n You can contact our customer care."
            },
        };
        msgPayload.Add(payload);

        await SendResolvedResponse(inboundMessage.Wa_Id, msgPayload, inboundMessage, true, inboundMessage.To);
    }

    private async Task SendResolvedResponse(string waId, List<BusinessMessageDto<BaseInteractiveDto>> nextMessageRequests,
        InboundMessage inboundMessage, bool isUnResolved = false, string businessPhoneNumber = null)
    {
        foreach (var nextMessageRequest in nextMessageRequests)
        {
            try
            {
                if (isUnResolved)
                {
                    await _businessMessageFactory.GetBusinessMessageImpl(EMessageType.Text.ToString())
                            .HttpSendBusinessMessage(waId, nextMessageRequest, inboundMessage);
                }
                else
                {
                   
                    await _businessMessageFactory.GetBusinessMessageImpl(nextMessageRequest.MessageType)
                            .HttpSendBusinessMessage(waId, nextMessageRequest, inboundMessage);

                    // determine if message should trigger form processing initiation
                    if (nextMessageRequest.ShouldTriggerFormProcessing)
                    {
                        await InitiateFormProcessing(waId, nextMessageRequest, businessPhoneNumber);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    /// <summary>
    /// Get follow up message if existent
    /// </summary>
    /// <param name="businessMessage"></param>
    /// <param name="sendFirstOrDefaultBusinessMessage"></param>
    /// <returns></returns>
    private async Task GetFollowUpMessage(Guid businessMessageId, string waId)
    {
        BusinessMessage businessMessage =
               await _businessMessageRepo.FirstOrDefault(x => x.Id == businessMessageId);

        if (businessMessage is not null)
        {
            // get the business and retrieve the associate next message based on inbound message option selected by user
            // get business message where ParentFollowMessageId = Id of the current message

            BusinessMessageDto<BaseInteractiveDto>  followUpMessageResult= null;

            if (businessMessage.ShouldRetrieveContentAtRuntime
                && !string.IsNullOrEmpty(businessMessage.ContentRetrievalProcessorKey))
            {
                followUpMessageResult = await GetExternalContentAndReturnBusinessMsg(businessMessage, waId);
            }
            else {

                var messageData = await _businessMessageFactory
                    .GetBusinessMessageImpl(businessMessage.MessageType)
                    .GetBusinessMessageById(businessMessage.Id);

                followUpMessageResult = messageData.Data;
            }


            if (followUpMessageResult is not null)
                _followUpMessagesWithContent.Add(followUpMessageResult);

            if (businessMessage.HasFollowUpMessage && businessMessage.FollowParentMessageId.HasValue)
            {
                // Recurse
                await GetFollowUpMessage(businessMessage.FollowParentMessageId.Value, waId);

            }
        }
    }

    private async Task InitiateFormProcessing(string waId, BusinessMessageDto<BaseInteractiveDto> message, string businessPhoneNumber)
    {
        var businessForm = await _businessFormService.GetBusinessFormFisrtOrDefault(x =>
        x.Id == message.BusinessFormId.Value && x.BusinessConversationId == message.BusinessConversationId);

        if (businessForm is null) 
            throw new BackgroundJobException("The Business Form associated to the business message could not be found",
                nameof(InitiateFormProcessing), this);

        IList<FormElement> formElements = businessForm.FormElements.OrderBy(x => x.Position).ToList();

        SessionFormDetail sessionFormDetail = new ();
        var session = await _sessionManagement.GetByWaId(waId);

        sessionFormDetail.CurrentFormElement = formElements.FirstOrDefault();

        if(sessionFormDetail.CurrentFormElement.NextFormElementPosition != 0)
        {
            sessionFormDetail.NextFormElement = businessForm.FormElements?
                .FirstOrDefault(x => x.Position == sessionFormDetail
                .CurrentFormElement.NextFormElementPosition);
        }

        sessionFormDetail.IsCurrentValueConfirmed = false;
        sessionFormDetail.IsFormQuestionSent = false;
        sessionFormDetail.IsFormCompleted = false;
        sessionFormDetail.BusinessFormId = businessForm.Id;
        sessionFormDetail.BusinessForm = new BusinessFormVM().MapBusinessFormToVM(businessForm);
        

        if (session is not null)
        {
            session.SessionState = ESessionState.FORMCONVOABOUTTOSTART;
            session.UpdatedAt = DateTime.Now;
            session.SessionFormDetails = sessionFormDetail;

            await _sessionManagement.Update(waId, session);
        }
        else
        {
            throw new BackgroundJobException("Session cannot be null at this point",
                nameof(InitiateFormProcessing));
        }

        var formRequest = new FormRequestResponse
        {
            BusinessFormId = businessForm.Id,
            FormElement = sessionFormDetail?.CurrentFormElement?.Key,
            Direction = EMessageDirection.Outbound.ToString(),
            From = businessPhoneNumber,
            To = waId,
            BusinessId = message.BusinessId,
            MessageType = EMessageType.Text.ToString(),
            Status = EResponseProcessingStatus.Pending.ToString(),
            Message = sessionFormDetail?.CurrentFormElement?.Label,
            IsValidationResponse = false,
            FollowUpPartnerContentIntegrationKey = sessionFormDetail?
                            .CurrentFormElement?.PartnerContentProcessorKey,
            
        };
        await _formRequestResponseService.Create(formRequest);
    }

    /// <summary>
    /// This method retrieves the businessMessage content from an external implementation
    /// (api, local function or library). This makes use of the PartnerContentProcessorKey
    /// </summary>
    /// <param name="model"></param>
    /// <param name="waId"></param>
    /// <returns></returns>
    private async Task<BusinessMessageDto<BaseInteractiveDto>> GetExternalContentAndReturnBusinessMsg(BusinessMessage model, string waId)
    {
        string contentRetrieved = await _apiContentIntegrationManager.RetrieveContent<string>(
            model.ContentRetrievalProcessorKey, waId, "");

        return new BusinessMessageDto<BaseInteractiveDto>()
        {
            FollowParentMessageId = model.FollowParentMessageId,
            Id = model.Id,
            BusinessFormId = model.BusinessFormId,
            ShouldRetrieveContentAtRuntime = model.ShouldRetrieveContentAtRuntime,
            ContentRetrievalProcessorKey = model.ContentRetrievalProcessorKey,
            MessageType = model.MessageType,
            HasFollowUpMessage = model.HasFollowUpMessage,
            Name = model.Name,
            Position = model.Position,
            MessageTypeObject = new BaseInteractiveDto
            {
                Body = contentRetrieved,
                BusinessMessageId = model.Id,
            },
            BusinessId = model.BusinessId,
            RecipientType = model.RecipientType,
            ShouldTriggerFormProcessing = model.ShouldTriggerFormProcessing,
            BusinessConversationId = model.BusinessConversationId
        };
    private void GreetUser(object sender, ElapsedEventArgs e, BusinessMessage resolvedBusinessMessage, InboundMessage inboundMessage)
    {
        var now = DateTime.UtcNow;
        if (now.Hour == 0 && now.Minute == 0 && now.Second == 0)
            resolvedBusinessMessage = _businessMessageRepo.FirstOrDefault(x =>
              x.BusinessId == inboundMessage.BusinessId
              && x.Position == 1).Result;
    }
}