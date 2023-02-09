using BillProcessorAPI.Dtos;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillProcessorAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RevpayController : ControllerBase
	{
		private readonly IRevpayService _revpay;
		public RevpayController(IRevpayService revpay)
		{
			_revpay = revpay;
		}

		[HttpPost]
		public async Task<IActionResult> ReferenceVerification(BillReferenceRequestDto model)
		{
			var response = await _revpay.ReferenceVerification(model);
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> PaymentVerification(PaymentVerificationRequestDto model)
		{
			var response = await _revpay.PaymentVerification(model);
			return Ok(response);
		}
	}
}
