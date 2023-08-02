using System;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.DashboardDtos;
using Application.Helpers;
using Application.Services.Interfaces.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gamawabs247API.Controllers
{
    
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/dashboard")]
    //[Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardStatisticsService _dashBoardStatService;
        public DashboardController(IDashboardStatisticsService dashBoardStatService)
        {
            _dashBoardStatService = dashBoardStatService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(SuccessResponse<GetStatisticsDto>), 200)]
        public async Task<IActionResult> GetStatistics()
        {
            var stats = await _dashBoardStatService.GetStatistics();
            return Ok(stats);
        }
    }
}
