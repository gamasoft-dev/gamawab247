using System.Threading;
using Infrastructure.Sessions;

namespace ApiCustomization.Common
{
    public class ApiContentIntegrationManager: IApiContentIntegrationManager
    {
		private ICollection<IApiContentRetrievalService> apiContentRetrievalServices;
        private ISessionManagement sessionManagement;

		public ApiContentIntegrationManager(ICollection<IApiContentRetrievalService> apiContentRetrievalServices,
            ISessionManagement sessionManagement)
		{
			this.apiContentRetrievalServices = apiContentRetrievalServices;
            this.sessionManagement = sessionManagement;
		}

		private IApiContentRetrievalService GetConcreteIntegrationImpl(string partnerContentProcessorKey)
		{
            if (string.IsNullOrEmpty(partnerContentProcessorKey))
                throw new Exception($"API customization key cannot be null. Value was null or empty");

            var concreteImpl = apiContentRetrievalServices?
                .FirstOrDefault(x => x.PartnerContentProcessorKey.ToLower().Trim() == partnerContentProcessorKey.ToLower().Trim());

            if (concreteImpl is null)
				throw new Exception($"No concrete implementation was found for this partner intgeration, with name {partnerContentProcessorKey}");

			return concreteImpl;
        }

        public async Task<RetrieveContentResponse> RetrieveContent<TRequest>(Guid businessId, string partnerContentProcessorKey,
            string waId, TRequest request)
        {
            var concreteImpl = GetConcreteIntegrationImpl(partnerContentProcessorKey);

            var result = await concreteImpl.RetrieveContent<TRequest>(businessId, waId, request);
            //if the session new updated is not null. Update the user session state to the new state
            if (result.UpdatedSessionState != null && !result.IsSuccessful)
            {
                var session = await sessionManagement.GetByWaId(waId);
                session.SessionState = result.UpdatedSessionState.Value;
                await sessionManagement.Update(waId, session);
            }

            return result;
        }
    }
}

