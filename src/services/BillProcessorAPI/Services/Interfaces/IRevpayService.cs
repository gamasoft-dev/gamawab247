using BillProcessorAPI.Dtos;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
	public interface IRevpayService
	{
		Task<SuccessResponse<BillPayerInfoDto>> ReferenceVerification(BillReferenceRequestDto model);
		Task<SuccessResponse<BillPayerInfoDto>> PaymentVerification(PaymentVerificationRequestDto model);
	}
}
