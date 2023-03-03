using Application.Exceptions;

namespace ApiCustomization.Common
{
    public class ApiContentIntegrationFactory: IApiContentIntegrationFactory
    {
		public ICollection<IApiContentRetrievalService> apiContentRetrievalServices;

		public ApiContentIntegrationFactory(ICollection<IApiContentRetrievalService> apiContentRetrievalServices)
		{
			this.apiContentRetrievalServices = apiContentRetrievalServices;
		}

		public IApiContentRetrievalService GetConcreteIntegrationImpl(string partnerContentProcessorKey)
		{
            if (string.IsNullOrEmpty(partnerContentProcessorKey))
                throw new ProcessCancellationException($"API customization key cannot be null. Value was null or empty");

            var concreteImpl = apiContentRetrievalServices?.FirstOrDefault(x => x.PartnerContentProcessorKey == partnerContentProcessorKey);

            if (concreteImpl is null)
				throw new ProcessCancellationException("No concrete implementation exist for this partner intgeration name");

			return concreteImpl;

        }
	}
}

