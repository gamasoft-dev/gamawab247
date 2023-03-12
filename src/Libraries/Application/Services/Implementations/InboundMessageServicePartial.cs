using System;
using Application.DTOs;
using Application.Helpers;
using Domain.Common;
using Domain.Entities;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.DTOs.InboundMessageDto;
using Domain.Enums;
using Newtonsoft.Json;
using Domain.Entities.FormProcessing;
using System.Collections.Generic;
using Application.Exceptions;
using Domain.Entities.FormProcessing.ValueObjects;

namespace Application.Services.Implementations
{
    public partial class InboundMessageService
    {
        private async Task ReceiveTextBasedMessage(Guid id, dynamic textRequestData)
        {
           bool shouldSaveInbound = true;

            if (_InteractiveMessageType != EMessageType.Text)
                throw new RestException(HttpStatusCode.BadRequest,
                    $"This Text based implementation cannot handle this kind of message. Received-MessageType: {_BaseMessageType.ToString()}");

            TextNotificationDto model = JsonConvert.DeserializeObject<TextNotificationDto>(textRequestData.ToString());

            if (id == Guid.Empty || model == null)
                throw new RestException(HttpStatusCode.BadRequest, message:
                    $"{ResponseMessages.PayLoadCannotBeNull} or {ResponseMessages.ParameterCannotBeNull}");

            #region Section added by Richard to handle message logs...
            var messageLog = new CreateMessageLogDto
            {
                From = model.Messages.FirstOrDefault()?.From,
                To = this._Business.PhoneNumber,
                BusinessId = _Business.Id,
                IsRead = false,
                MessageBody = model.Messages.FirstOrDefault()?.text?.body,
                RequestResponseData = textRequestData.ToString(),
                MessageType = EMessageType.Text,
                MessageDirection = EMessageDirection.Inbound,
                WhatsappUserId = this._WhatsappUser.Id,
            };

            // get the partner
            // get the partner integration details
            // what is the partnerIndentifier

            // use the indentifier with Factory to get the correct api integration
            // then call the retrieve content method.

            // call the service and add messageLog

            await _messageLogService.CreateMessageLog(messageLog);
            #endregion

            var inbound = _mapper.Map<InboundMessage>(model);
            //iterate thru 2 list  of records simultaneously...

            var messages = model?.Contacts.Zip(model.Messages,
                (contact, msg) => new { contact = contact, msg = msg });

            // check session state
            DialogSession session = await _sessionManagement.GetByWaId(messageLog.From);
            if (session is not null && session.SessionState == ESessionState.FORMCONVORUNNING
                && session.SessionFormDetails.IsFormQuestionSent == true)
            {
                //save user input as response to a form question.
                await ReceiveFormResponse(messageLog, session);
                shouldSaveInbound = false;
            }


            if (shouldSaveInbound)
            {
                if (messages != null || messages.Any())
                    foreach (var item in messages)
                    {
                        inbound.WhatsAppId = item.contact.wa_id;
                        inbound.Name = item.contact.profile.name;
                        inbound.WhatsAppMessageId = item.msg.Id;
                        inbound.Body = item.msg.text.body;
                        inbound.From = item.msg.From;
                        inbound.To = this._Business.PhoneNumber;
                        inbound.Type = item.msg.Type;
                        inbound.Wa_Id = item.contact.wa_id;
                        inbound.WhatsUserName = item.contact?.profile?.name;
                        inbound.ContextMessageId = item.msg?.context?.Id;
                        inbound.MsgOptionId = item?.msg?.text?.body;
                    }
                inbound.ResponseProcessingStatus = EResponseProcessingStatus.Pending.ToString();
                inbound.BusinessId = id;

                // raise event to trigger response if response is needed.

                await _inboundMessageRepo.AddAsync(inbound);
                await _inboundMessageRepo.SaveChangesAsync();
            }
        }

