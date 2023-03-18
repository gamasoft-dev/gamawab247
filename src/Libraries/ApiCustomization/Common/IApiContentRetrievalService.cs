using System;
namespace ApiCustomization.Common
{
    // Note any business that we retrives content via api is also a partner
    // any provider ERP/ CMS etc that the system interacts with to retrieve content or post content to is a partner
    public interface IApiContentRetrievalService
	{
		public string PartnerContentProcessorKey { get; }

		/// <summary>
		/// Use this method to retrieve api customization content
		/// </summary>
		/// <typeparam name="TRequest"></typeparam>
		/// <param name="waId">user phone number</param>
		/// <param name="request"> generic payload </param>
		/// <returns></returns>
		public Task<RetrieveContentResponse> RetrieveContent<TRequest>(Guid businessId, string waId, TRequest request);
	}
}

