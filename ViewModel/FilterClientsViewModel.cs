using Microsoft.AspNetCore.Mvc;

namespace OrderManagementApi.ViewModel
{
    public class FilterClientsViewModel
    {
        [FromQuery]
        public string? NameOrEmail { get; set; }
    }
}
