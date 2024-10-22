# 📘 StreamMaster -dokumentation

Detta dokument beskriver stegen för att ställa in och hantera dokumentation för [StreamMaster] (https://github.com/senexcrenhaw/streammaster) -projekt med MKDOCS.Uppsättningen inkluderar stöd för internationalisering (I18N) och användningen av MKDOCS -materialtema för ett modernt utseende.

## Varför bidra till dokumentationen?

Dokumentationen är det första som användare och utvecklare hänvisar till när de använder eller bidrar till StreamMaster.Genom att hjälpa till att förbättra och underhålla dokumentationen säkerställer du att StreamMaster förblir tillgänglig och enkel att använda för samhället.

Dina bidrag till dokumentationen:
- Hjälp andra att lära sig och använda StreamMaster mer effektivt.
- Förbättra tydligheten för icke-infödda engelsktalande genom bättre i18n-stöd.
- Gör det möjligt för utvecklare att lättare bidra till projektet.

Även små uppdateringar som att korrigera typfel eller klargöra instruktioner gör en stor skillnad!

## 🛠 Förutsättningar

För att generera och betjäna dokumentation lokalt behöver du Python installerad.Se till att "Pip", Pythons paketchef, också är tillgänglig.

### Installera mkdocs och plugins

För att installera mkdocs och de nödvändiga plugins för I18N och tema, kör följande kommando:

`` `bash
python -M pip install mkdocs mkdocs-i18n mkdocs-material mkdocs-static-i18n
`` `

Detta installerar följande:

- `mkdocs`: den statiska platsgeneratorn.
- `mkdocs-i18n`: för hantering av internationalisering.
- `MkDocs-Material ': Ett populärt tema med en modern design.
-`MKDOCS-STATIC-I18N`: Lägger till statisk internationaliseringsstöd.

## Lokal utveckling

För att bygga och betjäna dokumentationen lokalt under utvecklingen, följ dessa steg.

### Bygga dokumenten

För att generera den statiska dokumentationen:

`` `bash
mkdocs build
`` `

### Tjänar dokumenten lokalt

För att köra en utvecklingsserver som tittar på ändringar och laddas automatiskt om:

`` `bash
mkdocs tjänar
`` `

Detta kommer att vara värd för dokumentationen lokalt på `http: // localhost: 8000".

## Produktionsbyggnad

När du är redo att distribuera dokumentationen till produktionen, se till att du rengör den tidigare byggnaden och ställer in webbplatsen URL korrekt.Kör följande kommando:

`` `bash
mkdocs build-clean-site-url https://senexcrenhaw.github.io/streammaster/
`` `

Detta bygger en ren version av dokumentationen och ställer in rätt URL för produktionsplatsen.

## 📝 Bidrag till dokumentationen

Dokumentationsfiler lever under mappen "StreamMaster.docs \ StreamMaster" i [StreamMaster Repository] (https://github.com/senexcrenhaw/streammaster).

För att bidra till dokumentationen:

- ** Skapa en ny gren ** för dina ändringar.
- ** Se till att engelska (`en ') alltid uppdateras **.Engelska fungerar som det primära språket, och allt innehåll måste uppdateras på engelska.
- ** Ge bästa möjliga översättningar ** för andra språk som stöds, till exempel spanska (`es`), franska (` fr`), tyska (`de`) och alla andra stöd som stöds.Även om dessa översättningar inte behöver vara perfekta, bör de sträva efter att exakt återspegla det engelska innehållet.
- Engelska filer lever under "docs/en".
- Översättningar lever under sina respektive kataloger (t.ex. "docs/es" för spanska, "docs/fr" för franska, etc.).
- ** Test ** Alla förändringar genom att köra "MkDocs tjänar" lokalt för både den engelska versionen och eventuella uppdaterade översättningar.
- ** Skicka en Pull Request (PR) ** för granskning.

### Komma igång i 3 enkla steg!

1. Gaffla förvaret och klona det till din lokala maskin.
2. Skapa en ny gren för dina ändringar.
3. Se till att engelska ("en") uppdateras och ger bästa möjliga översättningar för andra stöd som stöds och skickar sedan in en PR.

Så är det!🎉 Du har bidragit till StreamMaster!

## Tips för att skriva bra dokumentation

- ** Var tydlig och kortfattad: ** Fokusera på huvudpunkterna och undvik alltför tekniskt språk där det är möjligt.
- ** Använd exempel: ** Kodavdrag eller visuella hjälpmedel hjälper till att klargöra komplexa koncept.
- ** Var konsekvent: ** Håll ton och terminologi konsekvent över alla avsnitt.
- ** Testa allt: ** Se till att alla kodexempel, kommandon och instruktioner fungerar som förväntat.

## Bidragsigenkänning 🌟

Vi uppskattar alla bidrag, oavsett hur litet!Alla bidragsgivare kommer att läggas till i StreamMaster Documentation Hall of Fame, nedan:

[Visa alla bidragsgivare] (bidragsgivare.md)

## Vi behöver din hjälp!🤝

Streammaster växer ständigt, och vi behöver samhällets hjälp för att hålla dokumentationen förstklassig.Inget bidrag är för litet, oavsett om det är att fixa typfel, lägga till exempel eller översätta innehåll.

Hoppa in, och låt oss göra Streammaster bättre tillsammans!✨

## Behöver du hjälp eller har frågor?Gå med oss ​​på Discord!🎮

För alla frågor, support eller diskussioner kan du gå med i den officiella ** StreamMaster Discord -servern **.

👉 [Gå med i StreamMaster Discord] (https://discord.gg/gfz7ethhg2) 👈

Vi är här för att hjälpa, och du hittar en aktiv gemenskap av utvecklare och användare.Ställ gärna frågor, rapportera frågor eller diskutera nya idéer för att förbättra Streammaster!