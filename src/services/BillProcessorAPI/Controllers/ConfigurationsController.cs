using BillProcessorAPI.Dtos;
using BillProcessorAPI.Services.Implementations;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BillProcessorAPI.Controllers
{
    [Route("api/charge")]
    [ApiController]
    public class ConfigurationsController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationsController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpPost("/luc/calculate")]
        [ProducesResponseType(typeof(ChargesResponseDto), 200)]
        [SwaggerOperation(Summary = "Endpoint to calculate charges on payable bill amount")]
        public IActionResult CalculateBillChargesOnAmount(LucChargesInputDto input)
        {
            var response = _configurationService.CalculateBillChargesOnAmount(input);
            return Ok(response);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(ChargesResponseDto), 200)]
        [SwaggerOperation(Summary = "Endpoint to add charges to the database")]
        public async Task<IActionResult> CreateBillCharges(CreateBillChargeInputDto input)
        {
            var response = await _configurationService.CreateBillCharges(input);
            return Ok(response);
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<ChargesResponseDto>), 200)]
        [SwaggerOperation(Summary = "Description: Endpoint get all charges from the db")]
        public async Task<IActionResult> GetBillCharges()
        {
            var response = await _configurationService.GetBillCharges();
            return Ok(response);
        }

        [HttpGet("business/{businessId}")]
        [ProducesResponseType(typeof(IEnumerable<ChargesResponseDto>), 200)]
        [SwaggerOperation(Summary = "Description: Endpoint get all charges from the db")]
        public async Task<IActionResult> GetBillCharges(Guid businessId)
        {
            var response = await _configurationService.GetBillChargesByBusiness(businessId);
            return Ok(response);
        }
    }
}
