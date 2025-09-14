using Microsoft.AspNetCore.Mvc;
using OrderManagementApi.Data.Repositories;
using OrderManagementApi.DTOs;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("statistics")]
    public class StatisticsController : ControllerBase
    {
        #region Attributes

        private readonly StatisticRepository _statisticRepository;

        #endregion

        #region Constructor

        public StatisticsController(StatisticRepository dataRepository)
        {
            _statisticRepository = dataRepository;
        }

        #endregion

        #region Methods

        [HttpGet("counts")]
        public async Task<IActionResult> GetCounts()
        {
            try
            {
                CountsDto counts = await _statisticRepository.GetCountsAsync();
                return Ok(counts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error. {ex.Message}" });
            }
        }

        #endregion
    }
}
