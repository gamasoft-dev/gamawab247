using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.AutofacDI;
using Domain.Entities;

namespace Application.Cron.ResponseProcessing
{
    public interface IResponsePreProcessingCron : IAutoDependencyService
    {
        Task InitiateMessageProcessing();

        Task SendUnResolvedMessage(InboundMessage inboundMessage, string Message);
    }
}
