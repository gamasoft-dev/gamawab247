using Application.AutofacDI;
using Application.Helpers;
using System;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IInboundMessageService : IAutoDependencyService
    {
        Task ReceiveAndProcessMessage(Guid businessId, dynamic messageRequestData);
        IInboundMessageService ResolveBaseMessageType(dynamic requestPayload);
        IInboundMessageService ResolveInteractiveMessageType(dynamic requestPayload);
    }
}