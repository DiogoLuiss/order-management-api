using OrderManagementApi.DTOs;
using OrderManagementApi.Service;
using OrderManagementApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using OrderManagementApi.Data.Repository;

namespace OrderManagementApi.Controllers
{

    [ApiController]
    [Route("clients")]
    public class ClientsController : Controller
    {
        #region Atributs

        private readonly ClientRepository _clientRepository;
        private readonly IValidationService _validationService;
        private readonly FilterClientsViewModelValidator _filterClientsViewModelValidator;

        #endregion

        #region Constructor

        public ClientsController(
            ClientRepository clientRepository,
            IValidationService validationService,
            FilterClientsViewModelValidator filterClientsViewModelValidator)
        {
            _clientRepository = clientRepository;
            _validationService = validationService;
            _filterClientsViewModelValidator = filterClientsViewModelValidator;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Index(FilterClientsViewModel viewModel)
        {
            var (isValid, errorResponse) = await _validationService.ValidateAsync(viewModel, _filterClientsViewModelValidator);

            if (!isValid)
                return BadRequest(errorResponse);

            try
            {
                List<ClientDto> clients = await _clientRepository.GetClientListAsync(viewModel.NameOrEmail);
                return PartialView("_ClientsGrid", clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro interno ao processar a requisição. {ex.Message}"});
            }
        }

        [HttpGet("simple")]
        public async Task<IActionResult> SimpleList(FilterClientsViewModel viewModel)
        {
            var (isValid, errorResponse) = await _validationService.ValidateAsync(viewModel, _filterClientsViewModelValidator);

            if (!isValid)
                return BadRequest(errorResponse);

            try
            {
                List<ClientSimpleDto> clients = await _clientRepository.GetClientListSimpleAsync(viewModel.NameOrEmail);
                return PartialView("_ClientsGridSimple", clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Erro interno ao processar a requisição. {ex.Message}" });
            }
        }

        #endregion
    }
}
