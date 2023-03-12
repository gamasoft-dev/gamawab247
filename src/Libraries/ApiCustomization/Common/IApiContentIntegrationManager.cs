namespace ApiCustomization.Common
{
    public interface IApiContentIntegrationManager
	{
		
        public Task<string> RetrieveContent<TRequest>(string partnerContentProcessorKey, string waId, TRequest request);

    }
}

