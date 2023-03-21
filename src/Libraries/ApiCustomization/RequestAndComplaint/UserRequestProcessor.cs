using System;
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

        public async Task<RetrieveContentResponse> RetrieveContent<TRequest>(Guid businessId, string waId, TRequest request)
        {
            try
            {
                var requestConfig = await requestConfigRepo.FirstOrDefaultNoTracking(x => x.BusinessId == businessId);

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
                    Subject = userSessionResponse.subject ?? "",
                    Detail = userSessionResponse.detail,
                    TicketId = RequestAndComplaint.GenerateTicketId(),
                    Type = ERequestComplaintType.Request,
                };

                await requestRepository.AddAsync(userRequest);
                await requestRepository.SaveChangesAsync();

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

            string userRequestPropertyKey = "request-detail";

            var argValueAttempt = session.SessionFormDetails?.UserData?.TryGetValue(userRequestPropertyKey, out formElementValue);

            if (argValueAttempt is null || formElementValue is null)
                throw new BackgroundException($"No detail for the request was found in this user's session to lodge. {waId}" +
                    $" no value was found in this session with key {userRequestPropertyKey} in the userData collection");

            responses.detail = formElementValue;

            return responses;
        }

        private RetrieveContentResponse SuccessResponseOnRequest(string ticketId, string userName, int resolutionTime) {

            var slaResolutionTime = resolutionTime == 0 ? "promptly" : $"in {resolutionTime} hours";
            var message = $"Request recieved with ID - *{ticketId}*." +
                $" \n \nOur LUC Support agent will respond to you shortly";

            return new RetrieveContentResponse
            {
                IsSuccessful = true,
                Response = message,
                UpdatedSessionState = null
            };
                 
        }

        private RetrieveContentResponse ErrorResponseOnRequest(string waId)
        {
            var message = $"There was an issue whilst processing your request, \n Kindly restart the process of request submission, \n Your feedback matter to us";
            return new RetrieveContentResponse
            {
                IsSuccessful = false,
                Response = message,
                UpdatedSessionState = ESessionState.PLAINCONVERSATION
            };
        }
    }
}

