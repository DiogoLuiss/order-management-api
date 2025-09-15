using OrderManagementApi.Data.Repository;
using OrderManagementApi.Service;
using OrderManagementApi.Validators;
using OrderManagementApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("product")]
    public class ProductController : Controller
    {
        #region Attributes

        private readonly ProductRepository _productRepository;
        private readonly IValidationService _validationService;
        private readonly CreateAlterProductViewModelValidator _createAlterProductViewModelValidator;

        #endregion

        #region Constructor

        public ProductController(
            ProductRepository productRepository,
            IValidationService validationService,
            CreateAlterProductViewModelValidator createProductViewModelValidator)
        {
            _productRepository = productRepository;
            _validationService = validationService;
            _createAlterProductViewModelValidator = createProductViewModelValidator;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAlterProductViewModel viewModel)
        {
            var (isValid, errorResponse) = await _validationService.ValidateAsync(viewModel, _createAlterProductViewModelValidator);
            if (!isValid) return BadRequest(errorResponse);

            try
            {
                await _productRepository.CreateProductAsync(viewModel.Name, viewModel.Description, viewModel.Price, viewModel.StockQuantity);
                return Ok(new { Message = "Produto criado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro ao criar produto. {ex.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateAlterProductViewModel viewModel)
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID inválido." });

            var (isValid, errorResponse) = await _validationService.ValidateAsync(viewModel, _createAlterProductViewModelValidator);
            if (!isValid) return BadRequest(errorResponse);

            try
            {
                bool updated = await _productRepository.UpdateProductAsync(id, viewModel.Name, viewModel.Description, viewModel.Price, viewModel.StockQuantity);
                if (!updated) return NotFound(new { Message = "Produto não encontrado." });

                return Ok(new { Message = "Produto atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro ao atualizar produto. {ex.Message}" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID inválido." });

            try
            {
                bool deleted = await _productRepository.DeleteProductAsync(id);
                if (!deleted) return NotFound(new { Message = "Produto não encontrado." });

                return Ok(new { Message = "Produto deletado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro ao deletar produto. {ex.Message}"});
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID inválido." });

            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);

                if (product == null)
                    return NotFound(new { Message = "Produto não encontrado." });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro ao consultar produto. {ex.Message}" });
            }
        }

        #endregion
    }
}
