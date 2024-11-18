# 📘 StreamMaster Dokumentation

Det här dokumentet beskriver stegen för att konfigurera och hantera dokumentation för projektet [StreamMaster](https://github.com/SenexCrenshaw/StreamMaster) med hjälp av MkDocs. Installationen omfattar stöd för internationalisering (i18n) och användning av MkDocs materialtema för ett modernt utseende och känsla.

## Varför bidra till dokumentationen?

Dokumentationen är det första som användare och utvecklare hänvisar till när de använder eller bidrar till StreamMaster. Genom att hjälpa till att förbättra och underhålla dokumentationen ser du till att StreamMaster förblir tillgängligt och lätt att använda för samhället.

Dina bidrag till dokumentationen:
- Hjälpa andra att lära sig och använda StreamMaster mer effektivt.
- Förbättra tydligheten för personer som inte har engelska som modersmål genom bättre i18n-stöd.
- Göra det lättare för utvecklare att bidra till projektet.

Även små uppdateringar som att korrigera stavfel eller förtydliga instruktioner gör stor skillnad!

## 🛠 Förkunskapskrav

För att generera och tillhandahålla dokumentation lokalt behöver du Python installerat. Se till att `pip`, Pythons pakethanterare, också är tillgänglig.

### Installera MkDocs och insticksprogram

För att installera MkDocs och de nödvändiga insticksprogrammen för i18n och tematisering kör du följande kommando:

```bash
python -m pip install mkdocs mkdocs-i18n mkdocs-material mkdocs-static-i18n
```

Detta installerar följande:

- `mkdocs`: Generatorn för statiska webbplatser.
- `mkdocs-i18n`: För hantering av internationalisering.
- `mkdocs-material`: Ett populärt tema med modern design.
- `mkdocs-static-i18n`: Lägger till stöd för statisk internationalisering.

## Lokal utveckling

Följ dessa steg för att bygga och tillhandahålla dokumentationen lokalt under utvecklingen.

### Bygga upp dokumenten

Så här genererar du den statiska dokumentationen:

```bash
mkdocs build
```

### Lokal service för dokumentation

För att köra en utvecklingsserver som bevakar ändringar och automatiskt laddar om:

```bash
mkdocs serve
```

Detta kommer att hosta dokumentationen lokalt på `http://localhost:8000`.

## Produktions Build

När du är redo att distribuera dokumentationen till produktion, se till att du rensar den tidigare build och ställer in webbplatsens URL korrekt. Kör följande kommando:

```bash
mkdocs build --clean --site-url https://senexcrenshaw.github.io/StreamMaster/
```

Detta skapar en ren version av dokumentationen och anger rätt URL för produktionssajten.

## 📝 Att bidra till dokumentationen

Dokumentationsfiler finns i mappen `StreamMaster.Docs\streammaster` i [StreamMaster repository](https://github.com/SenexCrenshaw/StreamMaster).

Att bidra till dokumentationen:

- **Skapa en ny gren** för dina ändringar.
- **Se till att engelska (`en`) alltid är uppdaterad**. Engelska fungerar som huvudspråk och allt innehåll måste uppdateras på engelska.
- **Gör bästa möjliga översättningar** för andra språk som stöds, t.ex. spanska (`es`), franska (`fr`), tyska (`de`) och alla andra språk som stöds. Även om dessa översättningar inte behöver vara perfekta bör de sträva efter att korrekt återspegla det engelska innehållet.
  - Engelska filer finns under `docs/en`.
  - Översättningar finns i respektive katalog (t.ex. `docs/es` för spanska, `docs/fr` för franska etc.).
- **Testa** alla ändringar genom att köra `mkdocs serve` lokalt för både den engelska versionen och alla uppdaterade översättningar.
- **Lämna in en Pull Request (PR)** för granskning.

### Kom igång med 3 enkla steg!

1. Fork:a och klona till din lokala maskin.
2. Skapa en ny gren för dina ändringar.
3. Se till att engelska (`en`) uppdateras och tillhandahåll bästa möjliga översättningar för andra språk som stöds, skicka sedan in en PR.

Så där ja! 🎉 Du har bidragit till StreamMaster!

## Tips för att skriva bra dokumentation

- **Var tydlig och koncis:** Fokusera på huvudpunkterna och undvik alltför tekniskt språk där det är möjligt.
- **Användningsexempel:** Kodsnuttar eller visuella hjälpmedel hjälper till att klargöra komplexa begrepp.
- **Var konsekvent:** Håll tonen och terminologin konsekvent i alla avsnitt.
- **Testa allt:** Säkerställ att alla kodexempel, kommandon och instruktioner fungerar som förväntat.

## Erkännande av bidragsgivare 🌟

Vi uppskattar varje bidrag, oavsett hur litet det är! Alla bidragsgivare kommer att läggas till i StreamMaster Documentation Hall of Fame, som visas nedan:

[Se alla bidragsgivare](Contributors.md)

## Vi behöver din hjälp! 🤝

StreamMaster växer ständigt och vi behöver hjälp av våra kunder för att hålla dokumentationen på topp. Inget bidrag är för litet, oavsett om det handlar om att fixa stavfel, lägga till exempel eller översätta innehåll.

Hoppa in, och låt oss göra StreamMaster bättre tillsammans! ✨

## Behöver du hjälp eller har du frågor? Gå med oss på Discord! 🎮

För frågor, support eller diskussioner kan du gå med i den officiella **StreamMaster Discord-servern**.

👉 [Gå med i StreamMaster Discord](https://discord.gg/gFz7EtHhG2) 👈

Vi finns här för att hjälpa till, och du kommer att hitta en aktiv gemenskap av utvecklare och användare. Känn dig fri att ställa frågor, rapportera problem eller diskutera nya idéer för att förbättra StreamMaster!