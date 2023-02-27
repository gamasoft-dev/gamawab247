using Application.Common.Sessions;
using Application.DTOs;
using Application.DTOs.CreateDialogDtos;
using Application.Helpers;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    private readonly IDuplicateFIlterHelper _duplicateFIlterHelper;

    public ResponsePreProcessingCron(IRepository<InboundMessage> inboundMessageRepo,
        IRepository<OutboundMessage> outboundMessageRepo,
        IRepository<BusinessMessage> businessMessageRepo,
        IBusinessMessageFactory businessMessageFactory,
        IMapper mapper,
        IOutboundMesageService outboundMesageService,
        IOptions<SystemSettingsConfig> systemSettings,
        ISessionManagement sessionManagement,
        Application.Services.Interfaces.FormProcessing.IBusinessFormService businessFormService,
        //  IMessagePositionPhraseMapService messagePositionPhraseMapService,
        IUtilService utilService,
        IFormRequestResponseService formRequestResponseService,
        IDuplicateFIlterHelper duplicateFIlterHelper)
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
        _duplicateFIlterHelper = duplicateFIlterHelper;
    }

    public async Task InitiateMessageProcessing()
    {
        var now = DateTime.UtcNow;

        // get the list of pending inbound messages for processing.
        var pendingInbounds = await _inboundMessageRepo.Query(
            x => x.ResponseProcessingStatus.ToLower() ==
                 EResponseProcessingStatus.Pending.ToString().ToLower()
                 && x.CreatedAt < now).Skip(0).Take(20).ToListAsync();

        //Dictionary<string, InboundMessage> inboundProc = new Dictionary<string, InboundMessage>();

        if (pendingInbounds.Any())
        {
            //inboundProc = await _duplicateFIlterHelper.AddAndRetrieve<InboundMessage, InboundMessage>(pendingInbounds, inboundProc);
            foreach (var pendingInbound in pendingInbounds.Distinct())
            {
                #region Initializations
                bool sendFirstOrDefaultBusinessMessage = true;
                BusinessMessage businessMessage = null;
                InboundMessage inboundMessage = new();
                OutboundMessage outboundMessage = null;
                #endregion

                try
                {
                    if (!string.IsNullOrWhiteSpace(pendingInbound.ContextMessageId))
                    {
                        // get the associated out bound message being responded to if it's a reply 
                        outboundMessage = await _outboundMessageRepo.Query(x =>
                            x.WhatsAppMessageId == pendingInbound.ContextMessageId
                            && x.RecipientWhatsappId == pendingInbound.From
                            && x.CreatedAt < now.AddHours(_systemSettings.ConversationValidityPeriod))
                            .OrderByDescending(x => x.CreatedAt)
                            .LastOrDefaultAsync();

                        if (outboundMessage is null)
                        {
                            // sendFirstOrDefaultBusinessMessage 
                            sendFirstOrDefaultBusinessMessage = true;
                        }
                        if (outboundMessage is not null)
                        {
                            // sendFirstOrDefaultBusinessMessage 
                            sendFirstOrDefaultBusinessMessage = false;

                            // get the associated businessMessage
                            businessMessage = await
                                _businessMessageRepo.FirstOrDefault(x => x.Id == outboundMessage.BusinessMessageId);

                            if (businessMessage is null)
                            {
                                pendingInbound.ResponseProcessingStatus =
                                EResponseProcessingStatus.Failed.ToString().ToLower();
                                pendingInbound.UpdatedAt = DateTime.UtcNow;
                                pendingInbound.ErrorMessage = $"No associated business message found based " +
                                                                $"on the BusinessMessageId in the outbound Message," +
                                                                $"source: {nameof(InitiateMessageProcessing)}";

                                await _inboundMessageRepo.SaveChangesAsync();
                                await SendUnResolvedMessage(pendingInbound,string.Empty);
                            }
                        }
                    }

                    // update inbound message response status
                    pendingInbound.ResponseProcessingStatus =
                        EResponseProcessingStatus.Processing.ToString().ToLower();
                    pendingInbound.UpdatedAt = DateTime.UtcNow;
                    await _inboundMessageRepo.SaveChangesAsync();

                    // initiate response resolution
                    await ResolveNextMessage(pendingInbound, outboundMessage, businessMessage, sendFirstOrDefaultBusinessMessage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    pendingInbound.ErrorMessage = e.Message;
                    pendingInbound.UpdatedAt = DateTime.UtcNow;
                    pendingInbound.SendAttempt += 1;

                    pendingInbound.ResponseProcessingStatus = pendingInbound.SendAttempt < _systemSettings.MaxSendAttempt ?
                        EResponseProcessingStatus.Pending.ToString() : EResponseProcessingStatus.Failed.ToString();
                }
                finally
                {
                    await _inboundMessageRepo.SaveChangesAsync();
                    //_duplicateFIlterHelper.Remove<InboundMessage>(pendingInbound.Wa_Id, inboundProc);
                }
            }
        }
    }

    private async Task ResolveNextMessage(InboundMessage inboundMessage,
        OutboundMessage outboundMessage = null,
        BusinessMessage businessMessage = null,
        bool sendFirstOrDefaultBusinessMessage = false)
    {
        BusinessMessage resolvedBusinessMessage = null;
        BusinessMessageDto<BaseInteractiveDto> resolveMessageDto = null;
        List<BusinessMessageDto<BaseInteractiveDto>> allMessages = new();
        // bool isFormTriggered = false; //this is to ensure respective buttons are click to initiate a form for user instead of always beings triggered whenever option command is run.

        if (!inboundMessage.CanUseNLPMapping && sendFirstOrDefaultBusinessMessage)
        {
            // get the businessMessage for this business at position 1 and send
            resolvedBusinessMessage = await _businessMessageRepo.FirstOrDefault(x =>
               x.BusinessId == inboundMessage.BusinessId
               && x.Position == 1);
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

        if (outboundMessage is not null && businessMessage is not null)
        {
            // get the business and retrieve the associate next message based on inbound message option selected by user
            if (businessMessage.InteractiveMessageId != null)
            {
                var interactiveMessage = await _businessMessageFactory
                    .GetBusinessMessageImpl(businessMessage.MessageType)
                    .GetInteractiveMessageById(businessMessage.InteractiveMessageId.Value,
                        businessMessage.MessageType);

                if (interactiveMessage is not null && inboundMessage.MsgOptionId  is not null)
                {
                    var (response, isTriggerForm) = await _businessMessageFactory
                        .GetBusinessMessageImpl(businessMessage.MessageType)
                        .GetNextBusinessMessageByOptionId(interactiveMessage, businessMessage.BusinessId,
                            inboundMessage.MsgOptionId, businessMessage.BusinessFormId);

                   // resolvedBusinessMessage = nextBusinessMessage.response?.Data;
                }
            }
        }

        if (resolvedBusinessMessage is not null)
        {
            if (resolvedBusinessMessage.InteractiveMessageId != null)
            {
                var interactiveMessage = await _businessMessageFactory
                    .GetBusinessMessageImpl(resolvedBusinessMessage.MessageType)
                    .GetBusinessMessageById(resolvedBusinessMessage.Id);

                resolveMessageDto = interactiveMessage.Data;

                if (resolveMessageDto is not null)
                    allMessages.Add(resolveMessageDto);
            }

            if (resolvedBusinessMessage.FollowParentMessageId.HasValue)
            {
                #region Get Follow up messages
                //utilize recursive algo/ds here.
                resolveMessageDto.BusinessConversationId = resolvedBusinessMessage.Id;
                await GetFollowUpMessage(resolvedBusinessMessage.FollowParentMessageId.Value);
                if (_followUpMessagesWithContent.Any())
                {
                    allMessages.AddRange(_followUpMessagesWithContent);
                }
                #endregion
            }
            _followUpMessagesWithContent.Clear();
        }

        // make a call to send message
        await SendResolvedResponse(inboundMessage.From, allMessages, inboundMessage, false, inboundMessage.To);

        inboundMessage.ResponseProcessingStatus = EResponseProcessingStatus.Sent.ToString();
    }

    public async Task SendUnResolvedMessage(InboundMessage inboundMessage, string Message)
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
                    // determine if message should trigger form processing initiation
                    if (nextMessageRequest.ShouldTriggerFormProcessing)
                    {
                        await InitiateFormProcessing(waId, nextMessageRequest, businessPhoneNumber);
                    }

                    await _businessMessageFactory.GetBusinessMessageImpl(nextMessageRequest.MessageType)
                            .HttpSendBusinessMessage(waId, nextMessageRequest, inboundMessage);
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
    private async Task GetFollowUpMessage(Guid businessMessageId)
    {
        BusinessMessage businessMessage =
               await _businessMessageRepo.FirstOrDefault(x => x.Id == businessMessageId);

        if (businessMessage is not null)
        {
            // get the business and retrieve the associate next message based on inbound message option selected by user
            // get business message where ParentFollowMessageId = Id of the current message
            var followUpMessageResult = await _businessMessageFactory
                    .GetBusinessMessageImpl(businessMessage.MessageType)
                    .GetBusinessMessageById(businessMessage.Id);

            if (followUpMessageResult is not null)
                _followUpMessagesWithContent.Add(followUpMessageResult.Data);

            if (businessMessage.HasFollowUpMessage)
            {
                // Recurse
                await GetFollowUpMessage(businessMessage.FollowParentMessageId.Value);

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
        
        SessionFormDetail sessionFormDetail = new ();
        var session = await _sessionManagement.GetByWaId(waId);

        var currentFormElement = new FormElement();
        var nextFormElement = new FormElement();

        currentFormElement = businessForm.FormElements.FirstOrDefault(x => x.Id == 1)
                            ?? businessForm.FormElements.FirstOrDefault();

        // note the current form id would be the index of the next form element since its zero indexed.
        nextFormElement = businessForm.FormElements
            .FirstOrDefault(x=>x.Id == (currentFormElement.Id + 1));
        
        sessionFormDetail = new SessionFormDetail
        {
            CurrentElementId = currentFormElement.Id,
            CurrentFormElement = currentFormElement.Key,
            ValidationProcessorKey = currentFormElement.ValidationProcessorKey,
            IsValueConfirmed = false,
            IsValidationRequired = currentFormElement.IsValidationRequired,
            CurrentFormElementType = currentFormElement.KeyDataType,
            NextFormElement = nextFormElement.Key.ToString(), // gets preceeding index
            IsFormQuestionSent = false,
            IsFormCompleted = false,
            BusinessFormId = businessForm.Id,
            LastElementId = businessForm.FormElements.LastOrDefault().Id,
            BusinessForm = businessForm
        };

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
            FormElement = currentFormElement.Key,
            Direction = EMessageDirection.Outbound.ToString(),
            From = businessPhoneNumber,
            To = waId,
            BusinessId = message.BusinessId,
            MessageType = EMessageType.Text.ToString(),
            Status = EResponseProcessingStatus.Pending.ToString(),
            Message = currentFormElement.Label ?? currentFormElement.Key
        };
        await _formRequestResponseService.Create(formRequest);
    }
}