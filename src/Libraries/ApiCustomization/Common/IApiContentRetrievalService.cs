using System;
namespace ApiCustomization.Common
{
	// Note any business that we retrives content via api is also a partner
	// any provider ERP/ CMS etc that the system interacts with to retrieve content or post content to is a partner
	public interface IApiContentRetrievalService
	{
		public string PartnerApiProcessIndentifier { get; }

		public Task<string> RetrieveContent<TRequest>(TRequest request);
	}

	public class ApiContentIntegrationFactory: IApiContentIntegrationFactory
    {
		public ICollection<IApiContentRetrievalService> apiContentRetrievalServices;

		public ApiContentIntegrationFactory(ICollection<IApiContentRetrievalService> apiContentRetrievalServices)
		{
			this.apiContentRetrievalServices = apiContentRetrievalServices;
		}

		public IApiContentRetrievalService getConcreteIntegrationImpl(string partnerApiProcessIndentifier)
		{
			if (string.IsNullOrEmpty(partnerApiProcessIndentifier))
				throw new ProcessCancellationException("No concrete implementation exist for this partner intgeration name");

			return apiContentRetrievalServices?.FirstOrDefault(x => x.PartnerApiProcessIndentifier == partnerApiProcessIndentifier);
		}
	}

	public interface IApiContentIntegrationFactory
	{
		public IApiContentRetrievalService getConcreteIntegrationImpl(string partnerApiProcessIndentifier);

    }
}

