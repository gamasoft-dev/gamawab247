namespace ApiCustomization.Common
{
    public interface IApiContentIntegrationManager
	{
		
        public Task<RetrieveContentResponse> RetrieveContent<TRequest>(Guid businessID, string partnerContentProcessorKey,
            string waId, TRequest request);

    }
}

