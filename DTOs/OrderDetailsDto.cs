namespace OrderManagementApi.DTOs
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalValue { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<OrderItemDetailsDto> Items { get; set; } = new List<OrderItemDetailsDto>();
    }

    public class OrderItemDetailsDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
