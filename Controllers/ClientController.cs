using OrderManagementApi.Data.Repository;
using OrderManagementApi.DTOs;
using OrderManagementApi.Service;
using OrderManagementApi.Validators;
using OrderManagementApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("client")]
    public class ClientController : Controller
    {       
        #region Attributes

        private readonly ClientRepository _clientRepository;
        private readonly IValidationService _validationService;
        private readonly CreateClientViewModelValidator _createClientViewModelValidator;
        private readonly UpdateClientViewModelValidator _updateClientViewModelValidator;

        #endregion

        #region Constructor

        public ClientController(
            ClientRepository clientRepository,
            IValidationService validationService,
            CreateClientViewModelValidator createClientViewModelValidator,
            UpdateClientViewModelValidator updateClientViewModelValidator)
        {
            _clientRepository = clientRepository;
            _validationService = validationService;
            _createClientViewModelValidator = createClientViewModelValidator;
            _updateClientViewModelValidator = updateClientViewModelValidator;
        }

        #endregion

        #region Methods

        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int id)
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID inválido." });

            try
            {
                ClientDto? client = await _clientRepository.GetClientByIdAsync(id);

                if (client == null)
                    return NotFound(new { Message = "Cliente não encontrado." });

                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro interno ao processar a requisição. {ex.Message}"});
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientViewModel viewModel)
        {
            if (viewModel == null)
                return BadRequest(new { Message = "Dados inválidos." });

            var (isValid, errorResponse) = await _validationService.ValidateAsync(viewModel, _createClientViewModelValidator);

            if (!isValid)
                return BadRequest(errorResponse);
            try
            {
                await _clientRepository.CreateClientAsync(viewModel.Name, viewModel.Email, viewModel.Phone);
                return Ok(new { Message = "Cliente criado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro ao criar cliente. {ex.Message}"});
            }
        }

        // PUT: Client/Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClientViewModel viewModel)
        {
            if (id <= 0)
                return BadRequest(new { Message = "O campo é id obrigatorio e deve ser válido." });

            var (isValid, errorResponse) = await _validationService.ValidateAsync(viewModel, _updateClientViewModelValidator);

            if (!isValid)
                return BadRequest(errorResponse);

            try
            {
                bool updated = await _clientRepository.UpdateClientAsync(id, viewModel.Name, viewModel.Email, viewModel.Phone);

                if (!updated)
                    return NotFound(new { Message = "Cliente não encontrado." });

                return Ok(new { Message = "Cliente atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro ao atualizar cliente. {ex.Message}"});
            }
        }

        // DELETE: Client/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID inválido." });

            try
            {
                bool deleted = await _clientRepository.DeleteClientAsync(id);
                if (!deleted)
                    return NotFound(new { Message = "Cliente não encontrado." });

                return Ok(new { Message = "Cliente deletado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro ao deletar cliente. {ex.Message}" });
            }
        }

        #endregion
    }
}
