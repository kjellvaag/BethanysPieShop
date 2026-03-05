# 🥧 Bethany's Pie Shop — Del 3: Test-Driven Development

> TDD og Testing med xUnit

---

## TDD — Test Driven Development

### Syklusen

```
    ┌──────────┐
    │  1. RED  │  Skriv en test som FEILER
    │  🔴      │  (koden finnes ikke ennå)
    └────┬─────┘
         │
         ▼
    ┌──────────┐
    │ 2. GREEN │  Skriv MINIMAL kode
    │ 🟢       │  for å passere testen
    └────┬─────┘
         │
         ▼
    ┌──────────┐
    │3.REFACTOR│  Forbedre koden
    │ 🔵       │  uten å ødelegge testen
    └────┬─────┘
         │
         └──────▶ Tilbake til 1. RED
```

### xUnit — Testrammeverket

```csharp
public class PieTests
{
    [Fact]                              // Markerer at dette er en test
    public void Pie_Name_ShouldNotBeEmpty()
    {
        // Arrange — Sett opp testdata
        var pie = new Pie { Name = "Apple Pie" };

        // Act — Utfør handlingen
        var name = pie.Name;

        // Assert — Sjekk resultatet
        Assert.False(string.IsNullOrEmpty(name));
    }
}
```

### AAA-mønsteret

Alle tester følger **Arrange-Act-Assert**:

| Steg | Hva | Eksempel |
|---|---|---|
| **Arrange** | Sett opp testdata og avhengigheter | `var pie = new Pie(...)` |
| **Act** | Utfør handlingen du vil teste | `var result = service.GetPie(1)` |
| **Assert** | Sjekk at resultatet er riktig | `Assert.Equal("Apple", result.Name)` |

---

## Domenemodeller og Repository-mønster

### Hva er domenemodeller?

Domenemodeller er C#-klasser som representerer de **viktige "tingene"** i applikasjonen din. 
For Bethany's Pie Shop er de viktigste "tingene" paier og kategorier.

Tenk på dem som **datamodeller** eller **entiteter** som beskriver:
- Hvilke egenskaper (properties) en pie har
- Hvordan paier og kategorier henger sammen
- Regler for hvordan dataene skal se ut

### Våre domenemodeller

#### Category-modellen

```csharp
public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Pie>? Pies { get; set; }  // Navigation property
}
```

| Property | Type | Forklaring |
|---|---|---|
| `CategoryId` | int | Unik ID for kategorien (primærnøkkel) |
| `CategoryName` | string | Navnet på kategorien ("Fruit pies", "Cheese cakes") |
| `Description` | string? | Valgfri beskrivelse av kategorien |
| `Pies` | List<Pie>? | Alle paier som tilhører denne kategorien |

#### Pie-modellen

```csharp
public class Pie
{
    public int PieId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? LongDescription { get; set; }
    public string? AllergyInformation { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public bool IsPieOfTheWeek { get; set; }
    public bool InStock { get; set; }
    public int CategoryId { get; set; }        // Foreign key
    public Category Category { get; set; } = default!;  // Navigation property
}
```

#### Navigation Properties (sammenhenger)

- **`Category.Pies`** — En kategori kan ha mange paier (one-to-many)
- **`Pie.Category`** — Hver pie tilhører én kategori 
- **`Pie.CategoryId`** — Foreign key som kobler til `Category.CategoryId`

```
Category "Fruit pies" (CategoryId=1)
    ├── Pie "Apple Pie" (CategoryId=1)
    ├── Pie "Strawberry Pie" (CategoryId=1)
    └── Pie "Rhubarb Pie" (CategoryId=1)

Category "Cheese cakes" (CategoryId=2)
    └── Pie "Cheese cake" (CategoryId=2)
```

---

### Repository-mønster

#### Hva er Repository-mønster?

Repository-mønster skiller **forretningslogikk** fra **datakilden**:

```
Controller → Repository (interface) → Konkret implementasjon (Mock/Database)
```

**Fordeler:**
- **Testbarhet** — Kan bytte ut databasen med mock-data i tester
- **Fleksibilitet** — Kan bytte fra Mock → Entity Framework → Azure SQL uten å endre controllere
- **Abstraksjon** — Controlleren bryr seg ikke om HVOR dataene kommer fra

#### Våre repositories

**ICategoryRepository** (grensesnittet):
```csharp
public interface ICategoryRepository
{
    IEnumerable<Category> AllCategories { get; }
}
```

**IPieRepository** (grensesnittet):
```csharp
public interface IPieRepository
{
    IEnumerable<Pie> AllPies { get; }
    IEnumerable<Pie> PiesOfTheWeek { get; }
    Pie? GetPieById(int pieId);
    IEnumerable<Pie> SearchPies(string searchQuery);
}
```

#### Mock-implementasjoner

I stedet for en ekte database bruker vi **Mock-implementasjoner** med hardkodede testdata:

