using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Application.DTOs.InboundMessageDto;
using Newtonsoft.Json;
using Application.Exceptions;
using Infrastructure.Sessions;
using Domain.Entities;
using System.Linq;

namespace Application.Services.Implementations
{
    /// <summary>
    /// This service class
    /// i. Validates a recieved message,
    /// ii. On successfull validation , it saves the message the to the db as an inbound message.
    /// iii. Raise an event to trigger conversation.
    /// iv. Retrieve best fit response based on the request keyword and message sent.
    /// To achieve this it searches the preconfigured conversation flow records based on the businessId
    /// v. If no business conversation flow record is found. Repeat (iv) based on the IndustryId of the business.
    /// vi. If none is found it retrieves the default conversation flow record for that industry based.
    /// This process is called from the webhook method which is the BusinessMessageController file in the "ProcessMessage" action method.
    /// </summary>
    public class Dialog360MessageProcessor : IMessageProcessor
    {
        private readonly ILogger<Dialog360MessageProcessor> _logger;
        private readonly IBusinessService _businessService;
		private readonly IBusinessSettingsService _businessSettingsService;
        private readonly ISessionManagement _session;

        public Dialog360MessageProcessor(ILogger<Dialog360MessageProcessor> logger,
            IBusinessSettingsService businessSettingsService,
            IBusinessService businessService, ISessionManagement session)
        {
            _logger = logger;
            _businessSettingsService = businessSettingsService;
            _businessService = businessService;
            _session = session;
          
        }

        /// <summary>
        /// Validate the inbound message received is in good shape and points to a valid business on the platform.
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task ValidateInboundMessage(Guid businessId, dynamic request)
        {
            try
            {
                if (request is null || businessId == Guid.Empty) 
                    throw new ProcessCancellationException("Not a valid inbound request, request is null or businessId param is empty");
        
                var businessResponse = await _businessService.GetBusinessByBusinessId(businessId);
                
                if(businessResponse is null && businessResponse.Data is not null)
                    throw new RestException(System.Net.HttpStatusCode.BadRequest, "No business found to process this message");
                
                var businessSettings = await _businessSettingsService.GetByBusinessId(businessId);
                if (businessSettings is null)
                    throw new RestException(System.Net.HttpStatusCode.BadRequest, "No business configuration found to process this message");

                var status = JsonConvert.DeserializeObject<WhatsAppStatusDto>(request.ToString());
                if(status is not null && status?.statuses != null)
                    throw new ProcessCancellationException("This is a message status notification, not a message");

                // remove this is session and validatoin middle ware is being used.
                _360MessageDto requestPayload = GetDialo360Message(request.ToString());


                var session = await _session.GetByWaId(requestPayload.contacts.FirstOrDefault()?.wa_id);
                var messageTime = DateTimeHelper.GetDateTimeFromTimeStamp(requestPayload.messages.FirstOrDefault()?.timestamp);

                // end session if plain text "end" is sent by user
                if (requestPayload.messages.FirstOrDefault()?.text?.body == "end")
                {
                    await _session.RemoveUserSession(requestPayload.contacts.FirstOrDefault()?.wa_id);
                    throw new ProcessCancellationException("Session was ended by user by typing the end keyword");
                }

                if ((DateTime.Now - messageTime) >= TimeSpan.FromMinutes(10))
                    throw new ProcessCancellationException("Message is redundant, coming in a later time than 10mins");

                if (session is null)
                {
                    var business = new Business() { Id = businessId };
                    session = await _session.CreateNewSession(
                        requestPayload.contacts.FirstOrDefault()?.wa_id,
                        requestPayload.messages.FirstOrDefault()?.id,
                        requestPayload.contacts.FirstOrDefault()?.profile?.name,
                        business, messageTime, null, null, null);
                }
                else

                {
                    if(session.LastInboundMessageTime != null && session.LastInboundMessageTime > messageTime)
                        throw new ProcessCancellationException("Redundant message received, Dialog already running and ahead of this received message");
                    
                    if(session.LastMessageId == requestPayload.messages.FirstOrDefault()?.id)
                        throw new ProcessCancellationException("Redundant message received, Message already received and processed");

                    // update the session last inbound message time
                    session.LastInboundMessageTime = messageTime;
                    session.LastMessageId = requestPayload.messages.FirstOrDefault()?.id;
                    session.UpdatedAt = DateTime.UtcNow;
                    await _session.Update(waId: requestPayload.contacts.FirstOrDefault()?.wa_id, session);
                }
            }
            catch(RestException rx)
            {
                throw rx;
            }
            catch (ProcessCancellationException px)
            {
                throw px;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw new RestException(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        static _360MessageDto GetDialo360Message(string request)
        {
            if (string.IsNullOrEmpty(request))
                throw new ProcessCancellationException("Empty validate request");

            return JsonConvert.DeserializeObject<_360MessageDto>(request);
        }
    }
}
