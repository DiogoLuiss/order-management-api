namespace OrderManagementApi.DTOs
{
    public class ProductSimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class ProductSimpleListResponseDto
    {
        public List<ProductSimpleDto> Products { get; set; } = new List<ProductSimpleDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
