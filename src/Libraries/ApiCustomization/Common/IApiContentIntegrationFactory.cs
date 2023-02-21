namespace ApiCustomization.Common
{
    public interface IApiContentIntegrationFactory
	{
		public IApiContentRetrievalService GetConcreteIntegrationImpl(string partnerApiProcessIndentifier);

    }
}

