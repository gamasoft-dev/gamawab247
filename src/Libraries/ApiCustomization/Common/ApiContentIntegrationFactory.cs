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

		public IApiContentRetrievalService GetConcreteIntegrationImpl(string partnerApiProcessIndentifier)
		{
			if (string.IsNullOrEmpty(partnerApiProcessIndentifier))
				throw new ProcessCancellationException("No concrete implementation exist for this partner intgeration name");

			return apiContentRetrievalServices?.FirstOrDefault(x => x.PartnerApiProcessIndentifier == partnerApiProcessIndentifier);
		}
	}
}