```csharp
public class MockCategoryRepository : ICategoryRepository
{
    public IEnumerable<Category> AllCategories =>
        new List<Category>
        {
            new Category{CategoryId=1, CategoryName="Fruit pies", Description="All-fruity pies"},
            new Category{CategoryId=2, CategoryName="Cheese cakes", Description="Cheesy all the way"},
            new Category{CategoryId=3, CategoryName="Seasonal pies", Description="Get in the mood for a seasonal pie"}
        };
}
```

**Hvorfor Mock først?**
- Rask utvikling uten database-oppsett
- Lettere testing
- Kan fokusere på MVC-logikk før Entity Framework

---

### Dependency Injection

#### Hva er Dependency Injection?

I stedet for at en Controller lager sine egne avhengigheter:

```csharp
// ❌ Tight coupling - vanskelig å teste
public class PieController : Controller
{
    private readonly IPieRepository _pieRepository;
    
    public PieController()
    {
        _pieRepository = new MockPieRepository(); // Hardkodet!
    }
}
```

**Ber** den ASP.NET Core om å gi dem det de trenger:

```csharp
// ✅ Dependency Injection - testbart og fleksibelt
public class PieController : Controller
{
    private readonly IPieRepository _pieRepository;
    
    // Constructor Injection - ASP.NET gir oss repository automatisk
    public PieController(IPieRepository pieRepository)
    {
        _pieRepository = pieRepository;
    }
}
```

#### Konfigurasjon i Program.cs

```csharp
using BethanysPieShop.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Registrer repositories i DI-container
builder.Services.AddScoped<ICategoryRepository, MockCategoryRepository>();
builder.Services.AddScoped<IPieRepository, MockPieRepository>();

var app = builder.Build();
```

**`AddScoped`** betyr:
- Én instans per HTTP-request
- Samme instans deles mellom alle som trenger den i samme request
- Nye instanser for hver nye request

#### Magien

1. **Registrering**: `builder.Services.AddScoped<Interface, Implementation>()`
2. **Injeksjon**: Controller ber om `IPieRepository` i constructor
3. **Oppløsning**: ASP.NET Core ser i DI-containeren: "Ah, `IPieRepository` → `MockPieRepository`"
4. **Instansiering**: Lager `new MockPieRepository()` og gir til controlleren

---

### TDD for domenemodeller

#### Vår TDD-tilnærming

**Red-Green-Refactor** for hver komponent:

1. **RED**: Skriv test for Category-modell → FEIL (ingen Category.cs ennå)
2. **GREEN**: Lag Category.cs med minimum implementation → PASS
3. **REFACTOR**: Forbedre koden uten å ødelegge tester
4. Gjenta for Pie, repositories, osv.

#### Eksempel: Testing av Category

```csharp
[Fact]
public void Category_ShouldSetPropertiesCorrectly()
{
    // Arrange
    var testId = 1;
    var testName = "Test Category";
    var testDescription = "Test Description";
    
    // Act
    var category = new Category
    {
        CategoryId = testId,
        CategoryName = testName,
        Description = testDescription
    };
    
    // Assert
    Assert.Equal(testId, category.CategoryId);
    Assert.Equal(testName, category.CategoryName);
    Assert.Equal(testDescription, category.Description);
}
```

#### Testdekning

Vi har 28 tester som dekker:
- **Modell-properties** — Kan vi sette og hente verdier?
- **Repository-grensesnitt** — Implementerer Mock-klassene interface korrekt?
- **Forretningslogikk** — PiesOfTheWeek returnerer kun paier med `IsPieOfTheWeek = true`?
- **Søkefunksjonalitet** — SearchPies finner riktige paier?

---

## Viktige xUnit Assert-metoder

### Grunnleggende sammenligninger

```csharp
// Likhet
Assert.Equal(expected, actual);
Assert.NotEqual(notExpected, actual);

// Sannhet
Assert.True(condition);
Assert.False(condition);

// Null-sjekking
Assert.Null(value);
Assert.NotNull(value);

// Strenger
Assert.StartsWith("Hello", actualString);
Assert.EndsWith("World", actualString);
Assert.Contains("test", actualString);

// Collections
Assert.Empty(collection);
Assert.NotEmpty(collection);
Assert.Single(collection);  // Akkurat ett element
Assert.Contains(expectedItem, collection);
```

### Tester for exceptions

```csharp
[Fact]
public void GetPieById_InvalidId_ThrowsException()
{
    // Arrange
    var repository = new MockPieRepository();
    
    // Act & Assert
    Assert.Throws<ArgumentException>(() => repository.GetPieById(-1));
}
```

### Parametriserte tester

```csharp
[Theory]
[InlineData(1, "Strawberry Pie")]
[InlineData(2, "Cheese cake")]
[InlineData(3, "Rhubarb Pie")]
public void GetPieById_ValidId_ReturnsCorrectPie(int pieId, string expectedName)
{
    // Arrange
    var repository = new MockPieRepository();
    
    // Act
    var pie = repository.GetPieById(pieId);
    
    // Assert
    Assert.NotNull(pie);
    Assert.Equal(expectedName, pie.Name);
}
```

---

## Organisering av tester

### Filstruktur

