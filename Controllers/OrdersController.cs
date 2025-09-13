using Microsoft.AspNetCore.Mvc;
using OrderManagementApi.Data.Repository;
using OrderManagementApi.Service;
using OrderManagementApi.Validators;
using OrderManagementApi.ViewModel;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : Controller
    {
        #region Attributes

        private readonly OrderRepository _orderRepository;
        private readonly IValidationService _validationService;
        private readonly FilterOrdersViewModelValidator _filterOrdersViewModelValidator;

        #endregion

        #region Constructor

        public OrdersController(
            OrderRepository orderRepository,
            IValidationService validationService,
            FilterOrdersViewModelValidator filterOrdersViewModelValidator)
        {
            _orderRepository = orderRepository;
            _validationService = validationService;
            _filterOrdersViewModelValidator = filterOrdersViewModelValidator;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Index view with filter
        /// </summary>
        public async Task<IActionResult> Index([FromQuery] FilterOrdersViewModel viewModel)
        {
            var (isValid, errorResponse) = await _validationService.ValidateAsync(viewModel, _filterOrdersViewModelValidator);

            if (!isValid)
                return BadRequest(errorResponse);

            try
            {
                var orders = await _orderRepository.GetOrdersAsync(viewModel.ClientName,  viewModel.OrderStatusId);
                return PartialView("_OrdersGrid", orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal error while processing request. {ex.Message}" });
            }
        }

        #endregion
    }
}
