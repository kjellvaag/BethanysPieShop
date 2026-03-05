# 🥧 Bethany's Pie Shop — Kjørebok

> En læringsguide for ASP.NET Core MVC med C#, Git og TDD.
> Bygget steg for steg fra scratch.

---

## 📚 Innholdsfortegnelse — Delt i flere filer

### Del 1: Grunnleggende oppsett
📁 **[kjorebok-del1.md](kjorebok-del1.md)** — Git, Prosjektstruktur og ASP.NET Core MVC grunnleggende
- Git — Versjonskontroll og kommandoer
- Prosjektstruktur — Mapper og filer
- ASP.NET Core MVC — Oversikt over arkitekturen
- Program.cs — Oppstart og konfigurasjon

### Del 2: MVC Arkitektur
📁 **[kjorebok-del2.md](kjorebok-del2.md)** — Controllers, Views og Routing
- Routing — Hvordan URL-er kobles til kode  
- Controllers — Trafikk-dirigenten
- Views og Layout — HTML-sidene
- ViewModels — Dataoverføring mellom Controller og View

### Del 3: Test-Driven Development
📁 **[kjorebok-del3.md](kjorebok-del3.md)** — TDD og Testing
- TDD — Test Driven Development prinsippene
- Unit Testing med xUnit
- Red-Green-Refactor syklusen
- Mocking og Repository Pattern

### Del 4: Avanserte konsepter
📁 **[kjorebok-del4.md](kjorebok-del4.md)** — Dependency Injection, Session og Shopping Cart
- Dependency Injection i ASP.NET Core
- Session Management
- Shopping Cart implementering
- Entity Framework og Database

---

## 🚀 Hvor er vi nå?

**Ferdigstilt:**
- ✅ Fase 1: Prosjektoppsett og Git
- ✅ Fase 2: Domenemodeller og Repository Pattern  
- ✅ Fase 3: Controllers og Views (MVC implementering)
- ✅ Fase 4: Routes og Navigation (PieController med List/Details)

**Neste:**
- 🚧 Fase 5: Shopping Cart funksjonalitet
- ⏳ Fase 6: Checkout prosess
- ⏳ Fase 7: Authentication

---

## 🎯 Læringsmål per del

| Del | Fokus | Konsepter |
|-----|-------|-----------|
| Del 1 | Grunnleggende | Git workflow, ASP.NET Core setup, MVC pattern |
| Del 2 | Arkitektur | Controllers, Views, Routing, ViewModels |
| Del 3 | Testing | TDD, xUnit, Repository pattern, Mocking |
| Del 4 | Avansert | DI, Sessions, EF Core, Database |

**🎓 Tilnærming:** Hver del bygger på forrige, men kan også brukes som oppslag.

## ✅ Fase 5: Modul 06 - Seed Data fra Referanse (Ferdig)

### Mål
Erstatte eksisterende seed data med det komplette datasettet fra modul 06.

### Gjennomført
- **DbInitializer oppdatert** med alle 16 pies fra modul 06-referansen
- **3 kategorier**: Fruit pies, Cheese cakes, Seasonal pies
- **Dictionary-basert kategoritilnærming** for bedre referansehåndtering
- **Tvungen database-rensing** ved oppstart for fresh seed data
- **Komplett pie-liste**: Caramel Popcorn Cheese Cake, Chocolate Cheese Cake, Pistache Cheese Cake, osv.

### Teknisk løsning
```csharp
// Slett eksisterende data og seed på nytt ved hver oppstart
if (context.Pies.Any())
{
    context.Pies.RemoveRange(context.Pies);
    context.SaveChanges();
}
if (context.Categories.Any())
{
    context.Categories.RemoveRange(context.Categories);
    context.SaveChanges();
}

// Seed 3 kategorier + 16 pies
context.Categories.AddRange(Categories.Select(c => c.Value));
context.Pies.AddRange(/* 16 pies fra modul 06 */);
```

### Verifisering
- ✅ **16 pies vises** i `/Pie/List` (tidligere bare 5)
- ✅ **Alle 56 tester bestått**
- ✅ **Database seed fungerer** ved hver oppstart
- ✅ **Routing og navigasjon** fungerer med nytt datasett

### Læringspunkter
- **Entity Framework seed strategier**: `EnsureCreated()` vs `RemoveRange()`
- **SQL Server LocalDB**: Ikke .db filer, men `(localdb)\mssqllocaldb`
- **Dictionary pattern** for kategorireferanser i seed data
- **MERGE statements**: EF Core genererer effektive bulk inserts