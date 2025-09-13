namespace OrderManagementApi.DTOs
{
    public class CreateOrderDto
    {
        public int ClientId { get; set; }
        public decimal TotalValue { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
