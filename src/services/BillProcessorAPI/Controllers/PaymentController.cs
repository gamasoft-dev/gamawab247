using BillProcessorAPI.Services.Implementations;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillProcessorAPI.Controllers
{
    [ApiController]
    [Route("v1/payment")]
    [Produces("application/json")]
    public partial class PaymentController : ControllerBase
    {
        private readonly IPayThruService _paythruService;
        private readonly IFlutterwaveService _transactionService;
        public PaymentController(IPayThruService paythruService, IFlutterwaveService transactionService)
        {
            _paythruService = paythruService;
            _transactionService = transactionService;
        }
    }
}
