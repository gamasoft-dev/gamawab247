using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
    public interface ICutlyService
    {
        Task<SuccessResponse<string>> GenerateShortenedPaymentLink(string waId, string billCode);
        Task<string> ShortLink(string link);
    }
}
