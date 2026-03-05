# 🥧 Kjørebok Del 2: MVC Arkitektur

> Controllers, Views, Routing og ViewModels

[← Tilbake til hovedoversikt](kjorebok.md)

---

## 🎯 Hva lærer vi i Del 2?

- **Controllers** — Trafikk-dirigenten i MVC
- **Views** — HTML-sidene med Razor syntax  
- **Routing** — Hvordan URL-er kobles til kode
- **ViewModels** — Dataoverføring mellom Controller og View
- **Layout og Bootstrap** — Konsistent design

---

## Controllers — Trafikk-dirigenten

### Hva er en Controller?

En **Controller** er "trafikk-dirigenten" i MVC-arkitekturen. Den:
- Mottar HTTP-forespørsler (GET, POST, osv.)
- Henter data fra modeller/repositories
- Sender data til Views
- Returnerer HTTP-respons tilbake til browseren

```csharp
public class PieController : Controller
{
    private readonly IPieRepository _pieRepository;

    public PieController(IPieRepository pieRepository)
    {
        _pieRepository = pieRepository;  // Dependency Injection
    }

    public ViewResult List()
    {
        var pies = _pieRepository.AllPies;
        var viewModel = new PieListViewModel
        {
            Pies = pies,
            CurrentCategory = "All Pies"
        };
        return View(viewModel);  // Sender data til View
    }
}
```

### Controller Konvensjoner

| Konvensjon | Forklaring | Eksempel |
|------------|------------|----------|
| Navn | Slutter alltid med "Controller" | `PieController` |
| Arv | Arver fra `Controller` base class | `: Controller` |
| Actions | Public metoder = Actions | `List()`, `Details()` |
| Return types | `ViewResult`, `IActionResult`, osv. | `public ViewResult List()` |

### Action Metoder

En **Action** er en public metode i en Controller:

```csharp
// GET: /Pie/List
public ViewResult List()
{
    // Hent data
    // Send til View
}

// GET: /Pie/Details/5
public ViewResult Details(int id)
{
    var pie = _pieRepository.GetPieById(id);
    return View(pie);
}
```

---

## Views — HTML-sidene

### Razor Syntax

**Razor** lar oss blande C# og HTML:

```html
@model PieListViewModel

<h1>@Model.CurrentCategory</h1>

@foreach (var pie in Model.Pies)
{
    <div class="card">
        <h2>@pie.Name</h2>
        <p>@pie.Price.ToString("c")</p>
    </div>
}
```

### Viktige Razor Konsepter

| Syntax | Forklaring | Eksempel |
|--------|------------|----------|
| `@model` | Definerer type data fra Controller | `@model PieListViewModel` |
| `@Model` | Tilgang til data fra Controller | `@Model.CurrentCategory` |
| `@{}` | C# kode-blokk | `@{ var total = 10; }` |
| `@()` | C# uttrykk | `@(pie.Price * 1.25)` |
| `@@` | Literal @ tegn | `@@username` → `@username` |

### View Struktur

```
Views/
├── Shared/
│   ├── _Layout.cshtml      ← Master layout
│   └── _ViewImports.cshtml ← Global imports
├── Home/
│   └── Index.cshtml        ← Home/Index action
└── Pie/
    ├── List.cshtml         ← Pie/List action
    └── Details.cshtml      ← Pie/Details action
```

### Layout og Sections

**_Layout.cshtml** definerer felles struktur:

```html
<!DOCTYPE html>
<html>
<head>
    <title>@ViewData["Title"] - Bethany's Pie Shop</title>
    <link rel="stylesheet" href="~/css/site.css" />
</head>
<body>
    <nav><!-- Navigation --></nav>
    
    <main>
        @RenderBody()  <!-- Her kommer View-innholdet -->
    </main>
    
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

---

## Routing — URL til Kode

### Standard Routing

ASP.NET Core bruker konvensjon-basert routing:

```
URL: /Controller/Action/Id
     /Pie/Details/1
      ↓
Controller: PieController
Action: Details(int id)
Parameter: id = 1
```

### Route Konfigurering

I `Program.cs`:

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
```

**Forklaring:**
- `{controller=Home}` — Default controller = HomeController
- `{action=Index}` — Default action = Index()
- `{id?}` — Valgfri id parameter

