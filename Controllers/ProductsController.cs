using OrderManagementApi.Data.Repository;
using OrderManagementApi.DTOs;
using OrderManagementApi.Service;
using OrderManagementApi.Validators;
using OrderManagementApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : Controller
    {
        #region Attributes

        public readonly ProductRepository _productRepository;
        private readonly IValidationService _validationService;
        private readonly FilterProductsViewModelValidator _filterProductsViewModelValidator;

        #endregion

        #region Constructor

        public ProductsController(ProductRepository productRepository, IValidationService validationService, FilterProductsViewModelValidator filterProductsViewModelValidator)
        {
            _validationService = validationService;
            _productRepository = productRepository;
            _filterProductsViewModelValidator = filterProductsViewModelValidator;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Index([FromQuery] FilterProductsViewModel viewModel)
        {
            var (isValid, errorResponse) = await _validationService.ValidateAsync(viewModel, _filterProductsViewModelValidator);

            if (!isValid)
                return BadRequest(errorResponse);

            try
            {
                List<ProductDto> products = await _productRepository.GetProductListAsync(viewModel.Name);
                return PartialView("_ProductsGrid", products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro interno ao processar a requisição. {ex.Message}" });
            }
        }

        [HttpGet("simple")]
        public async Task<IActionResult> Get([FromQuery] FilterProductsViewModel viewModel)
        {
            var (isValid, errorResponse) = await _validationService.ValidateAsync(viewModel, _filterProductsViewModelValidator);

            if (!isValid)
                return BadRequest(errorResponse);

            try
            {
                var products = await _productRepository.GetSimpleProductListAsync(viewModel.Name);
                return PartialView("_ProductsGridSimple", products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro ao consultar produtos. {ex.Message}" });
            }
        }

        #endregion
    }
}