        private async Task ReceiveButtonBasedMessage(Guid id, dynamic buttonRequestData)
        {
            if (_InteractiveMessageType != EMessageType.Button)
                throw new RestException(HttpStatusCode.BadRequest,
                    $"This Button based implementation cannot handle this kind of message. Received-MessageType: {_BaseMessageType.ToString()}");

            ButtonReplyNotificationDto replyNotificationDto = JsonConvert.DeserializeObject<ButtonReplyNotificationDto>(buttonRequestData.ToString());

            if (id == Guid.Empty || replyNotificationDto == null)
                throw new RestException(HttpStatusCode.BadRequest, message:
                        $"{ResponseMessages.PayLoadCannotBeNull} or {ResponseMessages.ParameterCannotBeNull}");

            var business = await _businessService.GetBusinessByBusinessId(id);
            if (business == null)
                throw new RestException(HttpStatusCode.BadRequest, $"No business with this id {id} was found to receive and process this message");

            var messageLog = new CreateMessageLogDto
            {
                From = replyNotificationDto.Messages.FirstOrDefault()?.From,
                To = this._Business.PhoneNumber,
                BusinessId = _Business.Id,
                IsRead = false,
                MessageBody = replyNotificationDto.Messages.FirstOrDefault()?.interactive?.button_reply?.title,
                RequestResponseData = JsonConvert.SerializeObject(buttonRequestData),
                MessageType = EMessageType.Button,
                MessageDirection = EMessageDirection.Inbound,
                WhatsappUserId = this._WhatsappUser.Id
            };

            // call the service and add messageLog
            await _messageLogService.CreateMessageLog(messageLog);

            var inbound = _mapper.Map<InboundMessage>(replyNotificationDto);

            var buttonMessage = replyNotificationDto.Contacts.Zip(replyNotificationDto.Messages,
           (contact, msg) => new { contact = contact, msg = msg });

            if (buttonMessage.Any())
            {
                for (int i = 0; i < buttonMessage.LongCount(); i++)
                {
                    inbound.Wa_Id = replyNotificationDto.Contacts[i].wa_id;
                    inbound.Name = replyNotificationDto.Contacts[i].profile.name;
                    inbound.Body = replyNotificationDto.Messages[i].interactive.button_reply.title;
                    inbound.Type = replyNotificationDto.Messages[i].interactive.type;
                    inbound.From = replyNotificationDto.Messages[i].From;
                    inbound.To = this._Business.PhoneNumber;
                    inbound.WhatsAppMessageId = replyNotificationDto.Messages[i].Id;
                    inbound.MsgOptionId = replyNotificationDto.Messages[i].interactive.button_reply.id;
                    inbound.Timestamp = replyNotificationDto.Messages[i].Timestamp;
                    inbound.ContextMessageId = replyNotificationDto.Messages[i].context.Id;
                    inbound.BusinessId = business.Data.Id;
                    inbound.ResponseProcessingStatus = EResponseProcessingStatus.Pending.ToString();
                    inbound.WhatsUserName = replyNotificationDto.Contacts?.FirstOrDefault()?.profile.name;
                }
                                                                       
                await _inboundMessageRepo.AddAsync(inbound);
                await _inboundMessageRepo.SaveChangesAsync();
            }
        }

