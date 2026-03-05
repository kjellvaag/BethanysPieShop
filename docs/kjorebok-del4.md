# 🥧 Kjørebok Del 4: Entity Framework Core og Database

> **Avanserte konsepter:** Dependency Injection, Entity Framework Core og databaseintegrasjon

---

## 📚 Hva lærer vi i Del 4?

I denne delen går vi fra mock-data til ekte database ved hjelp av **Entity Framework Core**. Vi lærer:

- **Entity Framework Core** — Microsoft's moderne ORM (Object-Relational Mapping)
- **DbContext** — Hjertet i EF Core som administrerer databaseforbindelser
- **Code First** — Lage database fra kode i stedet for omvendt
- **Migrations** — Versjonskontroll for database-skjema
- **Repository Pattern med EF Core** — Erstatte mock-repositories med ekte datalagring
- **Connection Strings** — Konfigurasjon av databaseforbindelse
- **In-Memory Database** — For testing uten ekte database

---

## 🔍 Hva er Entity Framework Core?

**Entity Framework Core (EF Core)** er et moderne **Object-Relational Mapping (ORM)** verktøy for .NET. Det lar oss jobbe med databaser ved hjelp av C#-objekter i stedet for rå SQL.

### 🔄 Hvordan fungerer ORM?

```
┌─────────────────┐    ORM     ┌─────────────────┐
│   C# Objects    │ ←------→  │    Database     │
│                 │            │                 │
│ Pie pie = new() │            │ SELECT * FROM   │
│ Category cat    │            │ Pies WHERE...   │
│ List<Pie> pies  │            │ INSERT INTO...  │
└─────────────────┘            └─────────────────┘
```

**Fordeler med ORM:**
- ✅ Skriver C#-kode i stedet for SQL
- ✅ Type-sikkerhet og IntelliSense
- ✅ Automatisk håndtering av joins og relationer
- ✅ Migreringer for database-versjonering
- ✅ Enklere testing med in-memory database

---

## 🏗️ Code First Approach

**Code First** betyr at vi definerer modellene våre som C#-klasser først, og EF Core genererer databasestrukturen automatisk.

### Våre modeller blir til database-tabeller:

```csharp
// Models/Pie.cs (C#-klasse)
public class Pie 
{
    public int PieId { get; set; }      // → PRIMARY KEY
    public string Name { get; set; }    // → VARCHAR kolonne
    public decimal Price { get; set; }  // → DECIMAL kolonne
    public int CategoryId { get; set; } // → FOREIGN KEY
    public Category Category { get; set; } // → Navigation property
}
```

```sql
-- Generert SQL-tabell
CREATE TABLE Pies (
    PieId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(MAX),
    Price DECIMAL(18,2),
    CategoryId INT,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);
```

### 🔗 Relasjoner mellom tabeller:

- **One-to-Many**: En Category har mange Pies
- **Foreign Key**: `CategoryId` i Pies-tabellen refererer til `CategoryId` i Categories-tabellen
- **Navigation Properties**: `pie.Category` lar oss navigere fra Pie til Category

---

## 🎯 DbContext — Hjertet i EF Core

**DbContext** er klassen som styrer alt som har med databasen å gjøre:

```csharp
public class BethanysPieShopDbContext : DbContext
{
    public BethanysPieShopDbContext(DbContextOptions<BethanysPieShopDbContext> options) 
        : base(options) { }
    
    // DbSet = Tabell i databasen
    public DbSet<Pie> Pies { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    // Konfigurering av modeller
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Her kan vi fintuning relasjoner og constraints
    }
}
```

### DbSet forklart:

- `DbSet<Pie> Pies` = "Pies"-tabellen i databasen
- `DbSet<Category> Categories` = "Categories"-tabellen i databasen
- Vi kan spørre: `context.Pies.Where(p => p.Price < 20)`
- Vi kan legge til: `context.Pies.Add(newPie)`
- Vi kan lagre: `context.SaveChanges()`

---

## 🔄 Migrations — Database versjonering

**Migreringer** er EF Core's måte å versjonere database-skjemaet på.

### Hvorfor trenger vi migreringer?

1. **Første gang**: Lage tabellene fra scratch
2. **Endringer**: Legge til nye kolonner, tabeller eller indekser
3. **Samarbeid**: Alle utviklere får samme database-struktur
4. **Produksjon**: Trygg oppdatering av produksjonsdatabase

### Migration workflow:

```bash
# 1. Lage migrasjon (sammenligner modeller med database)
dotnet ef migrations add InitialCreate

# 2. Se hva som vil skje (optional)
dotnet ef migrations script

# 3. Kjøre migrasjon (oppdaterer databasen)
dotnet ef database update
```

### Hva skjer når vi kjører `dotnet ef migrations add`?

1. EF Core ser på våre modeller (Pie.cs, Category.cs)
2. Sammenligner med nåværende database (eller tom hvis første gang)  
3. Genererer C#-kode som beskriver endringene
4. Lagrer i `Migrations/`-mappen

