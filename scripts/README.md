# 🛠️ Scripts

Denne mappen inneholder nyttige scripts for å vedlikeholde prosjektet.

## cleanup-temp-files.sh

Fjerner midlertidige filer som kan genereres under utvikling:

- `nul` - Tomme filer som kan oppstå på Windows
- `excalidraw.log` - Loggfiler fra Excalidraw-diagrammer
- Andre temp-filer (`*.tmp`, `*.temp`, `*~`, osv.)

### Bruk:

```bash
# Kjør fra rot-mappen i prosjektet
bash scripts/cleanup-temp-files.sh
```

### Automatisk kjøring:

Du kan kjøre dette scriptet jevnlig, for eksempel:
- Før hver git commit
- Som del av din daglige workflow
- Når prosjektet føles "rotete"

Script-et er trygt å kjøre - det fjerner bare filer som allerede er ignorert av git.