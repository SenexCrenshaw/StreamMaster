# 📘 Kommandoprofiler

Kommandoprofiler i StreamMaster specificerar de streamingkommandon som används för att hämta eller bearbeta strömmar för en **Stream Group**. Dessa profiler bestämmer metoden och parametrarna för att komma åt eller vidarebefordra strömmar, vilket möjliggör olika tillvägagångssätt baserat på nätverksinställningar, enhetskrav eller prestandapreferenser.

---

## Standard kommandoprofiler 🚀

StreamMaster innehåller flera standardkommandoprofiler, var och en optimerad för vanliga streaming-scenarier. Nedan följer en sammanfattning av varje profil:

> **⚠️ OBS:** _Kan inte redigeras._ För att ändra inställningar, **skapa en ny profil** och navigera till `Settings` för att ändra systemets standardinställningar.

| **Profilnamn**  | **Kommando**  | **Beskrivning**                                                                                                    |
| ----------------- | ------------ | ------------------------------------------------------------------------------------------------------------------ |
| **Default**       | STREAMMASTER | Intern binär mekanism som flyttar byte utan bearbetning.                                                 |
| **SMFFMPEG**      | `ffmpeg`     | Använder ffmpeg med alternativ för användaragent, inställningar för återanslutning och optimerad buffring för stabil nätverksstreaming. |
| **SMFFMPEGLocal** | `ffmpeg`     | Optimerad för lokal streaming med ffmpeg, med minskad buffring för att uppnå lägre latens.                   |
| **YT**            | `yt.sh`      | Exekverar skriptet `yt.sh`, främst utformat för streaming från YouTube-länkar.                                  |
| **Redirect**      | Inget         | Omdirigerar direkt till den ursprungliga strömmen utan någon modifiering.                                                |

> **Notera:** Varje standardkommandoprofil är skrivskyddad som standard, vilket ger stabila och tillförlitliga konfigurationer för att säkerställa en konsekvent upplevelse.

---

## Ersättning av parametrar 📝

StreamMaster tillhandahåller två användbara parameterersättningar för att förenkla dynamiska konfigurationer:

- **`{clientUserAgent}`**: Lägger till klientens användaragentsträng, vilket gör att strömmar kan konfigureras för specifika enheter eller webbläsare.
- **`{streamUrl}`**: Ersätts dynamiskt med URL:en för den stream som hämtas.

Med dessa substitutioner kan du anpassa kommandon utan att behöva hårdkoda specifika värden, vilket gör dina kommandoprofiler mer mångsidiga.

---

## Exempel på kommandoprofiler och parametrar 🔧

Nedan följer exempel som visar hur Command och Parameters ställs in inom profiler och hur StreamMaster ersätter värden dynamiskt:

### Exempel 1: SMFFMPEG-profil

- **Kommando**: `ffmpeg`
- **Parametrar**: `-user_agent "{clientUserAgent}" -reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 2 -i "{streamUrl}" -f mpegts -fflags +genpts+discardcorrupt`

**Resulterande kommando med substitutioner:**
Om `{clientUserAgent}` är inställd på `Mozilla/5.0 (Windows NT 10.0; Win64; x64)` och `{streamUrl}` är `http://example.com/live-stream`, kommer StreamMaster att generera:

```bash
ffmpeg -user_agent "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" -reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 2 -i "http://example.com/live-stream" -f mpegts -fflags +genpts+discardcorrupt
```

### Exempel 2: YT-profil

- **Kommando**: `yt.sh`
- **Parametrar**: `"{streamUrl}"`

**Resulterande kommando med substitutioner:**
Om `{streamUrl}` är inställd på en YouTube-länk, till exempel `https://www.youtube.com/watch?v=dQw4w9WgXcQ`, kommer StreamMaster att generera:

```bash
yt.sh "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
```

---

## Kommandoprofil Sökväg 🗂️

StreamMaster söker automatiskt efter exekverbara kommandon i följande kataloger:

- `/config` — Konfigureras inom din Docker Compose.
- `/usr/local/bin`
- `/usr/bin`
- `/bin`

Detta gör att StreamMaster kan hitta och använda vanliga streamingverktyg (t.ex. `ffmpeg` eller anpassade skript som `yt.sh`), förutsatt att de är installerade i standardsystemets sökvägar.

---

## Anpassa kommandoprofiler 🛠️

Så här skapar eller redigerar du en anpassad kommandoprofil:

1. Gå till **Strömmar > ![Översikt över kommandoprofiler](assets/profiles.png) > Kommandoprofiler** i StreamMaster-gränssnittet.
2. Skapa eller redigera en kommandoprofil och konfigurera dess parametrar så att de passar dina streamingkrav.
3. Spara och tillämpa den här profilen på önskade Stream Groups.

> **Notera:** > **Notera:** Systemets standardprofiler kan inte ändras för att säkerställa stabilitet och tillförlitlighet i standardkonfigurationer.

> **Tips:** För avancerade scenarier, se [Discord-kanal för Command Profiles](https://discord.com/channels/1075403862124531753/1296815673472974878) för exempel och fördjupad användning.

Genom att använda Command Profiles på ett effektivt sätt kan du optimera StreamMaster för olika nätverkskonfigurationer och streaminguppsättningar. Utforska de tillgängliga profilerna och anpassa dem efter behov för att uppnå bästa resultat!

---

{% include-markdown "../includes/_footer.md" %}