### Migration-fil eksempel:

```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Hva som skal gjøres
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                CategoryId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.CategoryId);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Hvordan rulle tilbake
        migrationBuilder.DropTable(name: "Categories");
    }
}
```

---

## 🔧 Configuration og Connection Strings

### appsettings.json

Database-forbindelsen konfigureres i `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BethanysPieShopDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Program.cs

I `Program.cs` registrerer vi EF Core:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Registrer DbContext med Dependency Injection
builder.Services.AddDbContext<BethanysPieShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Resten av konfigurasjonen...
```

---

## 🏪 Repository Pattern med EF Core

Vi erstatter mock-repositories med ekte EF Core repositories:

### Før (Mock):
```csharp
public class MockPieRepository : IPieRepository
{
    private List<Pie> _pies = new List<Pie> { /* hardkodet data */ };
    
    public IEnumerable<Pie> AllPies => _pies;
}
```

### Etter (EF Core):
```csharp
public class PieRepository : IPieRepository  
{
    private readonly BethanysPieShopDbContext _context;
    
    public PieRepository(BethanysPieShopDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<Pie> AllPies => _context.Pies.Include(p => p.Category);
    
    public Pie? GetPieById(int pieId) => 
        _context.Pies.Include(p => p.Category).FirstOrDefault(p => p.PieId == pieId);
}
```

### Include() forklart:

- Uten `Include()`: Bare Pie-data, `pie.Category` vil være `null`
- Med `Include(p => p.Category)`: EF Core joiner automatisk med Categories-tabellen
- Dette kalles **Eager Loading** — vi laster relaterte data med en gang

---

## 🧪 Testing med In-Memory Database

For testing bruker vi **In-Memory Database** i stedet for ekte SQL Server:

```csharp
[Test]
public void GetAllPies_ShouldReturnPies()
{
    // Arrange - Lage in-memory database for testing
    var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options;
        
    using var context = new BethanysPieShopDbContext(options);
    
    // Seede testdata
    context.Pies.Add(new Pie { Name = "Test Pie", Price = 10 });
    context.SaveChanges();
    
    var repository = new PieRepository(context);
    
    // Act
    var pies = repository.AllPies.ToList();
    
    // Assert
    Assert.Single(pies);
    Assert.Equal("Test Pie", pies[0].Name);
}
```

---

## 🌱 Data Seeding

For å få testdata i databasen bruker vi **seeding**:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Category>().HasData(
        new Category { CategoryId = 1, CategoryName = "Fruit pies" },
        new Category { CategoryId = 2, CategoryName = "Cheese cakes" }
    );
    
    modelBuilder.Entity<Pie>().HasData(
        new Pie { PieId = 1, Name = "Apple Pie", Price = 12.95M, CategoryId = 1 },
        new Pie { PieId = 2, Name = "Blueberry Pie", Price = 15.95M, CategoryId = 1 }
    );
}
```

---

## 🔄 TDD med Entity Framework

### Red-Green-Refactor med EF Core:

1. **🔴 Red**: Skriv test som feiler (repository finnes ikke enda)
2. **🟢 Green**: Implementer EF Core repository som får testen til å passere  
3. **🔵 Refactor**: Forbedre koden (optimalisere queries, rydde opp)

### Eksempel TDD-syklus:

```csharp
// 🔴 Red - Test feiler fordi EF repository ikke finnes
[Test]
public void GetPieById_WithValidId_ShouldReturnPie() 
{
    var repository = new PieRepository(context); // Finnes ikke enda!
    var pie = repository.GetPieById(1);
    Assert.NotNull(pie);
}

// 🟢 Green - Implementer minimum for å passere test
public class PieRepository : IPieRepository
{
    public Pie GetPieById(int id) => _context.Pies.Find(id);
}

// 🔵 Refactor - Forbedre implementasjonen
public Pie GetPieById(int id) => 
    _context.Pies.Include(p => p.Category).FirstOrDefault(p => p.PieId == id);
