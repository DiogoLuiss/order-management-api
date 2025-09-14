using Microsoft.AspNetCore.Mvc;

namespace OrderManagementApi.ViewModel
{
    public class FilterClientsViewModel : ViewModelPagination
    {
        [FromQuery]
        public string? NameOrEmail { get; set; }
    }
}
