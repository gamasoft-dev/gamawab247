﻿using System;
using ApiCustomization.ABC;
using ApiCustomization.Common;
using Domain.Entities;
using Domain.Entities.RequestAndComplaints;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Sessions;

namespace ApiCustomization.RequestAndComplaints
{
	public class UserRequestProcessor: IApiContentRetrievalService
    {
        private readonly IRepository<RequestAndComplaint> requestRepository;
        private readonly IRepository<RequestAndComplaintConfig> requestConfigRepo;
        private readonly ISessionManagement sessionManagement;

        public UserRequestProcessor(IRepository<RequestAndComplaint> requestRepository,
            IRepository<RequestAndComplaintConfig> requestConfigRepo,
            ISessionManagement sessionManagement)
        {
            this.requestRepository = requestRepository;
            this.requestConfigRepo = requestConfigRepo;
            this.sessionManagement = sessionManagement;
        }

        public string PartnerContentProcessorKey => EExternalPartnerContentProcessorKey
            .USER_REQUEST_PROCESSOR.ToString();

        public async Task<string> RetrieveContent<TRequest>(Guid businessId, string waId, TRequest request)
        {
            try
            {
                var requestConfig = await requestConfigRepo.FirstOrDefaultNoTracking(x => x.BusinessId == businessId && x.PartnerContentProcessorKey == PartnerContentProcessorKey);

                if (requestConfig is null) throw new BackgroundException($"There no configured Request Config for this business with id => {businessId}");

                var session = await sessionManagement.GetByWaId(waId);
                if (session is null) throw new BackgroundException($"There is no active session for this user that has submitted this request");


                (string subject, string detail) userSessionResponse = GetUserRequestFromSession(waId, session);

                var userRequest = new RequestAndComplaint
                {
                    BusinessId = businessId,
                    CallBackUrl = requestConfig.WebHookUrl,
                    Channel = "Whatsapp",
                    CustomerId = waId,
                    Subject = userSessionResponse.subject,
                    Detail = userSessionResponse.detail,
                    TicketId = RequestAndComplaint.GenerateTicketId(),
                    Type = ERequestComplaintType.Request,
                };

                await requestRepository.AddAsync(userRequest);

                return SuccessResponseOnRequest(userRequest.TicketId, session.UserName, requestConfig.TimeInHoursOfRequestResolution);

            }
            catch (Exception)
            {
                await sessionManagement.RemoveUserSession(waId);
                return ErrorResponseOnRequest(waId);
            }
        }

        private static (string subject, string detail) GetUserRequestFromSession(string waId, DialogSession session) {

            string formElementValue = null;
            (string subject, string detail) responses = new ();

            string userRequestPropertyKey = EERequestComplaintPropertyKeys.Subject.ToString();

            var argValueAttempt = session.SessionFormDetails?.UserData?.TryGetValue(userRequestPropertyKey, out formElementValue);

            if (argValueAttempt is null || formElementValue is null)
                throw new BackgroundException($"Error deserializing RequestAndComplaint retrieving value from cache," +
                    $" no value was found in this session with key {userRequestPropertyKey} in the userData collection");

            responses.subject = formElementValue;

            // value for the request detail.
            userRequestPropertyKey = EERequestComplaintPropertyKeys.Detail.ToString();

            argValueAttempt = session.SessionFormDetails?.UserData?.TryGetValue(userRequestPropertyKey, out formElementValue);

            if (argValueAttempt is null || formElementValue is null)
                throw new BackgroundException($"Error deserializing RequestAndComplaint retrieving value from cache," +
                    $" no value was found in this session with key {userRequestPropertyKey} in the userData collection");

            responses.detail = formElementValue;

            return responses;
        }

        private static string SuccessResponseOnRequest(string ticketId, string userName, int resolutionTime) {

            var slaResolutionTime = resolutionTime == 0 ? "promptly" : $"in {resolutionTime} hours";
            return $"Dear {userName}, \n your Request has been recieved and will be attended to {slaResolutionTime}. " +
                $" \n Your ticketId for tracking of this request is {ticketId}";
                 
        }

        private static string ErrorResponseOnRequest(string waId)
        {
            return $"There was an issue whilst processing your request, \n Kindly restart the process of request submission, \n Your feedback matter to us";

        }
    }
}