### Route Eksempler

| URL | Controller | Action | Parameter |
|-----|------------|--------|-----------|
| `/` | Home | Index | - |
| `/Pie` | Pie | Index | - |
| `/Pie/List` | Pie | List | - |
| `/Pie/Details/3` | Pie | Details | id=3 |

---

## ViewModels — Dataoverføring

### Hvorfor ViewModels?

**Problem:** Views trenger ofte data fra flere kilder eller i annet format enn modellene.

**Løsning:** ViewModels — spesielle klasser som pakker data for Views.

```csharp
public class PieListViewModel
{
    public IEnumerable<Pie> Pies { get; set; }
    public string CurrentCategory { get; set; }
}
```

### ViewModel Pattern

```
Controller ─────┐
               ├─→ ViewModel ─→ View
Repository ─────┘
```

1. **Controller** henter data fra Repository
2. **Controller** pakker data i ViewModel  
3. **Controller** sender ViewModel til View
4. **View** bruker ViewModel til å lage HTML

### Eksempel: HomeController

```csharp
public ViewResult Index()
{
    // Hent data fra repository
    var piesOfTheWeek = _pieRepository.PiesOfTheWeek;
    
    // Pakk i ViewModel
    var homeViewModel = new HomeViewModel(piesOfTheWeek);
    
    // Send til View
    return View(homeViewModel);
}
```

---

## Bootstrap og CSS

### Bootstrap Grid System

```html
<div class="row row-cols-1 row-cols-md-3 g-4">
    @foreach (var pie in Model.Pies)
    {
        <div class="col">
            <div class="card pie-card">
                <img src="@pie.ImageThumbnailUrl" class="card-img-top">
                <div class="card-body">
                    <h2>@pie.Name</h2>
                    <p>@pie.Price.ToString("c")</p>
                </div>
            </div>
        </div>
    }
</div>
```

**Forklaring:**
- `row-cols-1` — 1 kolonne på små skjermer
- `row-cols-md-3` — 3 kolonner på medium+ skjermer  
- `g-4` — Gap mellom kolonner
- `card` — Bootstrap kort-komponent

### Custom CSS

I `wwwroot/css/site.css`:

```css
.pie-card {
    border: none;
}

.pie-button {
    position: relative;
    padding: 0px;
}

.pie-link {
    text-decoration: none;
}
```

---

## Tag Helpers

### Hva er Tag Helpers?

**Tag Helpers** gjør HTML mer "smart" ved å koble det til C# kode:

```html
<!-- Vanlig HTML -->
<a href="/Pie/Details/1">Strawberry Pie</a>

<!-- Med Tag Helper -->
<a asp-controller="Pie" asp-action="Details" asp-route-id="1">Strawberry Pie</a>
```

### Vanlige Tag Helpers

| Tag Helper | Forklaring | Eksempel |
|------------|------------|----------|
| `asp-controller` | Spesifiserer controller | `asp-controller="Pie"` |
| `asp-action` | Spesifiserer action | `asp-action="Details"` |
| `asp-route-id` | Sender parameter | `asp-route-id="@pie.PieId"` |
| `asp-for` | Kobler til modell property | `asp-for="Name"` |

### Aktivering av Tag Helpers

I `_ViewImports.cshtml`:

```csharp
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

---

## Dependency Injection i Controllers

### Hva er Dependency Injection?

**Problem:** Controller trenger tilgang til Repository, men skal ikke lage den selv.

**Løsning:** ASP.NET Core "injiserer" Repository automatisk.

```csharp
public class PieController : Controller
{
    private readonly IPieRepository _pieRepository;

    // ASP.NET Core sender inn IPieRepository automatisk
    public PieController(IPieRepository pieRepository)
    {
        _pieRepository = pieRepository;
    }
}
```

### Registrering i Program.cs

```csharp
builder.Services.AddScoped<IPieRepository, MockPieRepository>();
```

**Forklaring:**
- Når noen ber om `IPieRepository`
- Send `MockPieRepository` instance
- `AddScoped` = en instance per HTTP request

---

## Komplett Flyten: Request til Response

```
1. Browser sender: GET /Pie/List

