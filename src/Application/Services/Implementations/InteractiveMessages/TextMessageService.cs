using System;
using System.Net;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.InteractiveMesageDto;
using Application.DTOs.InteractiveMesageDto.CreateMessageRequestDto;
using Application.Helpers;
using Application.Services.Interfaces.Interactive_Messages;
using AutoMapper;
using Domain.Common;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.DialogMessageEntitties.ValueObjects;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.Implementations.InteractiveMessages
{
    public class TextMessageService: IInteractiveMessageProvider
    {
        private readonly IMapper _mapper;
        private readonly IRepository<BusinessMessage> _businessMessageRepository;
        private readonly IRepository<BusinessConversation> _businessConversationRepo;
        private readonly IRepository<Button> _messageOptionRepo;
        private readonly IRepository<TextMessage> _textMessageRepo;

        public TextMessageService(IMapper mapper, 
            IRepository<BusinessMessage> businessMessageRepository, 
            IRepository<BusinessConversation> businessConversationRepo, 
            IRepository<Button> messageOptionRepo, 
            IRepository<TextMessage> textMessageRepo)
        {
            _mapper = mapper;
            _businessMessageRepository = businessMessageRepository;
            _businessConversationRepo = businessConversationRepo;
            _messageOptionRepo = messageOptionRepo;
            _textMessageRepo = textMessageRepo;
        }

        public EMessageType MessageType => EMessageType.Text;

        Task<IInteractiveMessageResponse> IInteractiveMessageProvider.ReceiveMessage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public async Task<IInteractiveMessageResponse> GetById(Guid businessMessageId, string recipient = null)
        {
            // var businessMessage = await _businessMessageRepository?.Query(x => x.Id == businessMessageId)
            //     ?.Include(x => x.ListMessage)
            //     ?.ThenInclude(x => x.Rows)
            //     ?.FirstOrDefaultAsync()!;
            //
            // if (businessMessage is null)
            //     throw new RestException(HttpStatusCode.NotFound, "No business message found");
            //
            // var replyMessageDto = _mapper.Map<ListActionDto>(businessMessage);
            //
            // return RequestHelper.ToListMessageModel<ListInteractiveMessageRequest>(businessMessage, recipient);
            
            throw new NotImplementedException();
        }

        public async Task<SuccessResponse<IInteractiveMessageResponse>> CreateMessage<T>(Guid businessId, T model)
        {
            if (typeof(T) != typeof(TextMessageDto))
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Incorrect Model type passed for creating a list message");

            if (businessId == Guid.Empty)
                throw new RestException(HttpStatusCode.BadRequest, "Business Id must not empty");

            var textMessageModel = model as TextMessageDto;
            textMessageModel.BusinessId = businessId;

            var businessConversationModels = _mapper.Map<CreateAllBusinessMessageObjectsDto<TextMessage>>(textMessageModel);
            var businessConversation = _mapper.Map<BusinessConversation>(businessConversationModels.CreateBusinessConversationDto);

            // add business conversation and generate Id
            await _businessConversationRepo.AddAsync(businessConversation);

            // create a business message 
            var businessMessage = _mapper.Map<BusinessMessage>(businessConversationModels.CreateBusinessMessageDto);
            businessMessage.BusinessConversationId = businessConversation.Id;
            await _businessMessageRepository.AddAsync(businessMessage);

            var messageType =
                _mapper.Map<TextMessage>(businessConversationModels.InteractiveMessageTypeConfigDto);

            messageType.BusinessMessageId = businessMessage.Id;
            await _textMessageRepo.AddAsync(messageType);

            var messageOption = _mapper.Map<Button>(textMessageModel);

            // messageOption.BusinessMessageConfigId = messageType.Id;
            await _textMessageRepo.SaveChangesAsync();

            var getBusinessMessage = await GetById(businessMessage.Id);

            throw new NotImplementedException();

            return new SuccessResponse<IInteractiveMessageResponse>
            {
                Data = getBusinessMessage,
                Message = ResponseMessages.CreationSuccessResponse
            };
        }
    }
}
