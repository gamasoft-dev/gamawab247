namespace ApiCustomization.Common
{
    public interface IApiContentIntegrationManager
	{
		
        public Task<string> RetrieveContent<TRequest>(Guid businessID, string partnerContentProcessorKey,
            string waId, TRequest request);

    }
}

