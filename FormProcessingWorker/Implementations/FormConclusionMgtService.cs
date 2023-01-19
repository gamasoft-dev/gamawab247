using System;
using Application.Exceptions;
using Application.Helpers;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Entities.FormProcessing;
using Domain.Enums;

namespace FormProcessingWorker
{
    public class FormConclusionMgtService: IFormConclusionMgtService
    {
        private readonly IOutboundMesageService _outboundMesageService;
        private readonly IBusinessMessageFactory _businessMessageFactory;

        public FormConclusionMgtService(IOutboundMesageService outboundMesageService,
            IBusinessMessageFactory businessMessageFactory)
        {
            _outboundMesageService = outboundMesageService;
            _businessMessageFactory = businessMessageFactory;
        }

        public async Task SaveConclusiveInboundMessage(string waId, string messageType,
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
    }

    public interface IFormConclusionMgtService : IAutoDependencyService
    {
        Task SaveConclusiveInboundMessage(string waId, string messageType,
            BusinessForm businessForm, DialogSession session);
    }
}

