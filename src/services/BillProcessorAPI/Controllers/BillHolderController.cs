using Application.Helpers;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities.ValueObjects;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace BillProcessorAPI.Controllers
{

    [ApiController]
	[ApiVersion("1.0")]
	[Route("v1/abc")]
	[Produces("application/json")]
	public class BillHolderController : ControllerBase
	{
		private readonly IBillService _revpay;
		private readonly ICollectionReportService _collectionReportService;
        public BillHolderController(IBillService revpay, ICollectionReportService collection)
        {
            _revpay = revpay;
            _collectionReportService = collection;
        }

        [HttpGet("invoice/{billPaymentCode}")]
        [ProducesResponseType(typeof(SuccessResponse<BillReferenceResponseDto>), 200)]
        [SwaggerOperation(Summary = "Endpoint to get bill payer reference")]
        public async Task<IActionResult> ReferenceVerification([FromRoute] string billPaymentCode, [FromQuery][Required(ErrorMessage ="Kindly provide a phone number")] string phone)
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

        [HttpGet( "bill-report", Name = nameof(Collections))]
        [ProducesResponseType(typeof(SuccessResponse<CollectionReportDto>), 200)]
        [SwaggerOperation(Summary = "Endpoint to get get bill reports")]
        public async Task<IActionResult> Collections([FromQuery] ResourceParameter param,[FromQuery] ReportParameters reportParameters)
        {
            var response = await _collectionReportService.GetAllPagedCollections(param, reportParameters, nameof(Collections), Url);
            return Ok(response);
        }

        [HttpGet("bill-statistics", Name = nameof(CollectionsStats))]
        [ProducesResponseType(typeof(SuccessResponse<TransactionDashboardStatsDto>), 200)]
        [SwaggerOperation(Summary = "Endpoint to get stats for dashboard")]
        public async Task<IActionResult> CollectionsStats()
        {
            var response = await _collectionReportService.GetAllCollections();
            return Ok(response);
        }
    }
}
