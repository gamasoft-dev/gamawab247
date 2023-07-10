using System;
using Application.Helpers;
using Application.Services.Interfaces;
using BillProcessorAPI.Repositories.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.FormProcessing;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BroadcastMessageServiceWorker.Services
{
    public class BroadcastDispatchService : IBroadcastDispatchService
    {
        private readonly Infrastructure.Repositories.Interfaces.IRepository<BroadcastMessage> _broadcastMessageRepo;
        //private readonly IBillTransactionRepository _billTransactionRepo;
        private readonly IOutboundMesageService _outboundMesageService;

        public BroadcastDispatchService(Infrastructure.Repositories.Interfaces.IRepository<BroadcastMessage> broadcastMessageRepo, IOutboundMesageService outboundMesageService)
        {
            _broadcastMessageRepo = broadcastMessageRepo;
            _outboundMesageService = outboundMesageService;
            //_billTransactionRepo = billTransactionRepo;
        }

        public async Task SendMessage()
        {
            // get paginated list of broacast messages on pending by order of FIFO using the createdTime
            var pendingBroadcastMessage = _broadcastMessageRepo
                .Query(x => x.Status == Domain.Enums.EBroadcastMessageStatus.Pending).OrderBy(x => x.CreatedAt).ToList();
            
            // iterate through the list and process message sending as below
            foreach (var broadcastMessage in pendingBroadcastMessage)
             {
                // format the message value of the broadcast message entity

                var formRequest = new FormRequestResponse();
                formRequest.From = broadcastMessage.From;
                formRequest.To = broadcastMessage.To;
                formRequest.Message = broadcastMessage.Message;
                formRequest.MessageType = EMessageType.Text.ToString();
                formRequest.BusinessId = broadcastMessage.BusinessId;

                // update the broadcast message status as processing once send
                broadcastMessage.Status = EBroadcastMessageStatus.Processing;

                // call the httpSendMessage service to send text based message;
                var outboundBroadcast = await _outboundMesageService.HttpSendTextMessage(formRequest.To, formRequest);

                // if response ok message is received from httpMessageSend service
                // update the broadcast message to sent
                if (outboundBroadcast.Data)
                {
                    broadcastMessage.Status = EBroadcastMessageStatus.Sent;
                  
                }
                else
                {
                    broadcastMessage.Status = EBroadcastMessageStatus.Failed;
                }
                await _broadcastMessageRepo.SaveChangesAsync();
                //await _billTransactionRepo.SaveChangesAsync();
            }
        }

       

    }
    public interface IBroadcastDispatchService
    {
        Task SendMessage();
    }
}

