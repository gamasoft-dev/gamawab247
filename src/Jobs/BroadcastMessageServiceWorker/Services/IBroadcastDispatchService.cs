using System;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Entities.FormProcessing;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace BroadcastMessageServiceWorker.Services
{
    public class BroadcastDispatchService : IBroadcastDispatchService
    {
        private readonly IRepository<BroadcastMessage> _broadcastMessageRepo;
        private readonly IOutboundMesageService _outboundMesageService;

        public BroadcastDispatchService(IRepository<BroadcastMessage> broadcastMessageRepo, IOutboundMesageService outboundMesageService)
        {
            _broadcastMessageRepo = broadcastMessageRepo;
            _outboundMesageService = outboundMesageService;
        }

        public async Task SendMessage()
        {
            // get paginated list of broacast messages on pending by order of FIFO using the createdTime
            var pendingBroadcastMessage = _broadcastMessageRepo
                .Query(x => x.Status == EBroadcastMessageStatus.Pending).OrderBy(x => x.CreatedAt).ToList();
            
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

                    // call the httpSendMessage service to send text based message;
                    var outboundBroadcast = await _outboundMesageService.HttpSendTextMessage(formRequest.To, formRequest);

                    // if response ok message is received from httpMessageSend service, update status
                    if (outboundBroadcast.Data)
                    {
                        broadcastMessage.Status = EBroadcastMessageStatus.Sent;
                        broadcastMessage.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        broadcastMessage.Status = EBroadcastMessageStatus.Failed;
                        broadcastMessage.UpdatedAt = DateTime.Now;
                    }

                }
                catch (Exception ex)
                {
                    broadcastMessage.ErrorMessage = ex.Message;
                    broadcastMessage.UpdatedAt = DateTime.Now;
                    broadcastMessage.Status = EBroadcastMessageStatus.Failed;
                }
                finally
                {
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

