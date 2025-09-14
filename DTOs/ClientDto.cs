namespace OrderManagementApi.DTOs
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ClientListResponseDto
    {
        public List<ClientDto> Clients { get; set; } = new List<ClientDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
