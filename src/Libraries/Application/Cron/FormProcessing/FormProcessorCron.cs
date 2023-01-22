using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Sessions;
using Application.Exceptions;
using Application.Helpers;
using Application.Services.Implementations.FormProcessing;
using Application.Services.Interfaces;
using Application.Services.Interfaces.FormProcessing;
using Domain.Entities;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.FormProcessing;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Cron
{
    public class FormProcessorCron : IFormProcessorCron
    {
        private readonly IFormRequestResponseService _formRequesResponseService;
        private readonly IRepository<BusinessForm> _businessFormRepo;
        private readonly IRepository<BusinessMessage> _businessMessageRepo;
        private readonly ISessionManagement _sessionManagement;
        private readonly ILogger<FormProcessorCron> _logger;
        private readonly IOutboundMesageService _outboundMesageService;
        private readonly IDuplicateFIlterHelper _duplicateFIlterHelper;
        private readonly IFormConclusionMgtService _formConclusionMgtService;

        public FormProcessorCron(IFormRequestResponseService formRequesResponseService,
           IRepository<BusinessForm> businessFormRepo,
           ISessionManagement sessionManagement,
           ILogger<FormProcessorCron> logger,
           IRepository<BusinessMessage> businessMessageRepo,
           IOutboundMesageService outboundMesageService,
           IDuplicateFIlterHelper duplicateFIlterHelper,
           IFormConclusionMgtService formConclusionMgtService)
        {
            _formRequesResponseService = formRequesResponseService;
            _businessFormRepo = businessFormRepo;
            _sessionManagement = sessionManagement;
            _formConclusionMgtService = formConclusionMgtService;
            _logger = logger;
            _outboundMesageService = outboundMesageService;
            _duplicateFIlterHelper = duplicateFIlterHelper;
            _businessMessageRepo = businessMessageRepo;
        }

        public async Task DoWork()
        {
            try
            {
                // get the list of form request that are pending and are within the hour
                var pending = EResponseProcessingStatus.Pending.ToString();
                Expression<Func<FormRequestResponse, bool>> func = (x => x.Status == pending
                && x.Direction == EMessageDirection.Outbound.ToString());

                var formRequests = _formRequesResponseService.GetAll(func, 0, 20, true);
                bool shouldPerformSessionFinalUpdate = false;

                if (formRequests.Any())
                {
                    foreach (var item in formRequests)
                    {
                        item.Status = EResponseProcessingStatus.Pending.ToString();
                        item.UpdatedAt = DateTime.UtcNow;
                        await _formRequesResponseService.Update(item);
                        DialogSession dialogSession = await _sessionManagement.GetByWaId(item.To.Trim());

                        try
                        {
                            // check the session for the state of things
                       
                            if(dialogSession is null)
                            {
                                item.Status = EResponseProcessingStatus.SessionExpired.ToString();
                                item.ErrorMessage = $"Could not retrieve session details for the user with phone: {item.To}";

                                throw new FormBgProcessorException(item.ErrorMessage, item.Status);
                            }

                            if ((dialogSession.SessionState == ESessionState.FORMCONVOABOUTTOSTART
                                || dialogSession.SessionState == ESessionState.FORMCONVORUNNING))
                            {
                                var currentFormElement = item?.BusinessForm.FormElements.FirstOrDefault(x => x.Key == item.FormElement);

                                if(currentFormElement is null)
                                {
                                    item.Status = EResponseProcessingStatus.Failed.ToString();
                                    item.ErrorMessage = $"Form Configuration Error : Could not retrieve the form request with name {item.FormElement} from the business form record";
                                    throw new FormBgProcessorException(item.ErrorMessage, item.Status);
                                }

                                bool isSent = false; int count = 1;
                                while (!isSent && count <= 2)
                                {
                                    if (!item.IsValidationResponse)
                                    {
                                        dialogSession.SessionState = ESessionState.FORMCONVORUNNING;
                                        dialogSession.SessionFormDetails.IsFormQuestionSent = true;
                                        dialogSession.SessionFormDetails.Payload += $"{Environment.NewLine} {currentFormElement.Id}" +
                                            $" {currentFormElement.Label}: ";
                                        dialogSession.SessionFormDetails.IsValidationRequired = currentFormElement.IsValidationRequired;
                                        dialogSession.UpdatedAt = DateTime.Now;

                                    }
                                       
                                    var sendMessageResult = await _outboundMesageService.HttpSendTextMessage(model: item, wa_Id: item.To);
                                    isSent = sendMessageResult.Data;
                                    count++;
                                }

                                if (isSent)
                                {
                                    await _sessionManagement.Update(waId: item.To, dialogSession: dialogSession);
                                    item.Status = EResponseProcessingStatus.Sent.ToString();
                                }
                                else
                                {
                                    item.Status = EResponseProcessingStatus.Failed.ToString();
                                    item.ErrorMessage = $"HttpError: Could not send form request with label : {currentFormElement.Label}";
                                    throw new FormBgProcessorException(item.ErrorMessage, item.Status);
                                }

                                if (dialogSession.SessionFormDetails.IsFormCompleted && item.IsSummaryMessage)
                                {
                                    item.ErrorMessage = "Form completed. Summary message sent";
                                    item.Status = EResponseProcessingStatus.ProcessCompleted.ToString();
                                    dialogSession.SessionState = ESessionState.FORMCONVERSATIONCOMPLETED;

                                    // save an inbound at this point of the business message that conclude a form.
                                    shouldPerformSessionFinalUpdate = true;
                                    throw new FormBgProcessorException(item.ErrorMessage, item.Status);

                                }
                            }
                            else
                            {
                                throw new FormBgProcessorException("The user's dialog is not in a form processing state. Reference dialog session",
                                    EResponseProcessingStatus.InValidSessionState.ToString());
                            }
                        }
                        catch (FormBgProcessorException e)
                        {
                            item.Status = !string.IsNullOrEmpty(e.FormProcessingStatus) ?
                                e.FormProcessingStatus : EResponseProcessingStatus.Failed.ToString();

                            item.ErrorMessage = e.Message;
                            
                        }
                        catch (Exception e)
                        {
                            item.Status = EResponseProcessingStatus.Failed.ToString();
                            item.ErrorMessage = e.Message;

                        }
                        finally
                        {
                            var sessionBusinessForm = dialogSession.SessionFormDetails?.BusinessForm;

                            item.Status = item.Status;
                            item.ErrorMessage = item.ErrorMessage;
                            await _formRequesResponseService.Update(item);

                            if(shouldPerformSessionFinalUpdate)
                                await _sessionManagement.Update(waId: item.To, dialogSession: dialogSession);

                            if(dialogSession.SessionState == ESessionState.FORMCONVERSATIONCOMPLETED)
                            {
                                // check and queue conclusion inbound message tied to the business form if any.
                                if(sessionBusinessForm.ConclusionBusinessMessageId is not null)
                                {
                                    var businessMessage = await _businessMessageRepo.GetByIdAsync(sessionBusinessForm.ConclusionBusinessMessageId.Value);

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}

