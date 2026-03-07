namespace BethanysPieShop.Models;

public interface IShoppingCart
{
    IEnumerable<ShoppingCartItem> ShoppingCartItems { get; set; }

    void AddToCart(Pie pie);
    int RemoveFromCart(Pie pie);
    IEnumerable<ShoppingCartItem> GetShoppingCartItems();
    void ClearCart();
    decimal GetShoppingCartTotal();



}