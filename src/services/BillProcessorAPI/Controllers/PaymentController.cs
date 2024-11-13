using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Configs;
using BillProcessorAPI.Services.Implementations;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit.Cryptography;
using Swashbuckle.AspNetCore.Annotations;

namespace BillProcessorAPI.Controllers
{
    [ApiController]
    [Route("v1/payment")]
    [Produces("application/json")]
    public partial class PaymentController : ControllerBase
    {
        private readonly IPayThruService _paythruService;
        private readonly IFlutterwaveMgtService _flutterwaveMgtService;
        private readonly ILogger<PaymentController> _logger;
        private readonly PaymentConfirmationDelayInSec paymentConfirmationDelayInSec;
        private readonly ICutlyService _cutlyService;
        public PaymentController(IPayThruService paythruService,
            IFlutterwaveMgtService flutterwaveMgtService,
            ILogger<PaymentController> logger, IOptions<PaymentConfirmationDelayInSec> options, ICutlyService cutlyService)
        {
            _paythruService = paythruService;
            _flutterwaveMgtService = flutterwaveMgtService;
            _logger = logger;
            this.paymentConfirmationDelayInSec = options.Value;
            _cutlyService = cutlyService;
        }


        //[HttpGet("/generate-invoice-link/{billNumber}")]
        //[ProducesResponseType(typeof(PaymentCreationResponse), 200)]
        //[SwaggerOperation(Summary = "Endpoint to create transaction")]
        //public async Task<IActionResult> GeneratePaymentLink([FromRoute] string billNumber, [FromQuery]string phoneNumber)
        //{
        //    //throw new RestException(HttpStatusCode.ExpectationFailed, "Payment service temporarily unavailable");
        //    var response = await _cutlyService.GenerateShortenedPaymentLink(phoneNumber,billNumber);
        //    return Ok(response);
        //}
    }
}
