# Bethany's Pie Shop — Prosjektkontekst for AI

> Denne filen leses av AI-assistenten ved oppstart av ny sesjon.
> Oppdateres fortløpende etter hvert som prosjektet utvikler seg.

## Hva er dette?

Et læringsprosjekt for ASP.NET Core MVC med C# (.NET 8), bygget steg for steg med TDD.
Brukeren (kvage) er fersk i både .NET og Git og ønsker å lære underveis.

## Brukerens preferanser

- **Språk:** Norsk (bokmål) i all kommunikasjon
- **Læringsstil:** Forklar konsepter underveis, bruk diagrammer og tabeller
- **TDD:** Alltid — Red/Green/Refactor-syklusen
- **Git:** Små, hyppige commits med beskrivende meldinger
- **Modellvalg:** Billige modeller er tilstrekkelig — se lokal `oh-my-opencode.json`
- **Kjørebok:** Alle forklaringer dokumenteres i `docs/kjorebok.md`

## Prosjektstruktur

```
├── BethanysPieShop.slnx            ← Solution (NET 8, låst via global.json)
├── src/BethanysPieShop/            ← ASP.NET Core MVC web-app
├── tests/BethanysPieShop.Tests/    ← xUnit testprosjekt
├── docs/kjorebok.md                ← Læringslogg med alle forklaringer
├── oh-my-opencode.json             ← Lokal modellkonfig (billige modeller)
└── global.json                     ← Låser .NET SDK til 8.0.x
```

## Overordnet plan (features fra Pluralsight-kurset)

| Fase | Feature | Status |
|------|---------|--------|
| 1 | Prosjektoppsett, git, solution | ✅ Ferdig |
| 2 | Domenemodell + Home page + Pie list | ⬜ Ikke startet |
| 3 | Shopping cart | ⬜ Ikke startet |
| 4 | Checkout form | ⬜ Ikke startet |
| 5 | Interactive search page | ⬜ Ikke startet |
| 6 | Authentication | ⬜ Ikke startet |

## GitHub

Repo er IKKE koblet til GitHub ennå. Brukeren installerer `gh` CLI.

## Neste steg

- Vent på at `gh auth login` er gjort
- Opprett GitHub-repo og push
- Start Fase 2: Domenemodell (Pie, Category), repository-mønster, TDD