        // TODO: Daniel
        private async Task ReceiveListBasedMessage(Guid id, dynamic listRequestData)
        {
            if (_InteractiveMessageType != EMessageType.List)
                throw new RestException(HttpStatusCode.BadRequest,
                    $"This List based implementation cannot handle this kind of message. Received-MessageType: {_BaseMessageType.ToString()}");

            ListReplyNotificationDto list = JsonConvert.DeserializeObject<ListReplyNotificationDto>(listRequestData.ToString());

            if (id == Guid.Empty || list == null)
                throw new RestException(HttpStatusCode.BadRequest, message:
                        $"{ResponseMessages.PayLoadCannotBeNull} or {ResponseMessages.ParameterCannotBeNull}");

            var business = await _businessService.GetBusinessByBusinessId(id);
            if (business == null)
                throw new RestException(HttpStatusCode.BadRequest, $"No business with this id {id} was found to receive and process this message");

            var messageLog = new CreateMessageLogDto
            {
                From = list.Messages.FirstOrDefault()?.From,
                To = this._Business.PhoneNumber,
                BusinessId = _Business.Id,
                IsRead = false,
                MessageBody = list.Messages.FirstOrDefault()?.Interactive?.list_reply?.description,
                RequestResponseData = JsonConvert.SerializeObject(list),
                MessageType = EMessageType.Button,
                MessageDirection = EMessageDirection.Inbound,
                WhatsappUserId = this._WhatsappUser.Id
            };

            // call the service and add messageLog
            await _messageLogService.CreateMessageLog(messageLog);

            var inbound = _mapper.Map<InboundMessage>(list);

            var listMessages = list.Contacts.Zip(list.Messages,
           (contact, msg) => new { contact = contact, msg = msg });


            if (listMessages.Any())
            {
                for (int i = 0; i < listMessages.LongCount(); i++)
                {
                    inbound.WhatsAppId = list.Contacts[i].wa_id;
                    inbound.Name = list.Contacts[i].profile.name;
                    inbound.Body = list.Messages[i].Interactive.list_reply.description;
                    inbound.Type = "list";//list.Messages[i].ButtonReply.type;
                    inbound.From = list.Messages[i].From;
                    inbound.To = this._Business.PhoneNumber;
                    inbound.WhatsAppMessageId = list.Messages[i].Id;
                    inbound.BusinessId = id;
                    inbound.ContextMessageId = list.Messages[i].context.Id != null ? list.Messages[i].context.Id : String.Empty;
                    inbound.Wa_Id = list.Contacts[i].wa_id;
                    inbound.Timestamp = list.Messages[i].Timestamp;
                    inbound.CreatedById = business.Data.CreatedById;
                    inbound.MsgOptionId = list.Messages[i].Interactive.list_reply.id;
                    inbound.BusinessId = business.Data.Id;
                    inbound.WhatsUserName = list.Contacts?.FirstOrDefault()?.profile.name;
                    inbound.ResponseProcessingStatus = EResponseProcessingStatus.Pending.ToString();
                }

                await _inboundMessageRepo.AddAsync(inbound);
                await _inboundMessageRepo.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Receieve User input for a form request
        /// </summary>
        /// <param name="messageLog"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        private async Task ReceiveFormResponse(CreateMessageLogDto messageLog, DialogSession session)
        {
            bool shouldSaveRequest = true;
            List<FormRequestResponse> formRequestResponses = new();
            string waId = messageLog.From;

            try
            {
                if (session is null || session.SessionFormDetails is null)
                {
                    throw new ProcessCancellationException(
                        "Session or Session  FormDetails " +
                        "cannot be null at this point but it is null ", (int)HttpStatusCode.Continue);
                }

                if (!session.SessionFormDetails.IsFormQuestionSent)
                {
                    throw new ProcessCancellationException(
                      "Redundant message received, but  " +
                      "Form request has not been sent yet. " +
                      "Therefore processing this may break the flow", (int)HttpStatusCode.Continue);
                }

                BusinessFormVM sessionBusinessForm = session.SessionFormDetails.BusinessForm;

                FormElement currentElement = session?.SessionFormDetails?.CurrentFormElement;
                if (currentElement is null)
                    throw new FormConfigurationException("There is not current element configured to recieve this input. " +
                        "Check that session has value for currentFormElement", false);

                FormElement nextElement = session?.SessionFormDetails?.NextFormElement;

                var receivedResponse = new FormRequestResponse
                {
                    To = messageLog.To,
                    From = messageLog.From,
                    BusinessFormId = session.SessionFormDetails.BusinessFormId,
                    BusinessId = session.BusinessId,
                    Direction = EMessageDirection.Inbound.ToString(),
                    FormElement = currentElement?.Key,
                    MessageType = EMessageType.Text.ToString(),
                    Status = EResponseProcessingStatus.Recieved.ToString(),
                    Message = messageLog.MessageBody,
                    FollowUpPartnerContentIntegrationKey = currentElement?.PartnerContentProcessorKey
                };

                // save recieved form response 
                formRequestResponses.Add(receivedResponse);

             
                // check if validationKey is present for this form element where in input was receieved
                if (!string.IsNullOrEmpty(currentElement.ValidationProcessorKey)
                    && currentElement.IsValidationRequired)
                {
                    session.SessionFormDetails.IsCurrentValueConfirmed = false;

                    try
                    {
                        _formValidationStrategyService.ValidateInput(currentElement.ValidationProcessorKey,
                      messageLog.MessageBody, currentElement.Key);

                        session.SessionFormDetails.IsCurrentValueConfirmed = true;
                    }
                    catch (FormInputValidationException e)
                    {
                        throw e;
                    }
                    catch (FormConfigurationException e)
                    {
                        throw e;
                    }
                }

                session.SessionFormDetails.IsFormResponseRecieved = true;
                session.SessionFormDetails.Payload += $"{messageLog.MessageBody} {Environment.NewLine}";

                if (nextElement is not null)
                {
                    var nextFormRequest = new FormRequestResponse
                    {
                        To = messageLog.From,
                        From = messageLog.To,
                        BusinessFormId = session.SessionFormDetails.BusinessFormId,
                        BusinessId = session.BusinessId,
                        Direction = EMessageDirection.Outbound.ToString(),
                        FormElement = nextElement.Key,
                        MessageType = EMessageType.Text.ToString(),
                        Status = EResponseProcessingStatus.Pending.ToString(),
                        Message = nextElement?.Label,
                        FollowUpPartnerContentIntegrationKey = nextElement?.PartnerContentProcessorKey
                    };

                    formRequestResponses.Add(nextFormRequest);

                    session.SessionFormDetails.IsCurrentValueConfirmed = true;
                }

                if (!session.SessionFormDetails.UserData.ContainsKey(currentElement.Key))
                {
                    session.SessionFormDetails.UserData.Add(currentElement.Key, messageLog.MessageBody);
                }

            }
            catch (FormConfigurationException wx)
            {
                if (wx.SendPromptMessageToUser)
                {
                    formRequestResponses.Add(new FormRequestResponse
                    {
                        To = messageLog.From,
                        IsValidationResponse = true,
                        From = messageLog.To,
                        BusinessFormId = session.SessionFormDetails.BusinessFormId,
                        BusinessId = session.BusinessId,
                        Direction = EMessageDirection.Outbound.ToString(),
                        FormElement = session.SessionFormDetails?.CurrentFormElement.Key,
                        MessageType = EMessageType.Text.ToString(),
                        Status = EResponseProcessingStatus.Pending.ToString(),
                        Message = wx.Message
                    });
                }
            }
            catch (ProcessCancellationException ex)
            {
                throw ex;
            }
            catch (FormInputValidationException fx)
            {
                formRequestResponses.Add(new FormRequestResponse
                {
                    To = messageLog.From,
                    IsValidationResponse = true,
                    From = messageLog.To,
                    BusinessFormId = session.SessionFormDetails.BusinessFormId,
                    BusinessId = session.BusinessId,
                    Direction = EMessageDirection.Outbound.ToString(),
                    FormElement = session.SessionFormDetails?.CurrentFormElement?.Key,
                    MessageType = EMessageType.Text.ToString(),
                    Status = EResponseProcessingStatus.Pending.ToString(),
                    Message = fx.Message,
                });
            }
            catch (Exception)
            {
                shouldSaveRequest = false;
                throw;
            }
            finally
            {
                await _sessionManagement.Update(waId: waId, session);

                if (shouldSaveRequest)
                    await _formRequestResponseService.Create(formRequestResponses);

            }
        }
    }
}