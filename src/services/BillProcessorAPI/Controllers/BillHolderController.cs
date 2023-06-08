using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities.ValueObjects;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BillProcessorAPI.Controllers
{

	[ApiController]
	[ApiVersion("1.0")]
	[Route("v1/abc")]
	[Produces("application/json")]
	public class BillHolderController : ControllerBase
	{
		private readonly IBillService _revpay;
		private readonly ICollectionReportService _collection;
        public BillHolderController(IBillService revpay, ICollectionReportService collection)
        {
            _revpay = revpay;
            _collection = collection;
        }

        [HttpGet("invoice/{billPaymentCode}")]
        [ProducesResponseType(typeof(SuccessResponse<BillReferenceResponseDto>), 200)]
        [SwaggerOperation(Summary = "Endpoint to get bill payer reference")]
        public async Task<IActionResult> ReferenceVerification([FromRoute] string billPaymentCode, [FromQuery] string phone)
		{
			var response = await _revpay.ReferenceVerification(phone, billPaymentCode);
			return Ok(response);
		}

		[HttpGet("receipt/{billPaymentCode}")]
        [ProducesResponseType(typeof(SuccessResponse<CustomBillPaymentVerificationResponse>), 200)]
        [SwaggerOperation(Summary = "Endpoint to get verify bill payment")]
        public async Task<IActionResult> PaymentVerification([FromRoute]string billPaymentCode)
		{
			var response = await _revpay.PaymentVerification(billPaymentCode);
			return Ok(response);
		}

        [HttpGet( "collection-report", Name = nameof(Collections))]
        [ProducesResponseType(typeof(SuccessResponse<CollectionReportDto>), 200)]
        [SwaggerOperation(Summary = "Endpoint to get verify bill payment")]
        public async Task<IActionResult> Collections([FromQuery] ResourceParameter param,[FromQuery] ReportParameters reportParameters)
        {
            var response = await _collection.GetAllCollections(param, reportParameters, nameof(Collections), Url);
            return Ok(response);
        }
    }
}
