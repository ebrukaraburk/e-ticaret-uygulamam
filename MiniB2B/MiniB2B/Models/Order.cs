namespace MiniB2B.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = OrderStatus.Pending;

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}