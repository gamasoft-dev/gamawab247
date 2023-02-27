using Application.AutofacDI;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public interface IWebHookRegistrationHelper : IAutoDependencyService
    {
        Task<bool> RegisterBusinessWebHookUrl(string webHookUrl, string apiKey);
    }
}