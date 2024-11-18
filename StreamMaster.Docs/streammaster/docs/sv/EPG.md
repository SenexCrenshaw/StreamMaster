# EPG/XML-filer

## Vad är en EPG/XML-fil?? 📘

En **EPG-fil (Electronic Program Guide)**, som ofta är formaterad i XML (särskilt XMLTV-format), används för att tillhandahålla programguideinformation för streamingkanaler. EPG-filer innehåller detaljer om schemalagda program, t.ex. titlar, start- och sluttider, beskrivningar och genrer. Dessa guideuppgifter förbättrar tittarupplevelsen genom att användarna kan bläddra bland aktuella och kommande program i ett strukturerat format.

I **StreamMaster (SM)** kan EPG/XML-filer importeras för att integrera programguider med dina IPTV-kanaler och berika användarupplevelsen med programscheman och metadata.

## Importera EPG/XML-filer i StreamMaster 🛠

För att lägga till EPG/XML-filer i StreamMaster, använd alternativet **Importera EPG** i StreamMasters användargränssnitt. StreamMaster tillhandahåller flera alternativ under importen för att anpassa hur EPG-data bearbetas och visas. Nedan visas de primära alternativen som är tillgängliga när du importerar en EPG-fil.

### Alternativ för EPG-import

| Alternativ              | Beskrivning                                                                                 |
| ------------------- | ------------------------------------------------------------------------------------------- |
| **Namn**            | The name you want to assign to the imported EPG file.                                       |
| **Filnamn**       | The local name to save the file under once imported.                                        |
| **EPG-nummer**      | A unique identifier for the EPG file, allowing differentiation among multiple EPG sources.  |
| **Tidsförskjutning**      | (Optional) Adjusts the time of all programs by the specified number of hours.               |
| **Uppdateringsintervall** | Intervall (i timmar) för automatisk kontroll och uppdatering av EPG-filen.                        |
| **Url/Fil-källa** | URL eller lokal filsökväg till den EPG-fil som ska importeras.                                  |
| **Färg**           | (Valfritt) Tilldelar en färg till guideposterna för enklare identifiering i gränssnittet. |

### Översikt över importprocessen

När en EPG/XML-fil har lagts till med hjälp av dessa alternativ, kommer StreamMaster:

1. **Validera** - Säkerställer att den angivna URL:en eller lokala filkällan är tillgänglig.
2. **Hämtar och analyserar innehåll** - Laddar ner och läser XML-innehållet för att fylla i guideinformationen.
3. **Bearbetar EPG-data** - Justerar tidszoner, använder färger och länkar program till kanaler enligt användarinställningarna.
4. **Spara och uppdatera** - EPG-filen sparas i StreamMasters arkiv och uppdaterar programdata regelbundet enligt specifikation.

### Felhantering 🔄

Under importen utför StreamMaster flera kontroller för att verifiera EPG-filen:

- Om **URL-källan** är ogiltig eller otillgänglig visas ett felmeddelande och filen läggs inte till.
- Om **filformatet** inte stöds eller är oläsligt, stoppar StreamMaster vidare bearbetning och varnar användaren.
- **Automatisk rensning** sker vid fel för att säkerställa att inga ofullständiga filer lämnas kvar i systemet.

### Felsökning av importproblem

Om en import misslyckas, kontrollera följande:

- Kontrollera att **URL eller filsökväg** är korrekt och tillgänglig.
- Bekräfta att **EPG-numret** är unikt och inte används av andra EPG-filer.
- Se till att **obligatoriska fält** (t.ex. Namn och UrlSource) är ifyllda.

### Automatisera uppdatering av EPG-filer 🚀

StreamMaster kan automatiskt uppdatera och uppdatera EPG-filer. Ställ in alternativet `Uppdateringsintervall` för att ange uppdateringsfrekvensen, så att din guideinformation hålls uppdaterad utan manuell inblandning.

---

{%
    include-markdown "../includes/_footer.md"
%}
