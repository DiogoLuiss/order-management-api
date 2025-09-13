namespace OrderManagementApi.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
