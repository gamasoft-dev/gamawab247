﻿using System;
using ApiCustomization.Common;
using Domain.Entities;
using Domain.Entities.RequestAndComplaints;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Sessions;

namespace ApiCustomization.RequestAndComplaints
{
	public class UserComplaintProcessor: IApiContentRetrievalService
    {
        private readonly IRepository<RequestAndComplaint> requestRepository;
        private readonly IRepository<RequestAndComplaintConfig> requestConfigRepo;
        private readonly ISessionManagement sessionManagement;
        private readonly IRepository<WhatsappUser> whatsappUserRepository;

        public UserComplaintProcessor(IRepository<RequestAndComplaint> requestRepository,
            IRepository<RequestAndComplaintConfig> requestConfigRepo,
            ISessionManagement sessionManagement,
            IRepository<WhatsappUser> whatsappUserRepository)
		{
            this.requestRepository = requestRepository;
            this.requestConfigRepo = requestConfigRepo;
            this.sessionManagement = sessionManagement;
            this.whatsappUserRepository = whatsappUserRepository;
		}

        public string PartnerContentProcessorKey => EExternalPartnerContentProcessorKey
            .USER_COMPLAINT_PROCESSOR.ToString();

        public async Task<RetrieveContentResponse> RetrieveContent<TRequest>(Guid businessId, string waId, TRequest request)
        {
            try
            {
                var requestConfig = await requestConfigRepo.FirstOrDefaultNoTracking(x => x.BusinessId == businessId);

                if (requestConfig is null) throw new BackgroundException($"There no configured Request Config for this business with id => {businessId}");

                var session = await sessionManagement.GetByWaId(waId);
                if (session is null) throw new BackgroundException($"There is no active session for this user that has submitted this request");

                var whatsaAppUser = await whatsappUserRepository.FirstOrDefaultNoTracking(x => x.WaId == waId);

                (string subject, string detail) userSessionResponse = GetUserCompplaintFromSession(waId, session);

                var userRequest = new RequestAndComplaint
                {
                    BusinessId = businessId,
                    CallBackUrl = requestConfig.WebHookUrl,
                    Channel = "Whatsapp",
                    CustomerId = waId,
                    CustomerName = whatsaAppUser is not null ? whatsaAppUser.Name : waId,
                    Subject = userSessionResponse.subject ?? "",
                    Detail = userSessionResponse.detail,
                    TicketId = RequestAndComplaint.GenerateTicketId(),
                    Type = ERequestComplaintType.Complaint,
                };

                await requestRepository.AddAsync(userRequest);
                await requestRepository.SaveChangesAsync();

                return SuccessResponseOnComplaint(userRequest.TicketId, session.UserName, requestConfig.TimeInHoursOfRequestResolution);

            }
            catch (Exception)
            {
                await sessionManagement.RemoveUserSession(waId);
                return ErrorResponseOnComplaint(waId);
            }
        }


        private static (string subject, string detail) GetUserCompplaintFromSession(string waId, DialogSession session)
        {

            string formElementValue = null;
            (string subject, string detail) responses = new();

            string userRequestPropertyKey = "complaint-detail";

            var argValueAttempt = session.SessionFormDetails?.UserData?.TryGetValue(userRequestPropertyKey, out formElementValue);

            if (argValueAttempt is null || formElementValue is null)
                throw new BackgroundException($"Error deserializing RequestAndComplaint retrieving value from cache," +
                    $" no value was found in this session with key {userRequestPropertyKey} in the userData collection");

            responses.detail = formElementValue;

            return responses;
        }

        private RetrieveContentResponse SuccessResponseOnComplaint(string ticketId, string userName, int resolutionTime)
        {

            var slaResolutionTime = resolutionTime == 0 ? "promptly" : $"in {resolutionTime} hours";
            var message = $"Compliant lodged with ID - *{ticketId}*." +
                $" \n \nOur LUC Support agent will respond to you shortly";

            return new RetrieveContentResponse
            {
                IsSuccessful = true,
                Response = message,
                UpdatedSessionState = null
            };
        }

        private RetrieveContentResponse ErrorResponseOnComplaint(string waId)
        {
            var message = $"There was an issue whilst lodging and processing your complaint, \nKindly restart the process of complaint submission, \nYour feedback matter to us";
            return new RetrieveContentResponse
            {
                IsSuccessful = false,
                Response = message,
                UpdatedSessionState = ESessionState.PLAINCONVERSATION
            };
        }
    }
}

