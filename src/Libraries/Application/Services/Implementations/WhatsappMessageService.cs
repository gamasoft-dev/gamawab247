using Application.DTOs.CreateDialogDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.Identities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Sessions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class WhatsappMessageService : IWhatsappMessageService
    {
        private readonly IRepository<WhatsappUser> _waUserRepository;
        private readonly ISessionManagement _sessionMgt;
        private readonly IRepository<Business> _businessRepo;
        private readonly IRepository<BusinessMessage> _businessMessageRepo;
        private readonly IRepository<TextMessage> _textMessageRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IMapper _mapper;
        private readonly IOutboundMesageService _outboundMesageService;
        public WhatsappMessageService(IRepository<WhatsappUser> waUserRepository, ISessionManagement sessionMgt, IRepository<Business> businessRepo, IRepository<BusinessMessage> businessMessageRepo, IMapper mapper, IOutboundMesageService outboundMesageService, IRepository<User> userRepo)
        {
            _waUserRepository = waUserRepository;
            _sessionMgt = sessionMgt;
            _businessRepo = businessRepo;
            _businessMessageRepo = businessMessageRepo;
            _mapper = mapper;
            _outboundMesageService = outboundMesageService;
            _userRepo = userRepo;
        }

        public async Task<SuccessResponse<bool>> DisableAutomatedResponse(string waId, Guid businessId)
        {
            var waUser = await _waUserRepository.FirstOrDefault(x => x.WaId == waId);

            if (waUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var business = await _businessRepo.FirstOrDefault(x => x.Id == businessId)
               ?? throw new RestException(HttpStatusCode.NotFound, "Business not found");

            var session = await _sessionMgt.GetByWaId(waId);

            if (session is null)
            {
                session = await _sessionMgt.CreateNewSession(waId, null, waUser.Name, business, DateTime.Now,
                    ESessionState.CONVERSATION_WITH_ADMIN, null, null);

            }
            else
            {
                session.SessionState = ESessionState.CONVERSATION_WITH_ADMIN; 
                await _sessionMgt.Update(waId, session);
            }
            var message = await GetDisableAutomatedResponsePrompt(businessId);
            if (message is null)
                throw new RestException(HttpStatusCode.NotFound, "No 'speak to admin' prompt configured");

            var adminInfo = await _userRepo.FirstOrDefault(x=>x.Id == WebHelper.UserId)
                ?? throw new RestException(HttpStatusCode.NotFound, "Admin user not found");

            await SendAutomatedResponse(waId,message,adminInfo.FirstName);
            return new SuccessResponse<bool>
            {
                Message = ResponseMessages.UpdateResponse,
                Data = true
            };
        }

        public async Task<BusinessMessageDto<BaseInteractiveDto>> GetDisableAutomatedResponsePrompt(Guid businessId)
        {
            var businessMessage = await _businessMessageRepo.FirstOrDefault(x => x.BusinessId == businessId && x.AdminResponseStatus == EAdminResponseStatus.Initiated.ToString());

            if (businessMessage is null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.BusinessNotFound);

            var textMessage =
                await _textMessageRepo.FirstOrDefault(x => x.Id == businessMessage.InteractiveMessageId);

            var businessMessageRes = new BusinessMessageDto<BaseInteractiveDto>
            {
                Id = businessMessage.Id,
                BusinessId = businessMessage.BusinessId,
                MessageType = businessMessage.MessageType,
                Name = businessMessage.Name,
                Position = businessMessage.Position,
                RecipientType = businessMessage.RecipientType,
                MessageTypeObject = textMessage != null ? _mapper.Map<TextMessageDto>(textMessage) : default,
                ShouldTriggerFormProcessing = businessMessage.ShouldTriggerFormProcessing,
                BusinessFormId = businessMessage.BusinessFormId,
                BusinessConversationId = businessMessage.BusinessConversationId
            };

            return businessMessageRes;
        }

        private async Task SendAutomatedResponse(string waId,BusinessMessageDto<BaseInteractiveDto> model, string adminName)
        {
            model.MessageTypeObject.Body.Replace("{adminName}", adminName);
            await _outboundMesageService.HttpSendTextMessage(waId, model,null);
        }


    }
}
