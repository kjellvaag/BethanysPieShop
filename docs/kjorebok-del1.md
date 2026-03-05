# 🥧 Bethany's Pie Shop — Del 1: Grunnleggende oppsett

> Git, Prosjektstruktur og ASP.NET Core MVC grunnleggende

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

## Git-kommandoer — Oppslagsverk

### Grunnleggende arbeidsflyt

```bash
# 1. Se status på filer
git status

# 2. Legg til filer i staging area
git add .                    # Alle filer
git add Program.cs           # En spesifikk fil
git add Controllers/         # En mappe

# 3. Lagre endringene
git commit -m "Beskrivende melding"

# 4. Send til GitHub
git push
```

### Nyttige kommandoer

```bash
# Se historikk
git log --oneline           # Kompakt visning
git log --graph             # Grafisk visning av branches

# Se forskjeller
git diff                    # Endringer i working directory
git diff --staged           # Endringer i staging area

# Angre endringer
git restore Program.cs      # Forkast endringer i én fil
git restore .               # Forkast alle endringer
git reset HEAD~1            # Angre siste commit (men behold endringene)

# Branches
git branch                  # Se alle branches
git branch feature-xyz      # Opprett ny branch
git checkout feature-xyz    # Bytt til branch
git merge feature-xyz       # Slå sammen branch til main

# GitHub
git remote -v               # Se remote repositories
git push origin main        # Push til main branch
git pull                    # Hent siste endringer fra GitHub
```

### Commit-meldinger som proffer

Følg formatet: `type: kort beskrivelse`

```bash
git commit -m "feat: Add Pie model with Category navigation"
git commit -m "test: Add unit tests for Pie model validation"
git commit -m "fix: Correct price calculation in PieRepository"
git commit -m "docs: Update README with installation guide"
git commit -m "refactor: Extract validation logic to separate class"
```

**Typer:**
- `feat` — Ny funksjonalitet
- `fix` — Bugfix
- `test` — Legger til eller endrer tester
- `docs` — Dokumentasjon
- `refactor` — Kodeforandring som verken fikser bug eller legger til funksjon
- `style` — Formatering, semicolon, osv (ikke CSS)
- `perf` — Ytelsesforbedring

### Når noe går galt

```bash
# Glemt å legge til filer i siste commit?
git add forgotten-file.cs
git commit --amend --no-edit

# Pushet feil branch til GitHub?
git push --delete origin wrong-branch-name

# Vil starte på nytt fra siste commit?
git reset --hard HEAD

# Vil se hva som skjedde?
git reflog                  # Se all aktivitet
```

---

## 💡 Tips for nybegynnere

### Git workflow som funker

1. **Start alltid med** `git status` — se hvilke filer som er endret
2. **Bruk** `git diff` før `git add` — se hva du faktisk har endret
3. **Små, hyppige commits** — hellre 10 små enn 1 stor
4. **Beskrivende meldinger** — "fix login bug" er bedre enn "fixes"
5. **Push ofte** — ikke vent til slutten av dagen

### Når du har gjort en feil

- **PANIKK IKKE** — git er lagd for å håndtere feil
- **Spør om hjelp** — det er bedre å spørre enn å gjøre det verre
- **Bruk** `git status` og `git log` for å forstå situasjonen
- **Google** — "git how to undo X" finner som regel svar

### .NET-spesifikke tips

```bash
# Bygg prosjektet
dotnet build

# Kjør tester
dotnet test

# Kjør applikasjonen
dotnet run

# Opprett ny controller/model
dotnet new controller -n PieController
```

### VS Code tips

- **Source Control panel** (Ctrl+Shift+G) — visuelt git-grensesnitt
- **GitLens extension** — se git blame og historikk inline
- **Auto save** — Files → Auto Save (slipper å huske Ctrl+S)
- **Terminal** (Ctrl+`) — kjør git-kommandoer rett i editoren

---

**🔗 Neste:** [Del 2: MVC Arkitektur](kjorebok-del2.md) — Controllers, Views og Routing