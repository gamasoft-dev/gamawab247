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
}

