using OrderManagementApi.Data.Repository;
using OrderManagementApi.DTOs;
using OrderManagementApi.Exceptions;
using OrderManagementApi.Service;
using OrderManagementApi.Validators;
using OrderManagementApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("order")]
    public class OrderController : Controller
    {
        #region Atributs

        private readonly OrderRepository _orderRepository;
        private readonly CreateOrderViewModelValidator _createOrderViewModelValidator;
        private readonly IValidationService _validationService;

        #endregion

        #region Constructor

        public OrderController(
            OrderRepository orderRepository,
            CreateOrderViewModelValidator createOrderViewModelValidator,
            IValidationService validationService)
        {
            _createOrderViewModelValidator = createOrderViewModelValidator;
            _orderRepository = orderRepository;
            _validationService = validationService;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderViewModel viewModel)
        {
            var (isValid, errorResponse) = await _validationService.ValidateAsync(viewModel, _createOrderViewModelValidator);
           
            if (!isValid) return BadRequest(errorResponse);

            try
            {
                CreateOrderDto createOrderDto = new CreateOrderDto
                {
                    ClientId = viewModel.ClientId,
                    Items = viewModel.Items.Select(i => new CreateOrderItemDto
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                };

                await _orderRepository.CreateOrderAsync(createOrderDto);
                return Created(string.Empty, new { Message = "Pedido criado com sucesso" });
            }
            catch (BadRequestException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro ao criar o pedido. {ex.Message}"});
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID inválido." });

            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro interno: {ex.Message}" });
            }
        }

        #endregion
    }
}
