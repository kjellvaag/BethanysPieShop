# 🥧 Bethany's Pie Shop — Kjørebok

> En læringsguide for ASP.NET Core MVC med C#, Git og TDD.
> Bygget steg for steg fra scratch.

---

## Innholdsfortegnelse

1. [Git — Grunnleggende](#git--grunnleggende)
2. [Prosjektstruktur](#prosjektstruktur)
3. [ASP.NET Core MVC — Oversikt](#aspnet-core-mvc--oversikt)
4. [Program.cs — Oppstart og konfigurasjon](#programcs--oppstart-og-konfigurasjon)
5. [Routing — Hvordan URL-er kobles til kode](#routing--hvordan-url-er-kobles-til-kode)
6. [Controllers — Trafikk-dirigenten](#controllers--trafikk-dirigenten)
7. [Views og Layout — HTML-sidene](#views-og-layout--html-sidene)
8. [TDD — Test Driven Development](#tdd--test-driven-development)
9. [Prosjektfilen (.csproj)](#prosjektfilen-csproj)
10. [Git-kommandoer — Oppslagsverk](#git-kommandoer--oppslagsverk)

---

## Git — Grunnleggende

### Hva er Git?

Git er et **versjonskontrollsystem**. Tenk på det som "save game"-systemet for koden din.
Hver **commit** er et lagringspunkt du kan gå tilbake til.
En **repository** (repo) er mappen git overvåker.

### De tre sonene i Git

```
┌───────────────────┐    git add     ┌───────────────────┐   git commit   ┌───────────────────┐
│  Working Directory │ ────────────▶ │   Staging Area     │ ────────────▶ │    Repository      │
│  (filene dine)     │               │   (klare til lagr.)│               │  (lagret historikk)│
└───────────────────┘               └───────────────────┘               └───────────────────┘
```

- **Working Directory** — Filene dine slik de er nå på disken
- **Staging Area** (index) — Filer du har "valgt ut" for neste commit
- **Repository** — Den lagrede historikken med alle commits

**Flyten er:** endre filer → `git add` (staging) → `git commit` (lagre)

Tenk på det som å pakke en pakke: Du velger hva som skal i pakken (`add`), og så sender du den (`commit`).

### Viktige begreper

| Begrep | Forklaring |
|---|---|
| **Repository (repo)** | Mappen git overvåker, inkludert all historikk |
| **Commit** | Et lagringspunkt — et "snapshot" av koden |
| **Branch** | En "gren" av koden — du kan ha flere parallelle versjoner |
| **main** | Hoved-branchen (tidligere kalt `master`) |
| **Staging** | Å velge ut hvilke endringer som skal med i neste commit |
| **Untracked** | Filer git ser, men ikke sporer ennå (nye filer) |
| **Hash** | Unik ID for en commit, f.eks. `0d64e70` |

---

## Prosjektstruktur

```
BethanysPieShop/
├── .gitignore                  ← Filer git skal ignorere (bin/, obj/, .vs/)
├── global.json                 ← Låser .NET SDK til versjon 8
├── BethanysPieShop.sln         ← Solution-fil (samler alle prosjekter)
│
├── BethanysPieShop/            ← Web-applikasjonen (MVC)
│   ├── Controllers/        ← Håndterer HTTP-requests (C#)
│   ├── Models/             ← Datamodeller (C# klasser)
│   ├── Views/              ← HTML-sider (Razor .cshtml)
│   │   ├── Home/           ← Views for HomeController
│   │   └── Shared/         ← Delte views (_Layout, _Error)
│   ├── wwwroot/            ← Statiske filer (CSS, JS, bilder)
│   ├── Program.cs          ← Oppstart og konfigurasjon
│   └── appsettings.json    ← App-konfigurasjon
│
└── BethanysPieShop.Tests/      ← Unit tests (xUnit, TDD)
```

### Hvorfor denne strukturen?

- **Prosjektnavnene skiller** — `BethanysPieShop` vs `BethanysPieShop.Tests` gjør `src/` og `tests/` overflødige
- **Solution-filen (.sln)** — Samler alle prosjekter slik at IDE-en (Visual Studio / Rider) kan åpne alt på én gang
- **global.json** — Sikrer at alle som jobber med prosjektet bruker samme .NET SDK-versjon
- **`.gitignore`** — Hindrer at byggefiler (`bin/`, `obj/`), IDE-filer (`.vs/`) og andre genererte filer spores i git
---

## ASP.NET Core MVC — Oversikt

### Hva er MVC?

MVC står for **Model-View-Controller** — et designmønster som deler applikasjonen i tre lag:

```
BRUKER skriver URL: /Pie/List
        │
        ▼
┌──────────────┐
│   ROUTING    │  Program.cs: {controller}/{action}/{id?}
│  /Pie/List   │  → PieController, action: List
└──────┬───────┘
       │
       ▼
┌──────────────┐
│  CONTROLLER  │  PieController.cs
│  List()      │  → Håndterer request, henter data
└──────┬───────┘
       │ sender data til
       ▼
┌──────────────┐
│    MODEL     │  Pie.cs, Category.cs
│  (Data)      │  → Dataobjekter med properties
└──────┬───────┘
       │ brukes av
       ▼
┌──────────────┐
│    VIEW      │  Views/Pie/List.cshtml
│  (HTML)      │  → Viser data som HTML til brukeren
└──────────────┘
```

| Lag | Ansvar | Eksempel |
|---|---|---|
| **Model** | Data og forretningslogikk | `Pie.cs` med Name, Price osv. |
| **View** | Presentasjon (HTML) | `List.cshtml` som viser pai-kort |
| **Controller** | Koordinering mellom Model og View | `PieController.cs` som henter paier og sender til view |

---

## Program.cs — Oppstart og konfigurasjon

`Program.cs` er filen som starter hele applikasjonen. Den har to hoveddeler:

### Del 1: Konfigurere tjenester (Services)

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(); // Registrer MVC
var app = builder.Build();
```

**Dependency Injection Container** — `builder.Services` er en "boks" der du registrerer alle tjenestene appen trenger. ASP.NET Core bruker dette til å automatisk gi controllere og andre klasser det de trenger (f.eks. en logger, en database-kobling, osv.)

### Del 2: Konfigurere HTTP-pipeline (Middleware)

```csharp
app.UseHttpsRedirection();   // Tving HTTPS
app.UseStaticFiles();        // Server CSS, JS, bilder fra wwwroot/
app.UseRouting();             // Aktiver URL-routing
app.UseAuthorization();       // Sjekk tilgangsrettigheter
app.MapControllerRoute(...);  // Definer rute-mønster
app.Run();                    // Start serveren!
```

**Middleware** er som et samlebånd. Hver `app.Use...()` legger til et steg som HTTP-requesten passerer gjennom, i rekkefølge:

```
Request inn ──▶ HTTPS ──▶ StaticFiles ──▶ Routing ──▶ Auth ──▶ Controller
                                                                    │
Response ut ◀──────────────────────────────────────────────────◀────┘
```

---

## Routing — Hvordan URL-er kobles til kode

### Standard rute-mønster

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

Mønsteret `{controller=Home}/{action=Index}/{id?}` betyr:

| URL | Controller | Action (metode) | id |
|---|---|---|---|
| `/` | HomeController | Index() | — |
| `/Home/Index` | HomeController | Index() | — |
| `/Home/Privacy` | HomeController | Privacy() | — |
| `/Pie/List` | PieController | List() | — |
| `/Pie/Details/5` | PieController | Details(int id) | 5 |

- `=Home` og `=Index` er **standardverdier** — brukes hvis ingenting er oppgitt
- `id?` — spørsmålstegnet betyr at `id` er **valgfri**

---

## Controllers — Trafikk-dirigenten

En controller er en C#-klasse som arver fra `Controller`. Hver offentlig metode er en **action** som svarer på en URL.

```csharp
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    // Constructor Injection — ASP.NET gir oss loggeren automatisk
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Action: Svarer på GET /Home/Index (eller bare /)
    public IActionResult Index()
    {
        return View(); // Finn og vis Views/Home/Index.cshtml
    }
}
```

### Viktige konsepter

- **`Controller`-baseklassen** — Gir tilgang til `View()`, `RedirectToAction()`, m.m.
- **Constructor Injection** — Parametere i constructoren fylles automatisk av DI-containeren
- **`IActionResult`** — Returtypen for actions. Kan være en View, Redirect, JSON, osv.
- **`return View()`** — Leter etter en `.cshtml`-fil i `Views/{ControllerNavn}/{ActionNavn}.cshtml`

---

## Views og Layout — HTML-sidene

### Razor (.cshtml)

Razor er en syntaks som blander C# og HTML. Filer har endelsen `.cshtml`.

```html
@{
    ViewData["Title"] = "Home Page";  // C#-kode i @{ }
}

<div class="text-center">
    <h1>Welcome to @ViewData["Title"]</h1>  <!-- @-tegnet for å bruke C# i HTML -->
</div>
```

### Layout-mønsteret (som en matrioshka-dukke)

`_Layout.cshtml` er en **mal** som alle sider deler. Den inneholder header, navigasjon og footer:

```
┌─────────────────────────────────┐
│ _Layout.cshtml (ytre ramme)     │
│ ┌─────────────────────────────┐ │
│ │ Navbar (navigasjon)         │ │
│ ├─────────────────────────────┤ │
│ │                             │ │
│ │   @RenderBody()             │ │
│ │   ↓ ↓ ↓                    │ │
│ │   Innholdet fra den         │ │
│ │   aktuelle siden settes     │ │
│ │   inn her automatisk!       │ │
│ │                             │ │
│ ├─────────────────────────────┤ │
│ │ Footer                      │ │
│ └─────────────────────────────┘ │
└─────────────────────────────────┘
```

### Tag Helpers

I stedet for vanlige HTML-lenker bruker vi ASP.NET Tag Helpers:

```html
<!-- Vanlig HTML (hardkodet URL, kan brekke) -->
<a href="/Home/Privacy">Privacy</a>

<!-- Tag Helper (genererer riktig URL automatisk) -->
<a asp-controller="Home" asp-action="Privacy">Privacy</a>
```

Tag Helpers er tryggere fordi de genererer URL-er basert på routing-konfigurasjonen.

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

## Prosjektfilen (.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

| Innstilling | Forklaring |
|---|---|
| `Sdk="Microsoft.NET.Sdk.Web"` | Web-prosjekt — inkluderer ASP.NET Core automatisk |
| `TargetFramework` | Hvilken .NET-versjon prosjektet kompileres mot |
| `Nullable=enable` | Kompilatoren advarer om potensielle null-referanser (tryggere kode!) |
| `ImplicitUsings=enable` | Vanlige `using`-statements inkluderes automatisk (System, System.Linq, osv.) |

---

## Git-kommandoer — Oppslagsverk

### Grunnleggende

| Kommando | Hva den gjør |
|---|---|
| `git init` | Opprett nytt repository i gjeldende mappe |
| `git add .` | Stage ALLE endringer (nye, endrede, slettede filer) |
| `git add filnavn` | Stage én spesifikk fil |
| `git status` | Se hva som er endret, staged, og untracked |
| `git commit -m "melding"` | Lagre staged endringer med en beskjed |
| `git log --oneline` | Vis commit-historikk (kompakt) |
| `git log` | Vis commit-historikk (detaljert) |
| `git diff` | Vis endringer som IKKE er staged ennå |

### Branch-håndtering

| Kommando | Hva den gjør |
|---|---|
| `git branch` | Vis alle branches |
| `git branch -M main` | Gi nytt navn til nåværende branch |

### .NET CLI

| Kommando | Hva den gjør |
|---|---|
| `dotnet new sln -n Navn` | Opprett solution-fil |
| `dotnet new mvc -n Navn -o mappe` | Opprett MVC-prosjekt |
| `dotnet new xunit -n Navn -o mappe` | Opprett xUnit testprosjekt |
| `dotnet sln add prosjekt.csproj` | Legg prosjekt til i solution |
| `dotnet add reference prosjekt.csproj` | Legg til prosjekt-referanse |
| `dotnet build` | Bygg prosjektet |
| `dotnet test` | Kjør alle tester |
| `dotnet run` | Start applikasjonen |

---

*Denne kjøreboken oppdateres fortløpende etter hvert som vi bygger Bethany's Pie Shop.*

## GitHub — Koble prosjektet til GitHub

### Installasjon og autentisering

1. **Installer GitHub CLI** — `winget install --id GitHub.cli`
2. **Autentiser** — `gh auth login`
   - Følg instruksjonene i terminalvinduet
   - Åpner en nettleser hvor du logger inn på GitHub
   - Kopierer en engangskode fra terminalen

### Opprette og koble repository

```bash
# 1. Opprett GitHub-repo
gh repo create BethanysPieShop --public --description "ASP.NET Core MVC læringsprosjekt - Bethany's Pie Shop"

# 2. Legg til remote origin
git remote add origin https://github.com/kjellvaag/BethanysPieShop.git

# 3. Konfigurer Git til å bruke GitHub CLI for autentisering
gh auth setup-git

# 4. Push lokale commits til GitHub
git push -u origin main
```

### Hva skjedde?

- **GitHub-repo opprettet** — Nå finnes prosjektet på https://github.com/kjellvaag/BethanysPieShop
- **Remote origin lagt til** — Git vet nå hvor det skal pushe til
- **Commits pushet** — Alle lokale commits (prosjektoppsett, `.gitignore`, etc.) er nå på GitHub
- **Upstream tracking** — `git push -u origin main` setter `main`-branchen til å tracke `origin/main`

| Git-kommando | Hva den gjør |
|---|---|
| `git remote add origin <URL>` | Kobler lokalt repo til GitHub-repo |
| `git push -u origin main` | Pusher `main`-branch og setter upstream tracking |
| `git push` | (fremover) Pusher til origin/main automatisk |

---