2. ASP.NET Core Routing:
   ┌─────────────────────────────────┐
   │ URL: /Pie/List                  │
   │ ↓                               │
   │ Controller: PieController       │  
   │ Action: List()                  │
   └─────────────────────────────────┘

3. Dependency Injection:
   ┌─────────────────────────────────┐
   │ new PieController(              │
   │   new MockPieRepository()       │
   │ )                               │
   └─────────────────────────────────┘

4. Controller Action:
   ┌─────────────────────────────────┐
   │ public ViewResult List()        │
   │ {                               │
   │   var pies = _pieRepository     │
   │     .AllPies;                   │
   │                                 │
   │   var viewModel = new           │
   │     PieListViewModel {          │
   │       Pies = pies,              │
   │       CurrentCategory = "All"   │
   │     };                          │
   │                                 │
   │   return View(viewModel);       │
   │ }                               │
   └─────────────────────────────────┘

5. View Engine:
   ┌─────────────────────────────────┐
   │ Finner: Views/Pie/List.cshtml   │
   │ Sender: PieListViewModel data   │
   │ Prosesserer: Razor syntax       │
   │ Genererer: HTML                 │
   └─────────────────────────────────┘

6. Browser får: HTML response
```

---

## Praktiske Eksempler

### Full PieController Implementering

```csharp
using BethanysPieShop.Models;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BethanysPieShop.Controllers
{
    public class PieController : Controller
    {
        private readonly IPieRepository _pieRepository;

        public PieController(IPieRepository pieRepository)
        {
            _pieRepository = pieRepository;
        }

        public ViewResult List()
        {
            var pieListViewModel = new PieListViewModel
            {
                Pies = _pieRepository.AllPies,
                CurrentCategory = "All Pies"
            };

            return View(pieListViewModel);
        }

        public ViewResult Details(int id)
        {
            var pie = _pieRepository.GetPieById(id);
            
            if (pie == null)
                return NotFound();

            return View(pie);
        }
    }
}
```

### Komplett Views/Pie/List.cshtml

```html
@model PieListViewModel

<h1>@Model.CurrentCategory</h1>

<div class="row row-cols-1 row-cols-md-3 g-4">
    @foreach (var pie in Model.Pies)
    {
        <div class="col">
            <div class="card pie-card">
                <img src="@pie.ImageThumbnailUrl" 
                     class="card-img-top" 
                     alt="@pie.Name">
                <div class="card-body pie-button">
                    <div class="d-flex justify-content-between mt-2">
                        <h2 class="text-start">
                            <a asp-controller="Pie" 
                               asp-action="Details" 
                               asp-route-id="@pie.PieId" 
                               class="pie-link">
                               @pie.Name
                            </a>
                        </h2>
                        <h5 class="text-nowrap">
                            @pie.Price.ToString("c")
                        </h5>
                    </div>
                </div>
            </div>
        </div>
    }
</div>
```

---

## Feilsøking og Vanlige Problemer

### 1. "Could not find view" feil

**Problem:**
```
InvalidOperationException: The view 'List' was not found.
```

**Løsninger:**
- Sjekk at view-filen eksisterer: `Views/Pie/List.cshtml`
- Sjekk at controller heter `PieController`
- Sjekk at action heter `List()`

### 2. Model binding feil

**Problem:**
```
NullReferenceException på @Model.Pies
```

**Løsninger:**
- Sjekk at Controller sender rett ViewModel: `return View(viewModel);`
- Sjekk at View har rett `@model` directive: `@model PieListViewModel`

### 3. Tag Helper virker ikke

**Problem:**
```html
<a asp-controller="Pie"> virker ikke som lenke
```

**Løsninger:**
- Sjekk `_ViewImports.cshtml` har: `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`
- Restart applikasjonen etter endring i `_ViewImports.cshtml`

---

## Neste Steg

Du har nå lært:
- ✅ Controller-arkitektur og Actions
- ✅ View-struktur og Razor syntax  
- ✅ Routing og URL-håndtering
- ✅ ViewModels for dataoverføring
- ✅ Bootstrap og responsive design

**Neste:** [Del 3 - Test-Driven Development](kjorebok-del3.md)

[← Tilbake til hovedoversikt](kjorebok.md)