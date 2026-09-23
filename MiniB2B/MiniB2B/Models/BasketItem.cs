namespace MiniB2B.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int BasketId { get; set; }
        public Basket? Basket { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}