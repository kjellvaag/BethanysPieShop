namespace BethanysPieShop.Models
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }
        public int PieId { get; set; } // Foreign key
        public Pie Pie { get; set; } = default!; // Navigation property
        public int Amount { get; set; }
        public string? ShoppingCartId { get; set; }
    }
}
