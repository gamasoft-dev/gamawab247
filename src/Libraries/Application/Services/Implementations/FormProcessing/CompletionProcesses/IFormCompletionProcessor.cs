using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Implementations.FormProcessing.CompletionProcesses
{
    public interface IFormCompletionProcessor
    {
        string completionActionName { get; }

        Task doWork(string waId, DialogSession session);
    }
}

