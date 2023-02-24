using Application.AutofacDI;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IWebHookRegistrationHelper : IAutoDependencyService
    {
        Task<bool> RegisterBusinessWebHookUrl(string webHookUrl, string apiKey);
    }
}