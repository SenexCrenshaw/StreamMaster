# M3U-filer

## Vad är en M3U-fil? 📘

En ** M3U-fil ** är en textbaserad spellistfil som används i stor utsträckning för streaming och multimediaändamål. Den innehåller en lista över mediefiler, som vanligtvis pekar på streamingkällor som TV-kanaler eller radiostationer, och är ofta formaterad i `.m3u` eller `.m3u8` tillägg. M3U-filer används ofta för IPTV-tjänster (Internet Protocol Television), vilket gör att användarna kan ladda och hantera kanalströmmar sömlöst.

I **StreamMaster (SM)** gör M3U-filer det möjligt för användare att importera IPTV-kanallistor, vilket ger flexibla alternativ för att hantera och anpassa sina kanalströmmar.

## Importera M3U-filer i StreamMaster 🛠

För att lägga till M3U-filer i StreamMaster, använd alternativet **Import M3U** i StreamMaster UI. StreamMaster tillhandahåller flera anpassningsbara alternativ under importen, vilket gör det möjligt att finjustera hur de importerade strömmarna fungerar och visas. Nedan visas de primära alternativen som är tillgängliga när du importerar en M3U-fil.

### Alternativ för M3U-import

| Alternativ                    | Beskrivning                                                                                      |
| ----------------------------- | -------------------------------------------------------------------------------------------------|
| **Name**                      | Det namn som du vill tilldela den importerade M3U-filen.                                         |
| **Max Stream Count**          | Ställer in en maxgräns för antalet samtidiga strömmar från varje M3U-fil.                        |
| **M3U8 OutPut Profile**       | Anger utmatningsprofilen om M3U8-format används för adaptiva streamingalternativ.                |
| **M3U Key**                   | En nyckelparameter för unik identifiering av en ström                                            |
| **M3U Name**                  | Vilket fält för att ange namnet på den kanal som skapas från en ström                            |
| **Default Stream Group Name** | Anger en standardgrupp när synkronisering är aktiverad.                                          |
| **Url/File Source**           | URL/Lokal fil för den M3U-fil som ska importeras.                                                |
| **Sync Channels**             | Boolesk flagga för att aktivera/deaktivera automatisk synkronisering av kanaler från M3U-källan. |
| **Hours To Update**           | Intervall (i timmar) för kontroll och uppdatering av M3U-filen.                                  |
| **Starting Channel Number**   | Ställer in ett startnummer för kanaler, användbart för anpassade numreringssekvenser.            |
| **Auto Set Channel Numbers**  | Tilldela automatiskt kanalnummer till varje post från M3U.                                       |
| **VOD Tags**                  | En lista med taggar som används för att kategorisera VOD-innehåll (Video on Demand) i M3U-filen. |

### Översikt över importprocessen

När en M3U-fil har lagts till med hjälp av dessa alternativ kommer StreamMaster:

1. **Validerar** - Säkerställer att den angivna URL:en/lokala källan är tillgänglig.
2. **Hämtar och analyserar innehåll** - Hämtar och läser M3U-innehållet för att generera strömposter.
3. **Synkroniserar och organiserar kanaler** - Baserat på de valda alternativen organiseras kanalerna och metadata tillämpas.
4. **Sparar och uppdaterar** - Filen sparas i StreamMasters arkiv och uppdateras regelbundet baserat på det angivna intervallet.

### Felhantering 🔄

Under importen utför StreamMaster flera kontroller för att verifiera M3U-filen:

- Om **URL-källan** är ogiltig eller otillgänglig kommer ett felmeddelande att visas och filen läggs inte till.
- När ** inga strömmar ** upptäcks i M3U, varnar StreamMaster användaren och stoppar vidare bearbetning.
- **Automatisk rensning** utförs vid fel för att säkerställa att inga ofullständiga filer finns kvar i systemet.

### Felsökning av importproblem

Om en import misslyckas ska du kontrollera följande:

- Kontrollera att **URL-källan** är korrekt och tillgänglig.
- Se till att **MaxStreamCount** är inställt inom intervallet för tillgängliga kanaler i M3U.
- Kontrollera att **nödvändiga fält** (t.ex. Namn och UrlSource) är ifyllda.

### Automatisera uppdatering av M3U-filer 🚀

StreamMaster kan automatiskt uppdatera och uppdatera M3U-filer. Ställ in alternativet `HoursToUpdate` för att ange uppdateringsfrekvensen, så att dina strömmar hålls uppdaterade utan manuell åtgärd.

---

{%
   include-markdown "../includes/_footer.md"
%}
