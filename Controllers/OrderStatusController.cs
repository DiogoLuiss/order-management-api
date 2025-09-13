using Microsoft.AspNetCore.Mvc;
using OrderManagementApi.Data.Repository;
using OrderManagementApi.DTOs;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("order/status")]
    public class OrderStatusController : Controller
    {
        #region Attributes

        private readonly OrderStatusRepository _orderStatusRepository;

        #endregion

        #region Constructor

        public OrderStatusController(OrderStatusRepository orderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetAllStatuses()
        {
            try
            {
                List<OrderStatusDto> statuses = await _orderStatusRepository.GetAllStatusesAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro ao consultar os status do pedido. {ex.Message}" });
            }
        }

        #endregion
    }
}
