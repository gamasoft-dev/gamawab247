using System;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Implementations.FormProcessing.CompletionProcesses
{
    public class FormProcessRestartProcessor : IFormCompletionProcessor
    {
        public string completionActionName => throw new NotImplementedException();

        public Task doWork(string waId, DialogSession session)
        {
            throw new NotImplementedException();
        }
    }
}

