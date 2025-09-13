namespace OrderManagementApi.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalValue { get; set; }
        public string NomeClinete { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
