using System.ComponentModel.DataAnnotations;

namespace OrderManagementApi.ViewModel
{
    public class CreateOrderViewModel
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public List<CreateOrderItemViewModel> Items { get; set; } = new();
    }

    public class CreateOrderItemViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
