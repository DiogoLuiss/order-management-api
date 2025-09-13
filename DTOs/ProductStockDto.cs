namespace OrderManagementApi.DTOs
{
    public class ProductStockDto
    {
        public bool Exists { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
