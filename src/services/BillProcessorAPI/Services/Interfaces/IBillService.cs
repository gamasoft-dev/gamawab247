using BillProcessorAPI.Dtos;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
	public interface IBillService
	{
		Task<SuccessResponse<BillReferenceResponseDto>> ReferenceVerification(BillRequestDto model);
		Task<SuccessResponse<BillPaymentVerificationResponseDto>> PaymentVerification(BillRequestDto model);
	}
}