```

---

## 📋 Migreringsprosess steg-for-steg

1. **Modeller klare** — Pie.cs og Category.cs finnes allerede ✅
2. **Legg til EF Core pakker** — NuGet installasjon
3. **Lag DbContext** — BethanysPieShopDbContext.cs  
4. **Konfigurer i Program.cs** — Connection string og DI
5. **Lag migrasjon** — `dotnet ef migrations add InitialCreate`
6. **Kjør migrasjon** — `dotnet ef database update`
7. **Implementer repositories** — Erstatt mock med EF Core
8. **Oppdater DI** — Registrer nye repositories
9. **Test** — Verifiser at alt fungerer

---

## ⚡ EF Core Performance Tips

### Viktige ytelsestips:

1. **AsNoTracking()** for read-only queries:
   ```csharp
   var pies = _context.Pies.AsNoTracking().ToList();
   ```

2. **Include() vs Select()** for relaterte data:
   ```csharp
   // Include - laster hele Category-objektet  
   var pies = _context.Pies.Include(p => p.Category).ToList();
   
   // Select - bare CategoryName
   var pieNames = _context.Pies.Select(p => new { p.Name, p.Category.CategoryName });
   ```

3. **Async/await** for database-operasjoner:
   ```csharp
   public async Task<IEnumerable<Pie>> GetAllPiesAsync()
   {
       return await _context.Pies.Include(p => p.Category).ToListAsync();
   }
   ```

---

## 🎯 Sammendrag

I Del 4 lærer vi:

| Konsept | Hva | Hvorfor |
|---------|-----|---------|
| **EF Core** | ORM for .NET | Enklere enn rå SQL, type-sikker |
| **Code First** | Modeller → Database | Versjonering, samarbeid |
| **DbContext** | Database-administrasjon | Sentralisert håndtering |
| **Migrations** | Database versjonering | Trygg schema-oppdatering |
| **Repository med EF** | Erstatter mock-data | Ekte datalagring |
| **In-Memory Testing** | Testing uten database | Rask, isolert testing |

**🔑 Nøkkelpunkt:** Vi går fra mock-data til ekte database, men beholder samme interface (IPieRepository) — dette er kraften i Repository Pattern!

---

## 🧪 TDD Green-fase: Implementasjon og testing (5. mars 2026)

### Test Discovery Problem løst!

**Problem:** `dotnet test --list-tests` feilet med recursive file copying errors.

**Årsak:** MSBuild prøvde å kopiere filer til seg selv i en sirkulær dependency:
```
C:\Tests\bin\Debug\net8.0\Tests\bin\Debug\net8.0\Tests\bin\Debug\net8.0\...
```

**Løsning:** Kjøre tester direkte fra DLL istedenfor via csproj:
```bash
# ❌ Dette feilet:
dotnet test Tests/BethanysPieShop.Tests.csproj

# ✅ Dette fungerte:
dotnet test Tests/bin/Debug/net8.0/BethanysPieShop.Tests.dll
```

### TDD Green-fase: Alle 56 tester passerer! 🎉

**Første testkjøring:**
- ✅ 54 tester passerte
- ❌ 2 tester feilet

### Testfeil og løsninger

#### 1. `AllCategories_ShouldReturnCategoriesInOrder` feilet

**Problem:** 
```
Expected: "Fruit pies"
Actual:   "Cheese cakes"
```

**Årsak:** `CategoryRepository` sorterte alfabetisk (`OrderBy(c => c.CategoryName)`), men testene forventet ID-rekkefølge.

**Alfabetisk rekkefølge:**
1. Cheese cakes
2. Chocolate pies  
3. Fruit pies

**Forventet rekkefølge (etter CategoryId):**
1. Fruit pies (ID=1)
2. Cheese cakes (ID=2)
3. Chocolate pies (ID=3)

**Løsning:** Endret sortering fra navn til ID:
```csharp
// Før:
public IEnumerable<Category> AllCategories => 
    _context.Categories.OrderBy(c => c.CategoryName);

// Etter:
public IEnumerable<Category> AllCategories => 
    _context.Categories.OrderBy(c => c.CategoryId);
```

#### 2. `SearchPies_CaseInsensitive_ShouldReturnMatchingPies` feilet

**Problem:** Søket etter `"APPLE"` returnerte tomt resultat, men skulle finne `"Apple Pie"`.

**Årsak:** `Contains()` er case-sensitive i EF Core!
```csharp
// Case-sensitive (❌):
p.Name.Contains("APPLE") // Finner ikke "Apple Pie"
```

**Løsning:** Konvertert begge til lowercase:
```csharp
// Før:
.Where(p => p.Name.Contains(searchQuery))

// Etter:
.Where(p => p.Name.ToLower().Contains(searchQuery.ToLower()))
```

### Viktige læringspunkter

| Område | Lærdom | Praktisk konsekvens |
|--------|--------|--------------------|
| **Testing** | In-memory DB bruker egne testdata | Tester er uavhengige av seed-data |
| **EF Core** | `Contains()` er case-sensitive | Må håndtere case-insensitive søk manuelt |
| **Sortering** | Default sorting kan variere | Eksplisitt sortering nødvendig for forutsigbarhet |
| **MSBuild** | Recursive copying kan skje | Kjør tester direkte fra DLL ved problemer |

### Endelig testresultat

```bash
Passed!  - Failed: 0, Passed: 56, Skipped: 0, Total: 56
```

**Test coverage:**
- ✅ 56/56 EF Core repository tester
- ✅ 37 PieRepository tester (CRUD, søk, filtering)
- ✅ 11 CategoryRepository tester (CRUD, relasjoner)
- ✅ 8 DbContext tester (konfigurasjon, seed-data)

---

**📍 Neste:** Vi fortsetter med shopping cart-funksjonalitet som nå kan lagres i database i stedet for session.

**📍 Neste:** Vi fortsetter med shopping cart-funksjonalitet som nå kan lagres i database i stedet for session.