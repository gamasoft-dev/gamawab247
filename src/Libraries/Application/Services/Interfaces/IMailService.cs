using Application.AutofacDI;
using Application.Helpers;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IMailService : IAutoDependencyService
    {
        Task<bool> SendSingleMail(string reciepientAddress, string message, string subject);    
    }
}
