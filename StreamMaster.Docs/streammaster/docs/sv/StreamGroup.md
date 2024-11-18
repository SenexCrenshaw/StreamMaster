# Strömmande grupper

## Vad är en strömgrupp? 📘

En **Stream Group** i StreamMaster är ett sätt att organisera och hantera flera streamingkanaler under en enda gruppering. Stream-grupper gör det möjligt för användare att tillämpa specifika profiler, kommandon och åtkomstnycklar, vilket skapar skräddarsydda konfigurationer som enkelt kan hanteras och tillämpas på olika strömmar. Den här funktionen är särskilt användbar för att organisera kanaler efter kategori eller tillämpa specifika uppspelningsinställningar på olika grupper av kanaler.

## Skapa en Stream Group 🛠

För att lägga till en ny Stream Group i StreamMaster, använd alternativet **Create Stream Group** i StreamMaster UI. Här kan du definiera gruppens namn och eventuellt ange en utdataprofil, kommandoprofil och en unik gruppnyckel. Nedan visas de primära alternativen som är tillgängliga när du skapar en Stream Group.

### Alternativ för Stream Group

| Alternativ               | Beskrivning                                                                                                       |
| ------------------------ | ----------------------------------------------------------------------------------------------------------------- |
| **Name**                 | Namnet på Stream Group. Detta är obligatoriskt och måste vara unikt.                                              |
| **Output Profile Name**  | (Valfritt) Anger den utdataprofil som ska gälla för alla strömmar i den här gruppen.                              |
| **Command Profile Name** | (Valfritt) Tilldelar en kommandoprofil som avgör specifika kommandon för uppspelning eller streaming för gruppen. |
| **Group Key**            | (Valfritt) En unik identifierare för gruppen. Om den inte anges kommer StreamMaster att generera en automatiskt.  |

### Skapa och hantera strömgrupper

När en Stream Group har skapats med hjälp av dessa alternativ, kommer StreamMaster:

1. **Validerar** - Säkerställer att namnet är unikt och inte reserverat. (t.ex. ”all” är reserverat och kan inte användas).
2. **Genererar ett unikt ID** - Använder en intern generator för att tilldela ett unikt enhets-ID för gruppen.
3. **Tillämpar profiler** - Lägger till standardprofiler eller specificerade profiler för utdata och kommandon, vilket gör det möjligt att anpassa streamingbeteendet.
4. **Sparar och uppdaterar** - Streamgruppen sparas i StreamMasters arkiv och blir tillgänglig för omedelbar användning.

# 🔐 Stream Group-kryptering och säkerhet i StreamMaster

StreamMaster erbjuder robusta säkerhetsalternativ för att kontrollera åtkomst och säkra länkar inom **Stream Groups (SGs)**. Stream Groups möjliggör kryptering genom gruppnycklar, vilket ger ett enkelt men säkert sätt att hantera länkåtkomst. Detta avsnitt beskriver de säkerhetsfunktioner som är tillgängliga för Stream Groups och förklarar hur de fungerar tillsammans med StreamMasters bredare autentiseringsfunktioner.

## Säkerhetsfunktioner för strömgrupper 🔒

Stream Groups i StreamMaster kan konfigureras med unika **Group Keys**. Dessa nycklar möjliggör:

- **Länk-kryptering**: Krypterar genererade HDHR-, XML- och M3U-länkar, vilket säkrar åtkomst till kanaler inom Stream Group.
- **Custom Access Control**: Varje gruppnyckel begränsar åtkomsten till endast de som har motsvarande nyckel, vilket ger ett extra säkerhetslager för externa anslutningar.

### 1. Kryptering av länkar med gruppnycklar

När en gruppnyckel konfigureras för en Stream Group krypteras alla länkar som genereras (t.ex. HDHR, XML och M3U) för den gruppen. Detta säkerställer att endast användare med lämplig dekrypteringsnyckel eller auktoriserad åtkomst kan visa strömlänkarna, vilket förbättrar säkerheten för extern åtkomst.

#### Konfigurera länkkryptering

1. **Navigera** till StreamMasters webbgränssnitt.
2. Gå till **Streams** > **Stream Group (SG)** och välj önskad Stream Group.
3. **Tilldela en unik gruppnyckel** för att kryptera länkar som genereras för gruppen.

> **Notera**: StreamMaster tillhandahåller också **korta, okrypterade länkar** för enklare lokal åtkomst, perfekt för säkra, privata nätverksmiljöer.

### 2. Hantering av gruppnycklar för säkerhet

StreamMaster låter dig uppdatera eller ändra gruppnycklar när som helst. Om du ändrar en gruppnyckel återskapas automatiskt alla krypterade länkar som är associerade med den aktuella Stream-gruppen, vilket ger ett nytt lager av säkerhet.

#### Steg för att uppdatera gruppnycklar

1. I StreamMaster-gränssnittet navigerar du till **Streams** > **Stream Group**.
2. Välj den Stream Group som du vill uppdatera.
3. Ange en ny gruppnyckel och spara ändringarna.
4. **Uppdatera alla delade länkar** så att de återspeglar den nya gruppnyckelkrypteringen.

> Mer information om hur Group Keys och autentisering fungerar finns i vår [Autentiseringsguide](Auth.md).

---

## Övergripande autentisering och länksäkerhet

StreamMaster stöder två huvudtyper av autentisering:

- **UI Autentisering** (formulär/ingen): Kontrollerar åtkomst till webbgränssnittet.
- **Autentisering av krypterad länk**: Säkrar streaminglänkar med hjälp av gruppnycklar.

För mer information om dessa alternativ, se

### Exempel på användningsfall för Stream Groups

- **Kategorisering av kanaler efter genre**: Gruppering av sportkanaler, nyhetskanaler eller filmkanaler för enklare åtkomst och hantering.
- **Använda anpassade profiler**: Använda specifika utmatningsprofiler för olika kanalgrupper, t.ex. högupplösta profiler för sport eller profiler med låg bandbredd för mobiltittande.
- **Säker åtkomstkontroll**: Inställning av unika gruppnycklar för att kontrollera åtkomst till vissa kanaler inom en grupp, vilket ger flexibilitet vid hantering av behörigheter.

### Felsökning av problem med skapande av strögrupper 🔄

Om ett fel uppstår under skapandet av Stream Group:

- Se till att **Name** är ifyllt och inte använder reserverade ord som "all".
- Kontrollera att alla angivna **profiler** (Output Profile eller Command Profile) finns och är korrekt namngivna.
- Om ingen gruppnyckel anges kommer StreamMaster att generera en automatiskt.

---

{%
    include-markdown "../includes/_footer.md"
%}
