namespace OrderManagementApi.DTOs
{
    public class ClientSimpleListResponseDto
    {
        public List<ClientSimpleDto> Clients { get; set; } = new List<ClientSimpleDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class ClientSimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
