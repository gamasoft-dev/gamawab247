using Application.Services.Interfaces.IReceiveMessageManager;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.InboundMessageDto;

namespace Application.Helpers.InboundMessageHelper
{
    public class MessageTypeResolver : IMessageTypeResolver
    {
        private readonly IEnumerable<IReceiveMessageManager> _receiveMessageManager;
        public MessageTypeResolver(IEnumerable<IReceiveMessageManager> receiveMessageManager)
        {
            _receiveMessageManager = receiveMessageManager;
        }

        public async Task<string> GetMessageType(TextNotificationDto inboundText)
        {
            var message = inboundText?.Messages?.FirstOrDefault();
            
            //recursive prog.. to aid recheck at this point..
            ReceiveMessageType initialInboundMessageType = GetMessageType(message);
            string response = string.Empty;

            // get the type from the interactive message object
            response = initialInboundMessageType == ReceiveMessageType.Interactive ? message?.Type : message?.Type;

            return response;
            
            var messageManager = _receiveMessageManager
                .FirstOrDefault(x => x.MessageType == initialInboundMessageType);

            if (messageManager == null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest,
                    "An error occurred. No Implementation found for this message type.");
           
            await messageManager
                .ProcessMessageByType(inboundText);
        }

        private ReceiveMessageType GetMessageType(TextMessageObjectDto textMessageObjectDto)
        {
            var modelType = textMessageObjectDto?.Type switch
            {
                "text" => ReceiveMessageType.Text,
                "interactive" => ReceiveMessageType.Interactive,
                _ => throw new NotImplementedException()
            };
            return modelType;
        }

        private string SetFirstMessage(string bot, string botName, string botDescription)
        {
            const string initMessage = "";
            string result = string.Empty;
            return result;
        }
    }
}
