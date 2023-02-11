using BillProcessorAPI.Dtos;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
	public interface IBillService
	{
		Task<SuccessResponse<BillPayerInfoDto>> ReferenceVerification(BillReferenceRequestDto model);
		Task<SuccessResponse<BillPaymentVerificationResponseDto>> PaymentVerification(BillPaymentVerificationRequestDto model);
	}
}
