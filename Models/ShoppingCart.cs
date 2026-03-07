using Microsoft.EntityFrameworkCore;

namespace BethanysPieShop.Models;

public class ShoppingCart : IShoppingCart
{
    private readonly BethanysPieShopDbContext _BPSContext;

    public string? ShoppingCartId { get; set; }
    public IEnumerable<ShoppingCartItem> ShoppingCartItems { get; set; }

    public ShoppingCart(BethanysPieShopDbContext _bpsCtx)
    {
        _BPSContext = _bpsCtx;
        ShoppingCartItems = new List<ShoppingCartItem>();
    }

    public static ShoppingCart GetCart(IServiceProvider services)
    {
        ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;

        var context = services.GetService<BethanysPieShopDbContext>() ?? throw new Exception("Error initializing BethanysPieShopDbContext");

        string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();

        session?.SetString("CartId", cartId);

        return new ShoppingCart(context) { ShoppingCartId = cartId };
    }

    
    public void AddToCart(Pie pie)
    {
        // Sjekk om pie allerede finnes i kurven
        var shoppingCartItem = _BPSContext.ShoppingCartItems.FirstOrDefault(
            s => s.PieId == pie.PieId && s.ShoppingCartId == ShoppingCartId);

        if (shoppingCartItem == null)
        {
            // Opprett ny cart item
            shoppingCartItem = new ShoppingCartItem
            {
                ShoppingCartId = ShoppingCartId,
                PieId = pie.PieId,
                Amount = 1
            };
            _BPSContext.ShoppingCartItems.Add(shoppingCartItem);
        }
        else
        {
            // Øk antallet
            shoppingCartItem.Amount++;
        }
        
        _BPSContext.SaveChanges();
    }

    public int RemoveFromCart(Pie pie)
    {
        // Finn eksisterende cart item
        var shoppingCartItem = _BPSContext.ShoppingCartItems.FirstOrDefault(
            s => s.PieId == pie.PieId && s.ShoppingCartId == ShoppingCartId);

        // Hvis item ikke finnes, returner 0
        if (shoppingCartItem == null)
        {
            return 0;
        }

        // Hvis Amount > 1, reduser med 1
        if (shoppingCartItem.Amount > 1)
        {
            shoppingCartItem.Amount--;
        }
        else
        {
            // Amount = 1, fjern item helt
            _BPSContext.ShoppingCartItems.Remove(shoppingCartItem);
        }

        _BPSContext.SaveChanges();
        return 1; // Alltid 1 item fjernet når metoden kommer hit
    }

    public IEnumerable<ShoppingCartItem> GetShoppingCartItems()
    {
        return _BPSContext.ShoppingCartItems
            .Where(s => s.ShoppingCartId == ShoppingCartId)
            .Include(s => s.Pie)
            .ToList();
    }
    public void ClearCart()
    {
        var cartItems = _BPSContext.ShoppingCartItems
            .Where(s => s.ShoppingCartId == ShoppingCartId)
            .ToList();

        _BPSContext.ShoppingCartItems.RemoveRange(cartItems);
        _BPSContext.SaveChanges();
    }

    public decimal GetShoppingCartTotal()
    {
        return _BPSContext.ShoppingCartItems
            .Where(s => s.ShoppingCartId == ShoppingCartId)
            .Select(s => s.Pie.Price * s.Amount)
            .Sum();
    }
}