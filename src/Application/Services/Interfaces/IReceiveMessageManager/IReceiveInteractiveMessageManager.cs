using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Services.Interfaces.IReceiveMessageManager
{
    public interface IReceiveInteractiveMessageManager
    {
        EMessageType MessageType { get; }
        Task<string> ProcessInteractiveMessage(string message);
    }
}