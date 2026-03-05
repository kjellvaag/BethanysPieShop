---

## 🔧 DbInitializer - Alternativ til Migration-basert Seeding

### Problemet med Migration-basert Seeding

Tidligere brukte vi `OnModelCreating` metoden i `DbContext` for å seede data, som ble inkludert i migreringer. Dette har noen ulemper:

❌ **Migrasjoner blir rotete** — Schema og data blandes sammen
❌ **Dårlig separasjon** — Strukturendringer og testdata kobles sammen
❌ **Vanskelig å vedlikeholde** — Seeding-kode blir del av database-skjema
❌ **MERGE-problemer** — Foreign key constraints kan feile

### Løsningen: DbInitializer

En **statisk DbInitializer-klasse** som håndterer seeding ved runtime i stedet for i migrasjoner:

```csharp
public static class DbInitializer
{
    public static void Seed(BethanysPieShopDbContext context)
    {
        // Sørg for at databasen er opprettet
        context.Database.EnsureCreated();

        // Sjekk om vi allerede har data (unngå duplikater)
        if (context.Categories.Any())
        {
            return; // Database er allerede seeded
        }

        // Seed kategorier FØRST
        var fruitPiesCategory = new Category 
        { 
            CategoryName = "Fruit pies", 
            Description = "Delicious fruit-based pies" 
        };
        // ... flere kategorier

        context.Categories.AddRange(fruitPiesCategory, cheeseCakesCategory, seasonalPiesCategory);
        context.SaveChanges(); // ⚡ VIKTIG: Generer CategoryIds

        // Seed pies ETTER kategorier (pga foreign keys)
        var pies = new Pie[]
        {
            new Pie
            {
                Name = "Apple Pie",
                CategoryId = fruitPiesCategory.CategoryId, // ✅ Bruk generert ID
                // ... resten av data
            },
            // ... flere pies
        };

        context.Pies.AddRange(pies);
        context.SaveChanges();
    }
}
```

### Integrering i Program.cs

```csharp
// Kall DbInitializer ved oppstart
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BethanysPieShopDbContext>();
    DbInitializer.Seed(context);
}
```

### 🔑 Kritiske Læringsmomenter

#### 1. Foreign Key Dependencies
```csharp
// ❌ FEIL: Hardkodede CategoryIds
CategoryId = 1, // Kan feile hvis ID genereres annerledes

// ✅ RIKTIG: Bruk genererte IDs
context.Categories.AddRange(categories);
context.SaveChanges(); // Generer IDs

var pie = new Pie 
{ 
    CategoryId = fruitPiesCategory.CategoryId // Bruk faktisk generert ID
};
```

#### 2. EF Core Identity Generation
- **Auto-generated IDs** er først tilgjengelige ETTER `SaveChanges()`
- **SaveChanges()** må kalles mellom parent og child entities
- **Database-genererte verdier** (IDENTITY, GUID) krever round-trip til database

#### 3. Duplikatsjekk
```csharp
// Sjekk om data allerede finnes
if (context.Categories.Any())
{
    return; // Unngå duplikater ved gjentatte oppstarter
}
```

### Fordeler med DbInitializer

✅ **Ren separasjon** — Schema (migrasjoner) vs. Data (runtime)
✅ **Enklere vedlikehold** — Seed-data i egen fil
✅ **Fleksibilitet** — Kan kjøre betinget basert på miljø
✅ **Ingen MERGE-problemer** — Proper foreign key håndtering
✅ **Testbarhet** — Kan kalles direkte i tester

### 🔄 Migrering til DbInitializer

**Steg 1:** Opprett DbInitializer.cs
**Steg 2:** Fjern seed-data fra DbContext.OnModelCreating
**Steg 3:** Legg til DbInitializer.Seed() i Program.cs
**Steg 4:** Opprett migrasjon for å fjerne existing seed data
**Steg 5:** Test at seeding fungerer ved runtime

### Resultat

- ✅ **Kompilerer:** Ingen feil eller advarsler
- ✅ **Tester:** Alle 56 tester passerer
- ✅ **Runtime:** Applikasjon starter uten feil
- ✅ **Seeding:** Data seedes korrekt ved første oppstart
- ✅ **Foreign keys:** Alle relasjoner fungerer perfekt

DbInitializer gir oss bedre kontroll over når og hvordan testdata seedes, og skiller tydelig mellom database-skjema og data-initialisering.