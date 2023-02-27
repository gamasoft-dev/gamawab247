using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.Exceptions;
using Application.Helpers;
using Application.Services.Implementations.FormProcessing.CompletionProcesses;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Entities.FormProcessing;
using Domain.Enums;

namespace Application.Services.Implementations.FormProcessing
{
    public class FormConclusionMgtService: IFormConclusionMgtService
    {
        private readonly IOutboundMesageService _outboundMesageService;
        private readonly IBusinessMessageFactory _businessMessageFactory;
        private readonly IEnumerable<IFormCompletionProcessor> _formCompletionProcessors;

        public FormConclusionMgtService(IOutboundMesageService outboundMesageService,
            IEnumerable<IFormCompletionProcessor> formCompletionProcessors,
            IBusinessMessageFactory businessMessageFactory)
        {
            _outboundMesageService = outboundMesageService;
            _businessMessageFactory = businessMessageFactory;
            _formCompletionProcessors = formCompletionProcessors;
        }

       
        /// <summary>
        /// Send conclusive message configured for the business form.
        /// </summary>
        /// <param name="waId"></param>
        /// <param name="messageType"></param>
        /// <param name="businessForm"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        /// <exception cref="FormBgProcessorException"></exception>
    
        public async Task SendConlusiveMessage(string waId, string messageType,
            BusinessForm businessForm,  DialogSession session)
        {
            try
            {
                if (businessForm.ConclusionBusinessMessageId != null)
                {
                    var inbound = new InboundMessage();

                    var businessMessageResult = await _businessMessageFactory.GetBusinessMessageImpl(messageType)
                                            .GetBusinessMessageById(businessForm.ConclusionBusinessMessageId.Value);

                    if (businessMessageResult is null)
                        throw new FormBgProcessorException("Require conclusive business message is null." +
                            " Trace ConclusiveBusinessMessageId on BusinessForm schema");

                    inbound.Wa_Id = waId;
                    inbound.Name = session.UserName;
                    inbound.BusinessId = session.BusinessId;
                    inbound.ResponseProcessingStatus = EResponseProcessingStatus.Pending.ToString();
                    inbound.WhatsUserName = session.UserName;

                    await _outboundMesageService.SendMessage(messageType, waId, businessMessageResult.Data, inbound);

                }
            }
            catch (Exception ex)
            {
                throw new FormBgProcessorException("Error occured whilst to send conclusive message of form data", ex);
            }
        }


        /// <summary>
        /// Perform the conclusion action based on user selection and the configured actions for the business form
        /// </summary>
        /// <param name="waId"></param>
        /// <param name="userSelectedActionId"></param>
        /// <param name="formConclusionProcessConfig"></param>
        /// <param name="dialogSession"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task DoConclusiveAction(string waId, string userSelectedActionId, BusinessFormConlusionConfig formConclusionProcessConfig, DialogSession dialogSession)
        {
            throw new NotImplementedException();
        }
    }

    public interface IFormConclusionMgtService : IAutoDependencyService
    {
        Task SendConlusiveMessage(string waId, string messageType,
            BusinessForm businessForm, DialogSession session);

        Task DoConclusiveAction(string waId, string userSelectedActionId,
            BusinessFormConlusionConfig formConclusionProcessConfig, DialogSession dialogSession);
       
    }
}