```
Tests/
├── Models/
│   ├── CategoryTests.cs
│   ├── PieTests.cs
│   ├── MockCategoryRepositoryTests.cs
│   └── MockPieRepositoryTests.cs
├── Controllers/
│   ├── HomeControllerTests.cs
│   └── PieControllerTests.cs
└── Integration/
    └── PieListIntegrationTests.cs
```

### Navnekonvensjoner

```csharp
// Pattern: [UnitOfWork]_[Scenario]_[ExpectedResult]
public void GetPieById_ValidId_ReturnsCorrectPie()
public void GetPieById_InvalidId_ReturnsNull()
public void SearchPies_EmptyQuery_ReturnsAllPies()
public void SearchPies_MatchingQuery_ReturnsFilteredResults()
```

### Test-kategorier med xUnit

```csharp
[Trait("Category", "Unit")]
[Trait("Category", "Integration")]
[Trait("Component", "Repository")]
[Trait("Component", "Controller")]
```

Kjør spesifikke kategorier:
```bash
dotnet test --filter "Category=Unit"
dotnet test --filter "Component=Repository"
```

---

## TDD Best Practices

### 1. Start med det enkleste

```csharp
// ✅ Start enkelt
[Fact]
public void Pie_CanSetName()
{
    var pie = new Pie { Name = "Apple Pie" };
    Assert.Equal("Apple Pie", pie.Name);
}

// ❌ Ikke start med kompleks logikk
[Fact] 
public void CalculateDiscountedPrice_WithComplexBusinessRules_ReturnsCorrectAmount()
{
    // For kompleks til første test
}
```

### 2. En assertion per test

```csharp
// ✅ Fokusert test
[Fact]
public void Pie_Name_CanBeSet()
{
    var pie = new Pie { Name = "Apple Pie" };
    Assert.Equal("Apple Pie", pie.Name);
}

[Fact]
public void Pie_Price_CanBeSet()
{
    var pie = new Pie { Price = 12.95m };
    Assert.Equal(12.95m, pie.Price);
}

// ❌ For mange assertions
[Fact]
public void Pie_AllProperties_CanBeSet()
{
    var pie = new Pie { Name = "Apple", Price = 12.95m };
    Assert.Equal("Apple", pie.Name);      // Hvis denne feiler...
    Assert.Equal(12.95m, pie.Price);     // ...kjører ikke denne
}
```

### 3. Beskrivende testnavn

```csharp
// ✅ Klar og beskrivende
public void GetPieById_WithValidId_ReturnsMatchingPie()
public void GetPieById_WithNonExistentId_ReturnsNull()
public void SearchPies_WithEmptyString_ReturnsAllPies()

// ❌ Vage eller tekniske
public void TestPie()
public void GetPieByIdTest()
public void Method1()
```

### 4. Isolerte tester

```csharp
// ✅ Hver test setter opp sine egne data
[Fact]
public void GetPieById_Test1()
{
    var repository = new MockPieRepository(); // Fresh instance
    // test logic
}

[Fact]
public void GetPieById_Test2()
{
    var repository = new MockPieRepository(); // Fresh instance
    // test logic
}

// ❌ Delt tilstand mellom tester
private MockPieRepository _sharedRepository = new MockPieRepository();

[Fact]
public void Test1() { /* bruker _sharedRepository */ }

[Fact] 
public void Test2() { /* bruker samme _sharedRepository - kan påvirkes av Test1! */ }
```

---

## Debugging av tester

### Output i tester

```csharp
public class PieTests
{
    private readonly ITestOutputHelper _output;
    
    public PieTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void SearchPies_Debug()
    {
        var repository = new MockPieRepository();
        var results = repository.SearchPies("apple");
        
        _output.WriteLine($"Found {results.Count()} pies");
        foreach (var pie in results)
        {
            _output.WriteLine($"- {pie.Name}");
        }
        
        Assert.NotEmpty(results);
    }
}
```

### Vanlige feilsøkingsteknikker

```csharp
// 1. Sjekk faktiske vs forventede verdier
var actual = pie.Name;
Assert.Equal("Apple Pie", actual); // Gir klar feilmelding hvis feil

// 2. Bruk Collection assertions for bedre feilmeldinger  
Assert.Collection(pies,
    pie => Assert.Equal("Apple", pie.Name),
    pie => Assert.Equal("Cherry", pie.Name)
);

// 3. Custom error messages
Assert.True(pie.Price > 0, $"Price should be positive but was {pie.Price}");
```

---

### Neste steg

Fra her kan vi:
1. **Lage Controllers** som bruker repositories
2. **Lage Views** for å vise pai-data
3. **Bytte til Entity Framework** når vi vil ha ekte database
4. **Legge til flere features** (shopping cart, checkout, etc.)

Repository-mønsteret gjør overgangen smidig — vi bytter bare implementasjoner uten å endre Controllers!

---

**🔗 Neste:** [Del 4: Avanserte konsepter](kjorebok-del4.md) — Dependency Injection, Session og Shopping Cart

---

*TDD er en reise, ikke et mål. Start enkelt og bygg kompleksitet gradvis. Hver rød test er en mulighet til læring!*