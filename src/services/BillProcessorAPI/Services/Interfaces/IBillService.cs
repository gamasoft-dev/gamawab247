using BillProcessorAPI.Dtos;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
	public interface IBillService
	{
		Task<SuccessResponse<BillReferenceResponseDto>> ReferenceVerification(string phone, string billPaymentCode);
		Task<SuccessResponse<CustomBillPaymentVerificationResponse>> PaymentVerification(string billPaymentCode);
		Task<SuccessResponse<IEnumerable<CustomBillPaymentVerificationResponse>>> GetAllTransactions();

    }
}
