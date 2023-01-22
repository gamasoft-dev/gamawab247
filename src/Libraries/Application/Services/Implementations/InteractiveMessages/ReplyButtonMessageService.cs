using System;
using System.Net;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.InteractiveMesageDto;
using Application.DTOs.InteractiveMesageDto.CreateMessageRequestDto;
using Application.DTOs.OutboundMessageRequests;
using Application.Helpers;
using Application.Services.Interfaces.Interactive_Messages;
using AutoMapper;
using Domain.Common;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.DialogMessageEntitties.ValueObjects;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementations.InteractiveMessages
{
    public class ReplyButtonMessageService: IInteractiveMessageProvider
    {
        public ReplyButtonMessageService(IMapper mapper,
            IRepository<BusinessMessage> businessMessageRepository,
            IRepository<BusinessConversation> businessConversationRepo,
            IRepository<Button> messageOptionRepo,
            IRepository<ReplyButtonMessage> replyButtonMessageRepo)
        {
            _mapper = mapper;
            _businessMessageRepository = businessMessageRepository;
            _businessConversationRepo = businessConversationRepo;
            _messageOptionRepo = messageOptionRepo;
            _replyButtonMessageRepo = replyButtonMessageRepo;
        }

        private readonly IMapper _mapper;
        private readonly IRepository<BusinessMessage> _businessMessageRepository;
        private readonly IRepository<BusinessConversation> _businessConversationRepo;
        private readonly IRepository<Button> _messageOptionRepo;
        private readonly IRepository<ReplyButtonMessage> _replyButtonMessageRepo;
    
        public EMessageType MessageType => EMessageType.Button;

        /// <summary>
        /// Create new message for reply button
        /// </summary>
        /// <param name="businessId"></param>
        /// <param name="model"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="RestException"></exception>
        public async Task<SuccessResponse<IInteractiveMessageResponse>> CreateMessage<T>(Guid businessId, T model)
        {
            if (typeof(T) != typeof(CreateReplyButtonMessageDto))
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Incorrect Model type passed for creating a reply message");

            if (businessId == Guid.Empty)
                throw new RestException(HttpStatusCode.BadRequest, "Business Id must not empty");
            
            var replyModel = model as CreateReplyButtonMessageDto;
            replyModel.BusinessId = businessId;
            
            var businessConversationModels = _mapper.Map<CreateAllBusinessMessageObjectsDto<CreateReplyButtonMessageConfigDto>>(replyModel);
            var businessConversation = _mapper.Map<BusinessConversation>(businessConversationModels.CreateBusinessConversationDto);
           
            // add business conversation and generate Id
            await _businessConversationRepo.AddAsync(businessConversation);
            
            // create a business message 
            var businessMessage = _mapper.Map<BusinessMessage>(businessConversationModels.CreateBusinessMessageDto);
            businessMessage.BusinessConversationId = businessConversation.Id;
            await _businessMessageRepository.AddAsync(businessMessage);

            var replyButtonMessage =
                _mapper.Map<ReplyButtonMessage>(businessConversationModels.InteractiveMessageTypeConfigDto);
            replyButtonMessage.BusinessMessageId = businessMessage.Id;
            await _replyButtonMessageRepo.AddAsync(replyButtonMessage);

            // foreach (var replyButton in replyModel?.ReplyButtonInteractive?.Interactive?.Action?.Buttons)
            // {
            //     var messageOption = _mapper.Map<Buttons>(replyButton.Reply);
            //     messageOption.BusinessMessageConfigId = replyButtonMessage.Id;
            //     replyButtonMessage.Buttons.Add(messageOption);
            // }

            await _messageOptionRepo.SaveChangesAsync();
            var getBusinessMessage = await GetById(businessMessage.Id);

            throw new NotImplementedException();

            return new SuccessResponse<IInteractiveMessageResponse>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = getBusinessMessage
            };
        }

        public Task<IInteractiveMessageResponse> ReceiveMessage<T>(T model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a reply message record by businessMessageId and serialize it into the shape af facebook reply message object 
        /// </summary>
        /// <param name="businessMessageId"></param>
        /// <param name="recipient"></param>
        /// <returns></returns>
        /// <exception cref="RestException"></exception>
        public async Task<IInteractiveMessageResponse> GetById(Guid businessMessageId, string recipient = null)
        {
            // var businessMessage = await  _businessMessageRepository?.Query(x=>x.Id == businessMessageId)
            //     ?.Include(x=>x.ReplyButtonMessage)
            //     ?.ThenInclude(x=>x.Buttons)
            //     ?.FirstOrDefaultAsync()!;
            //
            // if (businessMessage is null)
            //     throw new RestException(HttpStatusCode.NotFound, "No business message found");
            //
            // // var replyMessageDto = _mapper.Map<GetReplyButtonMessageDto>(businessMessage);
            //
            // return RequestHelper.ToModel<ReplyButtonMessageRequest>(businessMessage, recipient);
            
            throw new NotImplementedException();
        }
    }
}
