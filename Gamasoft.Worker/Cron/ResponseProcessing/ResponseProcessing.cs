using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.CreateDialogDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.DialogMessageEntitties;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gamasoft.Worker.Cron.ResponseProcessing;

public class ResponsePreProcessing : IResponsePreProcessing
{
    private readonly IRepository<InboundMessage> _inboundMessageRepo;
    private readonly IRepository<OutboundMessage> _outboundMessageRepo;
    private readonly IRepository<BusinessMessage> _businessMessageRepo;
    private readonly SystemSettingsConfig _systemSettings;
    private readonly IBusinessMessageFactory _businessMessageFactory;
    private readonly IOutboundMesageService _outboundMesageService;
    private readonly IMapper _mapper;
    private readonly IDuplicateFIlterHelper _duplicateFIlterHelper;
    public ResponsePreProcessing(IRepository<InboundMessage> inboundMessageRepo,
        IRepository<OutboundMessage> outboundMessageRepo, 
        IRepository<BusinessMessage> businessMessageRepo,
        IBusinessMessageFactory businessMessageFactory,
        IMapper mapper,
        IOutboundMesageService outboundMesageService,
        IOptions<SystemSettingsConfig> systemSettings,
        IDuplicateFIlterHelper duplicateFIlterHelper)
    {
        _inboundMessageRepo = inboundMessageRepo;
        _outboundMessageRepo = outboundMessageRepo;
        _businessMessageRepo = businessMessageRepo;
        _systemSettings = systemSettings.Value;
        _mapper = mapper;
        _outboundMesageService = outboundMesageService;
        _businessMessageFactory = businessMessageFactory;
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

        if (pendingInbounds.Any())
        {
            foreach (var pendingInbound in pendingInbounds)
            {
                #region Initializations
                bool sendFirstOrDefaultBusinessMessage = true;
                BusinessMessage businessMessage = null;
                InboundMessage inboundMessage = new();
                OutboundMessage outboundMessage = null;
                #endregion

                //// get the corresponding 
                //foreach (var pendingInbound in getInboundDict?.Values)
                //{
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

                        if (outboundMessage is not null)
                        {
                            // sendFirstOrDefaultBusinessMessage 
                            sendFirstOrDefaultBusinessMessage = false;
                        }
                        if (outboundMessage?.BusinessMessageId != Guid.Empty)
                        {
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
                                await SendUnResolvedMessage(pendingInbound);

                                throw new BackgroundJobException(
                                "No associated business message found based on the BusinessMessageId in the outbound Message",
                                source: nameof(InitiateMessageProcessing),
                                errors: null);
                            }
                        }
                        if (outboundMessage is not null && outboundMessage.BusinessMessageId is null)
                        {
                            // this message this is a reply but there is not business message tied to the initially sent message
                            pendingInbound.ResponseProcessingStatus =
                            EResponseProcessingStatus.Failed.ToString().ToLower();
                            pendingInbound.UpdatedAt = DateTime.UtcNow;
                            pendingInbound.ErrorMessage = $"Cannot process reply to an initially sent message " +
                                                            $"Where the sent message is not an initially created message, " +
                                                            $"source: {nameof(InitiateMessageProcessing)}";
                            await _inboundMessageRepo.SaveChangesAsync();
                            await SendUnResolvedMessage(pendingInbound);

                            throw new BackgroundJobException("Cannot process reply to an initially sent message" +
                                                                "Where the sent message is not an initially created message",
                                source: nameof(InitiateMessageProcessing),
                                errors: new NotImplementedException());
                        }
                    }

                    // update inbound message response status
                    pendingInbound.ResponseProcessingStatus =
                        EResponseProcessingStatus.Processing.ToString().ToLower();
                    pendingInbound.UpdatedAt = DateTime.UtcNow;
                    await _inboundMessageRepo.SaveChangesAsync();

                    // initiate response resolution
                    await ResolveNextMessage(pendingInbound, outboundMessage ?? new(), businessMessage ?? new(), sendFirstOrDefaultBusinessMessage);
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
                }
            }
        }
    }

    private async Task ResolveNextMessage(
        InboundMessage inboundMessage, 
        OutboundMessage outboundMessage,
        BusinessMessage businessMessage,
        bool sendFirstOrDefaultBusinessMessage = false
        )
    {
        BusinessMessage resolvedBusinessMessage = new();
        BusinessMessageDto<BaseInteractiveDto> resolveMessageDto = new();
        List<BusinessMessageDto<BaseInteractiveDto>> allMessages = new();

        if (sendFirstOrDefaultBusinessMessage)
        {
            // get the businessMessage for this business at position 1 and send
             resolvedBusinessMessage = await _businessMessageRepo.FirstOrDefault(x =>
                x.BusinessId == inboundMessage.BusinessId
                && x.Position == 1);
        }
        
        if (outboundMessage is not null && businessMessage is not null)
        {
            // get the business and retrieve the associate next message based on inbound message option selected by user
            if (businessMessage.InteractiveMessageId != null)
            {
                var interactiveMessage = await _businessMessageFactory
                    .GetBusinessMessageImpl(businessMessage.MessageType)
                    .GetInteractiveMessageById(businessMessage.InteractiveMessageId.Value,
                        businessMessage.MessageType);

                if(interactiveMessage is not null)
                {
                    var nextBusinessMessage = await _businessMessageFactory
                        .GetBusinessMessageImpl(businessMessage.MessageType)
                        .GetNextBusinessMessageByOptionId(interactiveMessage, businessMessage.BusinessId,
                            inboundMessage.MsgOptionId);

                    resolvedBusinessMessage = nextBusinessMessage.response?.Data;

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
                if(resolveMessageDto is not null) 
                    allMessages.Add(resolveMessageDto);
            }

            if (resolvedBusinessMessage.HasFollowUpMessage)
            {
                var followUpMessage = await GetFollowUpMessage(resolvedBusinessMessage);
                if(resolveMessageDto is not null) 
                    allMessages.Add(followUpMessage);
            }
        }

        // make a call to send message
        await SendResolvedResponse(inboundMessage.From, allMessages, inboundMessage);

        inboundMessage.ResponseProcessingStatus = EResponseProcessingStatus.Sent.ToString();
    }

    async Task SendUnResolvedMessage(InboundMessage inboundMessage)
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
                Body = $"Oops, we can't process your request at the moment.\nYou can contact our customer care.",
            },
        };

        msgPayload.Add(payload);

        await SendResolvedResponse(inboundMessage.Wa_Id, msgPayload, inboundMessage, true);

    }

    private async Task SendResolvedResponse(string waId, List<BusinessMessageDto<BaseInteractiveDto>> nextMessageRequests,
        InboundMessage inboundMessage, bool isUnResolved = false)
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
    private async Task<BusinessMessageDto<BaseInteractiveDto>> GetFollowUpMessage(
        BusinessMessage businessMessage,
        bool sendFirstOrDefaultBusinessMessage = false)
    {
        BusinessMessage resolvedBusinessMessage;
        BusinessMessageDto<BaseInteractiveDto> resolveMessageDto = new ();
        
        if (businessMessage is not null && businessMessage.HasFollowUpMessage)
        {
            // get the business and retrieve the associate next message based on inbound message option selected by user
            // get business message where ParentFollowMessageId = Id of the currency
            resolvedBusinessMessage =
                await _businessMessageRepo.FirstOrDefault(x => x.Id == businessMessage.FollowParentMessageId);

            if (resolvedBusinessMessage is not null)
            {
                var followUpMessage = await _businessMessageFactory
                    .GetBusinessMessageImpl(resolvedBusinessMessage.MessageType)
                    .GetBusinessMessageById(resolvedBusinessMessage.Id);

                resolveMessageDto = followUpMessage.Data;
            }
        }

        return resolveMessageDto;
    }
}