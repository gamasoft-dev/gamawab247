using Application.AutofacDI;
using Application.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Interfaces.Interactive_Messages.Manager
{
    public interface IInteractiveMessageManager: IAutoDependencyService
    {
        IInteractiveMessageProvider GetInteractiveMessageProvider(string messageType);  
    }

    public class InteractiveMessageManager : IInteractiveMessageManager
    {
        private readonly IEnumerable<IInteractiveMessageProvider> _interactiveMessageProviders;
        public InteractiveMessageManager(IEnumerable<IInteractiveMessageProvider> interactiveMessageProviders)
        {
            _interactiveMessageProviders = interactiveMessageProviders;
        }

        public IInteractiveMessageProvider GetInteractiveMessageProvider(string messageType)
        {
            if (string.IsNullOrWhiteSpace(messageType))
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "No message type provided");

            var provider =  _interactiveMessageProviders
                .FirstOrDefault(x => x.MessageType.ToString() == messageType);

            return provider ?? throw new RestException(System.Net.HttpStatusCode.NotImplemented,
                $"No implementation was found to process this message type {messageType}");
        }
    }
}
