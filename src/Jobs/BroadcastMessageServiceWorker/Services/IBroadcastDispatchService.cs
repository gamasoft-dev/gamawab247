using System;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Entities.FormProcessing;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Repositories.Interfaces;

namespace BroadcastMessageServiceWorker.Services
{
    public class BroadcastDispatchService : IBroadcastDispatchService
    {
        private readonly IRepository<BroadcastMessage> _broadcastMessageRepo;
        private readonly IOutboundMesageService _outboundMesageService;
        private readonly IMailService _mailService;
        private readonly IEmailTemplateService _emailTemplateService;

        public BroadcastDispatchService(IRepository<BroadcastMessage> broadcastMessageRepo, IOutboundMesageService outboundMesageService, IMailService mailService, IEmailTemplateService emailTemplateService)
        {
            _broadcastMessageRepo = broadcastMessageRepo;
            _outboundMesageService = outboundMesageService;
            _mailService = mailService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task SendMessage()
        {
            // get paginated list of broacast messages on pending by order of FIFO using the createdTime
            //var pendingBroadcastMessage = _broadcastMessageRepo
            //    .Query(x => x.Status == EBroadcastMessageStatus.Pending).OrderBy(x => x.CreatedAt).ToList();

            var pendingBroadcastMessage = _broadcastMessageRepo
                .Query(x => x.Status == EBroadcastMessageStatus.Failed && x.CreatedAt > DateTime.UtcNow.AddDays(-2)).OrderBy(x => x.CreatedAt).ToList();



            // iterate through the list and process message sending as below
            foreach (var broadcastMessage in pendingBroadcastMessage)
            {
                // format the message value of the broadcast message entity

                try
                {
                    var formRequest = new FormRequestResponse();
                    formRequest.From = broadcastMessage.From;
                    formRequest.To = broadcastMessage.To;
                    formRequest.Message = broadcastMessage.Message;
                    formRequest.MessageType = EMessageType.Text.ToString();
                    formRequest.BusinessId = broadcastMessage.BusinessId;

                    // update the broadcast message status as processing once send
                    broadcastMessage.Status = EBroadcastMessageStatus.Processing;
                    await _broadcastMessageRepo.SaveChangesAsync();

                    
                    if (!string.IsNullOrEmpty(broadcastMessage.EmailAddress))
                    {
                        var emailMessage = _emailTemplateService.GetReceiptBroadcastEmailTemplate(broadcastMessage.FullName, broadcastMessage.Message);

                        var emailSendSuccess = await _mailService.SendSingleMail(broadcastMessage.EmailAddress, emailMessage, "LUC Payment Receipt");

                        if (emailSendSuccess)
                        {
                            broadcastMessage.Status = EBroadcastMessageStatus.Sent;
                        }
                        else
                        {
                            throw new BackgroundException("An error occured while sending mail.");
                        }
                    }

                    // call the httpSendMessage service to send text based message;
                    //check that there is is receivers number before sending an http request
                    if (!string.IsNullOrEmpty(formRequest.To))
                    {
                        var outboundBroadcast = await _outboundMesageService.HttpSendTextMessage(formRequest.To, formRequest);
                        if (outboundBroadcast.Data)
                        {
                            broadcastMessage.Status = EBroadcastMessageStatus.Sent;
                        }
                        else
                        {
                            throw new BackgroundException("invalid phone number");
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    broadcastMessage.ErrorMessage = ex.Message;
                    broadcastMessage.Status = EBroadcastMessageStatus.Failed;
                }
                finally
                {
                    broadcastMessage.UpdatedAt = DateTime.Now;
                    await _broadcastMessageRepo.SaveChangesAsync();
                }

            }
        }



    }
    public interface IBroadcastDispatchService
    {
        Task SendMessage();
    }
}

