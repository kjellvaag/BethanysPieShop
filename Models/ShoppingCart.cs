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
        throw new NotImplementedException();
    }

    public int RemoveFromCart(Pie pie)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ShoppingCartItem> GetShoppingCartItems()
    {
        throw new NotImplementedException();
    }

    public void ClearCart()
    {
        throw new NotImplementedException();
    }

    public decimal GetShoppingCartTotal()
    {
        throw new NotImplementedException();
    }
}