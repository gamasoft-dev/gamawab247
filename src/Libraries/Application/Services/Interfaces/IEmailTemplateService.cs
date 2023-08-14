using Application.AutofacDI;
using Application.Helpers;

namespace Application.Services.Interfaces
{
    public interface IEmailTemplateService : IAutoDependencyService
    {
        string GetConfirmEmailTemplate(string otp, string email, string firstName, string title);
        string GetReceiptBroadcastEmailTemplate(string fullName, string message);
    }
}
