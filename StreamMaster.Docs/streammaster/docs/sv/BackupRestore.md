# 🗄️ Guide för säkerhetskopiering och återställning

StreamMaster har en inbyggd funktion för säkerhetskopiering och återställning som hjälper dig att skydda dina data och inställningar. Med automatiserade säkerhetskopior och en enkel återställningsprocess kan du tryggt återställa dina konfigurationer om det behövs.

---

## Säkerhetskopior 📋

För att aktivera automatisk säkerhetskopiering i StreamMaster, justera följande inställningar:

- **Aktivera**: Aktivera säkerhetskopiering
- **Versioner**: Definierar antalet versioner av säkerhetskopior som ska sparas. När denna gräns har uppnåtts raderas äldre säkerhetskopior för att ge plats åt nya.
- **Intervall**: Anger hur ofta (i timmar) säkerhetskopior ska skapas.

StreamMaster kommer nu att automatiskt säkerhetskopiera data varje `Interval` timme och behålla de senaste säkerhetskopiorna `Versions` som anges.

---

## Återställa en säkerhetskopia 🔄

Om du behöver återställa en säkerhetskopia kan du göra det genom att följa dessa steg:

### Steg för att återställa en säkerhetskopia

1.  **Stäng av StreamMaster-containern**:

        - Detta steg är viktigt för att undvika datakonflikter under återställningsprocessen.

1.  **Kopiera säkerhetskopian**:

        - Navigera till katalogen `config/Backups` och leta reda på den backup-fil som du vill återställa.
        - Kopiera den här filen till katalogen `config/Restore`.

1.  **Starta containern**:

        - När backup-filen finns i mappen `config/Restore`, starta StreamMaster-containern.
        - Systemet kommer automatiskt att upptäcka filen och återställa data från säkerhetskopian.

### Viktigt att tänka på

- Efter att ha slutfört återställningen fortsätter StreamMaster med sitt vanliga säkerhetskopieringsschema.
- Se till att katalogen `config/Restore` bara innehåller den fil som du tänker återställa. Alla backup-filer som finns här kommer att användas för återställning.

---

Genom att använda dessa funktioner för säkerhetskopiering och återställning kan du skydda dina data och snabbt återställa efter eventuella problem.

---

{% include-markdown "../includes/_footer.md" %}
