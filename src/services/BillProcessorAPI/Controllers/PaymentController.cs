using BillProcessorAPI.Dtos.Configs;
using BillProcessorAPI.Services.Implementations;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
        public PaymentController(IPayThruService paythruService,
            IFlutterwaveMgtService flutterwaveMgtService,
            ILogger<PaymentController> logger, IOptions<PaymentConfirmationDelayInSec> options)
        {
            _paythruService = paythruService;
            _flutterwaveMgtService = flutterwaveMgtService;
            _logger = logger;
            this.paymentConfirmationDelayInSec = options.Value;
        }
    }
}
