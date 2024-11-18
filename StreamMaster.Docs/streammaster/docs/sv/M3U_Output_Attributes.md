# 🎬 Dokumentation av M3U-utgångsattribut

Detta dokument innehåller en detaljerad beskrivning av varje attribut som är tillgängligt för anpassning av M3U-utdata i StreamMaster. Genom att förstå dessa attribut kan du styra de metadata som ingår i dina M3U-spellistfiler, vilket förbättrar visningsupplevelsen och organisationen.

---

## 🎬 M3U-utgång: Förståelse av `group-title`

Attributet `group-title` i en M3U-utdatafil används för att kategorisera kanaler i specifika grupper. Det här attributet är särskilt användbart för att organisera ett stort antal kanaler, eftersom det gör det möjligt för användare att filtrera och bläddra efter kategori i mediaspelare och IPTV-applikationer som stöds.

### Vad gör `group-title`?

- **Organiserar kanaler**: Kanaler med samma `group-title` grupperas tillsammans, vilket gör det enklare att hantera spellistor.
- **Förbättrar navigeringen**: Användare kan bläddra efter kategorier (t.ex. "Sport", "Nyheter", "Filmer") i stället för att bläddra igenom en lång lista med kanaler.
- **Förbättrar användarupplevelsen**: Genom att tilldela beskrivande `group-title`-etiketter kan användarna snabbt hitta det innehåll de söker, vilket resulterar i en mer organiserad och effektiv tittarupplevelse.


### Exempel på `group-title` i en M3U-post

```m3u
#EXTINF:-1 tvg-id="123" tvg-name="Channel Name" tvg-logo="http://example.com/logo.png" group-title="Sports", Channel Name
http://stream-url.com/stream
```

---

## 🖼️ M3U-utgång: Förståelse för `tvg-logo`

Attributet `tvg-logo` i en M3U-utdatafil visar en logotyp för varje kanal, vilket ger ett visuellt element som gör det lättare att identifiera kanaler.

### Vad gör `tvg-logo`?

- **Förbättrar igenkänningen av kanaler**: Genom att inkludera en kanallogotyp kan användare snabbt känna igen kanaler genom deras varumärke.
- **Förbättrar det visuella tilltalet**: Kanallogotyper gör spellistan visuellt tilltalande och organiserad.

### Exempel på `tvg-logo` i en M3U-post

```m3u
#EXTINF:-1 tvg-id="123" tvg-name="Channel Name" tvg-logo="http://example.com/logo.png", Channel Name
http://stream-url.com/stream
```

---

## 🆔 EPG/M3U-utgång: Förståelse för `CUID` / `channel-id`

Attributet `CUID` eller `channel-id` fungerar som en unik identifierare för varje kanal i M3U-filen. Denna identifierare hjälper till att särskilja kanaler, särskilt i system där flera kanaler kan ha liknande namn.

### Vad gör `CUID` / `channel-id`?

- Tillhandahåller unik kanalidentifiering**: Tilldelar en unik identifierare till varje kanal, vilket säkerställer konsekvent igenkänning mellan enheter och applikationer.
- **Förbättrar integrationen med EPG:er**: Detta anger också "channel-id" i EPG-utdata, vilket länkar M3U-kanalposter med EPG-data (Electronic Program Guide) för korrekt schemaläggning.

### Exempel på `CUID` i en M3U-post

```m3u
#EXTINF:-1 CUID="12345" channel-id="ChannelID" tvg-name="Channel Name" tvg-logo="http://example.com/logo.png", Channel Name
http://stream-url.com/stream
```

### Motsvarande EPG XML-post

Nedan visas ett exempel på hur en EPG XML-post kan se ut med `channel-id` inställt för att matcha M3U-kanalposten:

```xml
<tv>
  <channel id="ChannelID">
    <display-name>Channel Name</display-name>
    <icon src="http://example.com/logo.png" />
  </channel>
  <programme start="20240101060000 +0000" stop="20240101070000 +0000" channel="ChannelID">
    <title>Morning News</title>
    <desc>The latest news updates and reports.</desc>
  </programme>
</tv>
```

---

## 📺 M3U-utdata: Förståelse av `tvg-name`

Attributet `tvg-name` låter dig ställa in visningsnamnet för varje kanal, vilket vanligtvis matchar det ursprungliga namnet på strömmen eller TV-stationen.

### What Does `tvg-name` Do?

- **Visar kanalens namn**: Ger ett tydligt, identifierbart namn för varje kanal, vilket förbättrar användarupplevelsen.
- **Matchar det ursprungliga strömnamnet**: Återger vanligtvis namnet som tillhandahålls av källan, vilket säkerställer konsekvens.

### Exempel på `tvg-name` i en M3U-post
*** Translated with www.DeepL.com/Translator (free version) ***

```m3u
#EXTINF:-1 tvg-id="123" tvg-name="Channel Name" tvg-logo="http://example.com/logo.png", Channel Name
http://stream-url.com/stream
```

---

## 📂 M3U-utdata: Förståelse av `tvg-group`

Attributet `tvg-group` definierar den grupp eller kategori som en kanal tillhör. Denna gruppering kan användas tillsammans med `group-title` för att förbättra organisationen inom spellistor.

### Vad gör `tvg-group`?

- **Kategoriserar kanaler**: Kanaler med samma `tvg-group` grupperas tillsammans, vilket förbättrar organisationen av spellistor.
- ** Möjliggör enklare filtrering**: Användare kan filtrera efter kategori, på samma sätt som med `group-title`.

### Exempel på `tvg-group` i en M3U-post

```m3u
#EXTINF:-1 tvg-id="123" tvg-name="Channel Name" tvg-group="News", Channel Name
http://stream-url.com/stream
```

---

{% include-markdown "../includes/_footer.md" %}
