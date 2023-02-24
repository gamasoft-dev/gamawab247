using Application.AutofacDI;
using Application.Helpers;
using System;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IMessageProcessor : IAutoDependencyService
    {
        Task ValidateInboundMessage(Guid businessId, dynamic request);
    }
}
