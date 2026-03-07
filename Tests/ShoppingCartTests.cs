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

    [Fact]
    public void AddToCart_WithNewPie_ShouldAddToCartItems()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        // Opprett en kategori først
        var category = new Category
        {
            CategoryId = 1,
            CategoryName = "Fruit pies",
            Description = "Our delicious fruit pies"
        };
        context.Categories.Add(category);

        // Opprett en pie
        var pie = new Pie
        {
            PieId = 1,
            Name = "Apple Pie",
            Price = 12.95M,
            ShortDescription = "Delicious apple pie",
            LongDescription = "A delicious apple pie made with fresh apples",
            CategoryId = 1,
            ImageUrl = "applePie.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "applePieThumb.jpg",
            AllergyInformation = "Contains gluten"
        };
        context.Pies.Add(pie);
        context.SaveChanges();

        var shoppingCart = new ShoppingCart(context)
        {
            ShoppingCartId = "test-cart-id"
        };

        // Act
        shoppingCart.AddToCart(pie);

        // Assert
        var cartItem = context.ShoppingCartItems.FirstOrDefault(c => c.ShoppingCartId == "test-cart-id" && c.PieId == 1);
        Assert.NotNull(cartItem);
        Assert.Equal(1, cartItem.Amount);
        Assert.Equal(pie.PieId, cartItem.PieId);
    }

    [Fact]
    public void AddToCart_WithExistingPie_ShouldIncrementAmount()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        // Opprett kategori og pie
        var category = new Category { CategoryId = 1, CategoryName = "Fruit pies", Description = "Delicious" };
        var pie = new Pie
        {
            PieId = 1,
            Name = "Apple Pie",
            Price = 12.95M,
            ShortDescription = "Delicious",
            LongDescription = "A delicious apple pie",
            CategoryId = 1,
            ImageUrl = "applePie.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "applePieThumb.jpg",
            AllergyInformation = "Contains gluten"
        };

        context.Categories.Add(category);
        context.Pies.Add(pie);

        // Legg til eksisterende cart item
        var existingCartItem = new ShoppingCartItem
        {
            ShoppingCartId = "test-cart-id",
            PieId = 1,
            Amount = 2
        };
        context.ShoppingCartItems.Add(existingCartItem);
        context.SaveChanges();

        var shoppingCart = new ShoppingCart(context)
        {
            ShoppingCartId = "test-cart-id"
        };

        // Act
        shoppingCart.AddToCart(pie);

        // Assert
        var cartItem = context.ShoppingCartItems.FirstOrDefault(c => c.ShoppingCartId == "test-cart-id" && c.PieId == 1);
        Assert.NotNull(cartItem);
        Assert.Equal(3, cartItem.Amount); // Skulle øke fra 2 til 3
    }

    [Fact]
    public void RemoveFromCart_WithExistingItemQuantityGreaterThanOne_ShouldDecrementAmount()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        // Opprett kategori og pie
        var category = new Category { CategoryId = 1, CategoryName = "Fruit pies", Description = "Delicious" };
        var pie = new Pie
        {
            PieId = 1,
            Name = "Apple Pie",
            Price = 12.95M,
            ShortDescription = "Delicious",
            LongDescription = "A delicious apple pie",
            CategoryId = 1,
            ImageUrl = "applePie.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "applePieThumb.jpg",
            AllergyInformation = "Contains gluten"
        };

        context.Categories.Add(category);
        context.Pies.Add(pie);

        // Legg til cart item med Amount = 3
        var cartItem = new ShoppingCartItem
        {
            ShoppingCartId = "test-cart-id",
            PieId = 1,
            Amount = 3
        };
        context.ShoppingCartItems.Add(cartItem);
        context.SaveChanges();

        var shoppingCart = new ShoppingCart(context)
        {
            ShoppingCartId = "test-cart-id"
        };

        // Act
        var removedAmount = shoppingCart.RemoveFromCart(pie);

        // Assert
        var updatedCartItem = context.ShoppingCartItems.FirstOrDefault(c => c.ShoppingCartId == "test-cart-id" && c.PieId == 1);
        Assert.NotNull(updatedCartItem);
        Assert.Equal(2, updatedCartItem.Amount); // Skulle øke ned fra 3 til 2
        Assert.Equal(1, removedAmount); // Returnerer antall fjernet
    }

    [Fact]
    public void RemoveFromCart_WithExistingItemQuantityOne_ShouldRemoveItemCompletely()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        // Opprett kategori og pie
        var category = new Category { CategoryId = 1, CategoryName = "Fruit pies", Description = "Delicious" };
        var pie = new Pie
        {
            PieId = 1,
            Name = "Apple Pie",
            Price = 12.95M,
            ShortDescription = "Delicious",
            LongDescription = "A delicious apple pie",
            CategoryId = 1,
            ImageUrl = "applePie.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "applePieThumb.jpg",
            AllergyInformation = "Contains gluten"
        };

        context.Categories.Add(category);
        context.Pies.Add(pie);

        // Legg til cart item med Amount = 1
        var cartItem = new ShoppingCartItem
        {
            ShoppingCartId = "test-cart-id",
            PieId = 1,
            Amount = 1
        };
        context.ShoppingCartItems.Add(cartItem);
        context.SaveChanges();

        var shoppingCart = new ShoppingCart(context)
        {
            ShoppingCartId = "test-cart-id"
        };

        // Act
        var removedAmount = shoppingCart.RemoveFromCart(pie);

        // Assert
        var updatedCartItem = context.ShoppingCartItems.FirstOrDefault(c => c.ShoppingCartId == "test-cart-id" && c.PieId == 1);
        Assert.Null(updatedCartItem); // Item skal være helt fjernet
        Assert.Equal(1, removedAmount); // Returnerer antall fjernet
    }

    [Fact]
    public void RemoveFromCart_WithNonExistentItem_ShouldReturnZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        // Opprett kategori og pie
        var category = new Category { CategoryId = 1, CategoryName = "Fruit pies", Description = "Delicious" };
        var pie = new Pie
        {
            PieId = 1,
            Name = "Apple Pie",
            Price = 12.95M,
            ShortDescription = "Delicious",
            LongDescription = "A delicious apple pie",
            CategoryId = 1,
            ImageUrl = "applePie.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "applePieThumb.jpg",
            AllergyInformation = "Contains gluten"
        };

        context.Categories.Add(category);
        context.Pies.Add(pie);
        context.SaveChanges();

        var shoppingCart = new ShoppingCart(context)
        {
            ShoppingCartId = "test-cart-id"
        };

        // Act - forsøk å fjerne pie som ikke finnes i kurven
        var removedAmount = shoppingCart.RemoveFromCart(pie);

        // Assert
        Assert.Equal(0, removedAmount); // Skal returnere 0 da item ikke finnes
    }

    [Fact]
    public void GetShoppingCartItems_WithEmptyCart_ShouldReturnEmptyList()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        var shoppingCart = new ShoppingCart(context)
        {
            ShoppingCartId = "test-cart-id"
        };

        // Act
        var cartItems = shoppingCart.GetShoppingCartItems();

        // Assert
        Assert.NotNull(cartItems);
        Assert.Empty(cartItems);
    }

    [Fact]
    public void GetShoppingCartItems_WithMultipleItems_ShouldReturnAllItemsWithPieIncluded()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        // Opprett kategorier og pies
        var category1 = new Category { CategoryId = 1, CategoryName = "Fruit pies", Description = "Delicious" };
        var category2 = new Category { CategoryId = 2, CategoryName = "Cheese cakes", Description = "Creamy" };
        var pie1 = new Pie
        {
            PieId = 1,
            Name = "Apple Pie",
            Price = 12.95M,
            ShortDescription = "Delicious",
            LongDescription = "A delicious apple pie",
            CategoryId = 1,
            ImageUrl = "applePie.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "applePieThumb.jpg",
            AllergyInformation = "Contains gluten"
        };
        var pie2 = new Pie
        {
            PieId = 2,
            Name = "Cheese Cake",
            Price = 18.95M,
            ShortDescription = "Creamy",
            LongDescription = "A delicious cheese cake",
            CategoryId = 2,
            ImageUrl = "cheeseCake.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "cheeseCakeThumb.jpg",
            AllergyInformation = "Contains dairy"
        };

        context.Categories.AddRange(category1, category2);
        context.Pies.AddRange(pie1, pie2);

        // Legg til cart items
        var cartItem1 = new ShoppingCartItem
        {
            ShoppingCartId = "test-cart-id",
            PieId = 1,
            Amount = 2
        };
        var cartItem2 = new ShoppingCartItem
        {
            ShoppingCartId = "test-cart-id",
            PieId = 2,
            Amount = 1
        };
        context.ShoppingCartItems.AddRange(cartItem1, cartItem2);
        context.SaveChanges();

        var shoppingCart = new ShoppingCart(context)
        {
            ShoppingCartId = "test-cart-id"
        };

        // Act
        var cartItems = shoppingCart.GetShoppingCartItems().ToList();

        // Assert
        Assert.NotNull(cartItems);
        Assert.Equal(2, cartItems.Count);
        
        // Sjekk første item
        var firstItem = cartItems.FirstOrDefault(c => c.PieId == 1);
        Assert.NotNull(firstItem);
        Assert.Equal(2, firstItem.Amount);
        Assert.NotNull(firstItem.Pie); // Navigation property skal være lastet
        Assert.Equal("Apple Pie", firstItem.Pie.Name);
        Assert.Equal(12.95M, firstItem.Pie.Price);

        // Sjekk andre item
        var secondItem = cartItems.FirstOrDefault(c => c.PieId == 2);
        Assert.NotNull(secondItem);
        Assert.Equal(1, secondItem.Amount);
        Assert.NotNull(secondItem.Pie); // Navigation property skal være lastet
        Assert.Equal("Cheese Cake", secondItem.Pie.Name);
        Assert.Equal(18.95M, secondItem.Pie.Price);
    }

    [Fact]
    public void GetShoppingCartItems_WithDifferentCartIds_ShouldReturnOnlyCurrentCartItems()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        // Opprett kategori og pie
        var category = new Category { CategoryId = 1, CategoryName = "Fruit pies", Description = "Delicious" };
        var pie = new Pie
        {
            PieId = 1,
            Name = "Apple Pie",
            Price = 12.95M,
            ShortDescription = "Delicious",
            LongDescription = "A delicious apple pie",
            CategoryId = 1,
            ImageUrl = "applePie.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "applePieThumb.jpg",
            AllergyInformation = "Contains gluten"
        };

        context.Categories.Add(category);
        context.Pies.Add(pie);

        // Legg til cart items for forskjellige cart IDer
        var cartItem1 = new ShoppingCartItem
        {
            ShoppingCartId = "test-cart-id-1",
            PieId = 1,
            Amount = 2
        };
        var cartItem2 = new ShoppingCartItem
        {
            ShoppingCartId = "test-cart-id-2",
            PieId = 1,
            Amount = 3
        };
        context.ShoppingCartItems.AddRange(cartItem1, cartItem2);
        context.SaveChanges();

        var shoppingCart = new ShoppingCart(context)
        {
            ShoppingCartId = "test-cart-id-1"
        };

        // Act
        var cartItems = shoppingCart.GetShoppingCartItems().ToList();

        // Assert
        Assert.NotNull(cartItems);
        Assert.Single(cartItems); // Bare en item for denne cart ID
        Assert.Equal(2, cartItems.First().Amount);
        Assert.Equal("test-cart-id-1", cartItems.First().ShoppingCartId);
    }

    // ClearCart Method Tests
    [Fact]
    public void ClearCart_WithItemsInCart_ShouldRemoveAllItems()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        // Opprett kategori og pie
        var category = new Category { CategoryId = 1, CategoryName = "Fruit pies", Description = "Delicious" };
        var pie1 = new Pie
        {
            PieId = 1,
            Name = "Apple Pie",
            Price = 12.95M,
            ShortDescription = "Delicious",
            LongDescription = "A delicious apple pie",
            CategoryId = 1,
            ImageUrl = "applePie.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "applePie.jpg",
            AllergyInformation = ""
        };

        var pie2 = new Pie
        {
            PieId = 2,
            Name = "Cherry Pie",
            Price = 15.95M,
            ShortDescription = "Sweet",
            LongDescription = "A sweet cherry pie",
            CategoryId = 1,
            ImageUrl = "cherryPie.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "cherryPie.jpg",
            AllergyInformation = ""
        };

        context.Categories.Add(category);
        context.Pies.Add(pie1);
        context.Pies.Add(pie2);
        context.SaveChanges();

        var shoppingCart = new ShoppingCart(context) { ShoppingCartId = "test-cart-clear" };

        // Legg til items i handlekurven
        shoppingCart.AddToCart(pie1);
        shoppingCart.AddToCart(pie2);
        shoppingCart.AddToCart(pie1); // Add pie1 again for Amount = 2

        // Verify items exist before clearing
        var itemsBeforeClear = shoppingCart.GetShoppingCartItems();
        Assert.Equal(2, itemsBeforeClear.Count()); // 2 different pies

        // Act
        shoppingCart.ClearCart();

        // Assert
        var itemsAfterClear = shoppingCart.GetShoppingCartItems();
        Assert.Empty(itemsAfterClear);
        
        // Verify no items in database for this cart
        var dbItems = context.ShoppingCartItems
            .Where(s => s.ShoppingCartId == "test-cart-clear")
            .ToList();
        Assert.Empty(dbItems);
    }

    [Fact]
    public void ClearCart_WithEmptyCart_ShouldNotThrowException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);
        var shoppingCart = new ShoppingCart(context) { ShoppingCartId = "empty-cart-clear" };

        // Act & Assert - should not throw exception
        shoppingCart.ClearCart();

        var items = shoppingCart.GetShoppingCartItems();
        Assert.Empty(items);
    }

    // GetShoppingCartTotal Method Tests
    [Fact]
    public void GetShoppingCartTotal_WithEmptyCart_ShouldReturnZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);
        var shoppingCart = new ShoppingCart(context) { ShoppingCartId = "empty-total-cart" };

        // Act
        var total = shoppingCart.GetShoppingCartTotal();

        // Assert
        Assert.Equal(0m, total);
    }

    [Fact]
    public void GetShoppingCartTotal_WithSingleItem_ShouldReturnCorrectTotal()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        var category = new Category { CategoryId = 1, CategoryName = "Fruit pies", Description = "Delicious" };
        var pie = new Pie
        {
            PieId = 1,
            Name = "Apple Pie",
            Price = 12.95M,
            ShortDescription = "Delicious",
            LongDescription = "A delicious apple pie",
            CategoryId = 1,
            ImageUrl = "applePie.jpg",
            InStock = true,
            IsPieOfTheWeek = false,
            ImageThumbnailUrl = "applePieThumb.jpg",
            AllergyInformation = "Contains gluten"
        };

        context.Categories.Add(category);
        context.Pies.Add(pie);
        context.ShoppingCartItems.Add(new ShoppingCartItem
        {
            ShoppingCartId = "total-cart",
            PieId = 1,
            Amount = 2
        });
        context.SaveChanges();

        var shoppingCart = new ShoppingCart(context) { ShoppingCartId = "total-cart" };

        // Act
        var total = shoppingCart.GetShoppingCartTotal();

        // Assert
        Assert.Equal(25.90M, total); // 12.95 * 2 = 25.90
    }

    [Fact]
    public void GetShoppingCartTotal_WithMultipleItems_ShouldSumAllItemTotals()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BethanysPieShopDbContext(options);

        var category = new Category { CategoryId = 1, CategoryName = "Fruit pies", Description = "Delicious" };
        var pie1 = new Pie
        {
            PieId = 1, Name = "Apple Pie", Price = 12.95M,
            ShortDescription = "Delicious", LongDescription = "A pie",
            CategoryId = 1, ImageUrl = "pie.jpg", InStock = true,
            IsPieOfTheWeek = false, ImageThumbnailUrl = "pieThumb.jpg", AllergyInformation = ""
        };
        var pie2 = new Pie
        {
            PieId = 2, Name = "Cherry Pie", Price = 15.00M,
            ShortDescription = "Sweet", LongDescription = "A pie",
            CategoryId = 1, ImageUrl = "pie2.jpg", InStock = true,
            IsPieOfTheWeek = false, ImageThumbnailUrl = "pie2Thumb.jpg", AllergyInformation = ""
        };

        context.Categories.Add(category);
        context.Pies.AddRange(pie1, pie2);
        context.ShoppingCartItems.AddRange(
            new ShoppingCartItem { ShoppingCartId = "multi-total-cart", PieId = 1, Amount = 3 },  // 3 * 12.95 = 38.85
            new ShoppingCartItem { ShoppingCartId = "multi-total-cart", PieId = 2, Amount = 1 }   // 1 * 15.00 = 15.00
        );
        context.SaveChanges();

        var shoppingCart = new ShoppingCart(context) { ShoppingCartId = "multi-total-cart" };

        // Act
        var total = shoppingCart.GetShoppingCartTotal();

        // Assert
        Assert.Equal(53.85M, total); // 38.85 + 15.00 = 53.85
    }
}
