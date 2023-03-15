using System.Threading;

namespace ApiCustomization.Common
{
    public class ApiContentIntegrationManager: IApiContentIntegrationManager
    {
		public ICollection<IApiContentRetrievalService> apiContentRetrievalServices;

		public ApiContentIntegrationManager(ICollection<IApiContentRetrievalService> apiContentRetrievalServices)
		{
			this.apiContentRetrievalServices = apiContentRetrievalServices;
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

        public async Task<string> RetrieveContent<TRequest>(Guid businessId, string partnerContentProcessorKey,
            string waId, TRequest request)
        {
            var concreteImpl = GetConcreteIntegrationImpl(partnerContentProcessorKey);

            return await concreteImpl.RetrieveContent<TRequest>(businessId, waId, request);
        }
    }
}

