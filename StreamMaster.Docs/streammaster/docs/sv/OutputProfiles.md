# 📤 Utmatningsprofiler

En **Output Profile** i StreamMaster bestämmer hur **M3U/XML-filerna** för en Stream Group genereras och konfigureras. Den här profilen anpassar utdataformatet och styr vilka metadata (t.ex. ikoner, kanalnummer och grupptitlar) som inkluderas, vilket skapar en skräddarsydd spellisteupplevelse för användarna.

---

## Standardinställningar för utmatningsprofil 🛠️

Den standardutgångsprofil som tillhandahålls av StreamMaster innehåller följande konfigurationsinställningar:

> **⚠️ OBS:** _Kan ej redigeras._ För att ändra inställningar, **skapa en ny profil** och navigera till `Inställningar` för att ändra systemets standardinställning.

| **Inställning**           | **Beskrivning**                                                |
| ------------------------- | -------------------------------------------------------------- |
| **Enable Icon**           | Aktiverar `tvg-logo`.                                          |
| **Enable Group Title**    | Aktiverar `group-title`.                                       |
| **Enable Channel Number** | Aktiverar `tvg-chno` / `kanalnummer`                           |
| **Id**                    | Kartlägger vad som används för ID-fältet `CUID` / `channel-id` |
| **Name**                  | Kartlägger vad som används för fältet Namn `tvg-name`          |
| **Group**                 | Kartlägger vad som används för gruppfältet `tvg-group`.        |

> **Notera:** Standardutgångsprofilen är **lässkyddad** och kan inte tas bort eller ändras för att säkerställa konsekvens. Du kan dock skapa anpassade profiler för att uppfylla specifika utdatakrav.

---

## Mappningsalternativ för inställningarna `Id`, `Name` och `Group

Inställningarna `Id`, `Name` och `Group` i en utdataprofil kan mappas till olika metadatafält, vilket ger större flexibilitet i hur kanalinformation visas. Varje inställning kan mappas till något av följande:

- **Inte mappad**: Lämnar fältet omappat, vilket innebär att det inte kommer att inkluderas i utdata.
- **Namn**: Mappas till namnet på kanalen.
- **Grupp**: Mappas till gruppnamnet.
- **Kanalnummer**: Mappas till ett specifikt kanalnummer.
- **Kanalens namn**: Mappas till det ursprungliga kanalnamnet.

Med dessa mappningsalternativ kan du skräddarsy hur varje del av metadata representeras i M3U/XML-utdata, vilket förbättrar tittarens upplevelse.

---

[Förstå utmatningsattribut](M3U_Output_Attributes.md).

---

## Anpassa utmatningsprofiler 🎛️

Så här skapar eller ändrar du en anpassad utdataprofil:

1. Gå till **Inställningar > Utmatningsprofiler** i StreamMaster UI (samma område som **Kommandoprofiler**).
2. Konfigurera metadatafält som `EnableIcon`, `EnableChannelNumber` och andra för att anpassa utdataformatet.
3. Spara din anpassade profil och tillämpa den sedan på önskade Stream Groups.

Denna flexibilitet gör att du kan organisera strömmar med de metadata och visningsalternativ som passar bäst för din installation.

> **Tips:** Anpassade profiler gör det möjligt att skapa en mer organiserad och personlig utskrift genom att styra vilken information som visas i M3U/XML-filen. Experimentera med inställningarna för att skapa en användarvänlig layout!

Genom att använda Output Profiles på ett effektivt sätt kan du skapa en mer engagerande och tillgänglig spellistaupplevelse som är skräddarsydd för din publiks behov.

---

{% include-markdown "../includes/_footer.md" %}
