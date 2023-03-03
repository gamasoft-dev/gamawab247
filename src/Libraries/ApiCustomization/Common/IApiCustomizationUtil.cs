using System;
using Application.AutofacDI;
using Application.Common.Sessions;
using Domain.Entities.FormProcessing.ValueObjects;
using Domain.Exceptions;

namespace ApiCustomization.Common
{
	public interface IApiCustomizationUtil: IAutoDependencyService
	{
        Task<string> GetArgumentValue(KeyValueObj argumentObj, string phoneNumber, string processorKey);

    }

    public class ApiCustomizationUtil : IApiCustomizationUtil
    {
        private readonly ISessionManagement sessionManagement;

        public ApiCustomizationUtil(ISessionManagement sessionManagement)
        {
            this.sessionManagement = sessionManagement;
        }

        public async Task<string> GetArgumentValue(KeyValueObj argumentObj, string phoneNumber, string processorKey)
        {
            if (argumentObj is null)
                throw new BackgroundException($"The argument object cannot be null, this happened on the Customization for {processorKey}");

            var argumentValue = string.Empty;


            if (argumentObj.IsValueResolvedAtRuntime)
            {
                var session = await sessionManagement.GetByWaId(phoneNumber);
                string formElementValue = string.Empty;

                var argValueAttempt = session.SessionFormDetails?.UserData?.TryGetValue(argumentObj.Key, out formElementValue);

                if (argValueAttempt is null || !argValueAttempt.Value || formElementValue is null)
                    throw new BackgroundException($"Error deserializing argument value from session cache, no value was found for the session key {argumentObj.Key}");

                return formElementValue;

            }
            else
            {
                if (argumentObj?.Value is null)
                    throw new BackgroundException($"Error ArgumentObj is set to not retrieve value at runtime, yet value was not preset. Ref {processorKey}");

                return argumentObj.Value;
            }
        }

    }
}

