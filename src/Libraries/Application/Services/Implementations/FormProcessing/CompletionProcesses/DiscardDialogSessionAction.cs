using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Implementations.FormProcessing.CompletionProcesses
{
    public class IDialogSessionDiscard: IFormCompletionProcessor
    {
        string IFormCompletionProcessor.completionActionName => throw new System.NotImplementedException();


        Task IFormCompletionProcessor.doWork(string waId, DialogSession session)
        {
            throw new System.NotImplementedException();
        }
    }
}