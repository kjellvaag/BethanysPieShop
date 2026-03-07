using BethanysPieShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Threading;
namespace BethanysPieShop.Tests;

public class ShoppingCartTests
{
    [Fact]
    public void  ShoppingCart_ShouldImplementIShoppingCart()
    {
        // Arrange

        // Arrange - Sett opp in-memory database
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var _context = new BethanysPieShopDbContext(options);

        var shoppingCart = new ShoppingCart(_context);
        
        // Act & Assert
        Assert.IsAssignableFrom<IShoppingCart>(shoppingCart);
    }

    [Fact] 
    public void ShoppingCart_ShouldHaveShoppingCartItemsProperty()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var _context = new BethanysPieShopDbContext(options);
        var shoppingCart = new ShoppingCart(_context);
        
        // Act & Assert
        Assert.NotNull(shoppingCart.ShoppingCartItems);
        Assert.IsAssignableFrom<IEnumerable<ShoppingCartItem>>(shoppingCart.ShoppingCartItems);
    }

    [Fact]
    public async Task ShoppingCart_GetCart_ShouldReturnSameCartIdForSameContext()
    {
        // Arrange - Sett opp kompleks session testing

        // Sett opp service collection for session support
        var services = new ServiceCollection();
        services.AddDbContext<BethanysPieShopDbContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddDistributedMemoryCache();
        services.AddLogging();
        services.AddSession();
        services.AddHttpContextAccessor();

        var serviceProvider = services.BuildServiceProvider();
        var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();

        // Opprett HttpContext med session support
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = serviceProvider;

        // Sett opp session med en unik session ID
        var sessionKey = "TestSession" + Guid.NewGuid().ToString();
        var session = new DistributedSession(
            distributedCache,
            sessionKey,
            TimeSpan.FromMinutes(30),
            TimeSpan.FromMinutes(1),
            () => true,
            serviceProvider.GetRequiredService<ILoggerFactory>(),
            true);

        // Last session og sett den på HttpContext
        await session.LoadAsync(CancellationToken.None);
        httpContext.Session = session;

        // Oppdater HttpContextAccessor i servicene med vår HttpContext
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = httpContext;

        // Act - Kall GetCart to ganger med samme service provider
        var cart1 = ShoppingCart.GetCart(serviceProvider);
        var cart2 = ShoppingCart.GetCart(serviceProvider);

        // Assert - Begge carts skal ha samme CartId siden de bruker samme session
        Assert.Equal(cart1.ShoppingCartId, cart2.ShoppingCartId);
        Assert.NotNull(cart1.ShoppingCartId);
        Assert.NotEmpty(cart1.ShoppingCartId);

        // Verifiser at CartId er lagret i session
        var storedCartId = httpContext.Session.GetString("CartId");
        Assert.Equal(cart1.ShoppingCartId, storedCartId);
        Assert.Equal(cart2.ShoppingCartId, storedCartId);
    }
}