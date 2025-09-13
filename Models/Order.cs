namespace OrderManagementApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalValue { get; set; }
        public short StatusId { get; set; }
        public Client? Client { get; set; }
        public OrderStatus? Status { get; set; }
        public List<ItemOrder> Items { get; set; } = new();
    }
}
