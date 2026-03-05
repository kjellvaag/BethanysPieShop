#!/bin/bash
# cleanup-temp-files.sh
# Fjerner midlertidige filer som kan genereres under utvikling

echo "🧹 Rydder opp midlertidige filer..."

# Tell antall filer før opprydding
before_count=0

# Fjern nul-filer
if find . -name "nul" -type f | grep -q .; then
    nul_count=$(find . -name "nul" -type f | wc -l)
    echo "  📄 Fant $nul_count nul-fil(er), fjerner..."
    find . -name "nul" -type f -delete
    before_count=$((before_count + nul_count))
fi

# Fjern excalidraw.log filer
if find . -name "excalidraw.log" -type f | grep -q .; then
    log_count=$(find . -name "excalidraw.log" -type f | wc -l)
    echo "  📋 Fant $log_count excalidraw.log-fil(er), fjerner..."
    find . -name "excalidraw.log" -type f -delete
    before_count=$((before_count + log_count))
fi

# Fjern andre vanlige midlertidige filer
temp_patterns=(
    "*.tmp"
    "*.temp" 
    "*~"
    ".#*"
    "#*#"
)

for pattern in "${temp_patterns[@]}"; do
    if find . -name "$pattern" -type f | grep -q .; then
        temp_count=$(find . -name "$pattern" -type f | wc -l)
        echo "  🗑️  Fant $temp_count $pattern-fil(er), fjerner..."
        find . -name "$pattern" -type f -delete
        before_count=$((before_count + temp_count))
    fi
done

if [ $before_count -eq 0 ]; then
    echo "✨ Ingen midlertidige filer funnet - prosjektet er rent!"
else
    echo "✅ Fjernet $before_count midlertidig(e) fil(er)"
fi

echo ""
echo "💡 Tips: Kjør 'bash scripts/cleanup-temp-files.sh' jevnlig for å holde prosjektet rent"