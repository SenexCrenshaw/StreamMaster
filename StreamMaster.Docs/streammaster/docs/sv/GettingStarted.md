# Komma igång med StreamMaster 🚀

Välkommen till **StreamMaster-dokumentationen**! Den här guiden hjälper dig att konfigurera StreamMaster snabbt och enkelt med hjälp av Docker Compose. Följ instruktionerna nedan för att få din streamingmiljö igång.

## Varför använda Docker Compose?

Att använda Docker Compose förenklar installations- och konfigurationsprocessen genom att definiera din installation i en enda YAML-fil. Detta tillvägagångssätt gör det enkelt att hantera, uppdatera och skala StreamMaster med minimala kommandon.

---

## Förutsättningar 🛠

Innan du börjar, se till att följande är installerade:

1. **Docker** - Installera Docker från [Dockers officiella webbplats](https://docs.docker.com/get-docker/).
2. **Docker Compose** - De flesta Docker-installationer inkluderar Docker Compose, men du kan verifiera med:

   ```bash
   docker-compose --version
   ```

---

## Arkitekturer med stöd 🖥️

StreamMaster stöder flera arkitekturer för att säkerställa kompatibilitet mellan olika system:

- **amd64** (de flesta skrivbords-/servermiljöer)
- **arm64** (lämplig för ARM-baserade enheter)

Den korrekta bilden för din arkitektur kan hämtas automatiskt genom att använda taggen `senexcrenshaw/streammaster:latest`.

---

## Versionstaggar 🔖

StreamMaster tillhandahåller följande taggar för specifika byggen:

- `latest`: Den senaste stabila versionen
- `x.x.x`: Specifika releaseversioner
- `nightly`: Nightly byggen för testning av de senaste funktionerna (kan vara instabila)

---

## Konfigurera StreamMaster med Docker Compose

### Steg 1: Skapa Docker Compose-filen

Skapa en fil `docker-compose.yml` i din önskade katalog och kopiera följande konfiguration till den:

```yaml
services:
  streammaster:
    image: senexcrenshaw/streammaster:latest
    container_name: streammaster
    ports:
      - 7095:7095
      - 7096:7096 # Optional, for SM to host SSL
    environment:
      PUID: 1000
      PGID: 1000
      POSTGRES_USER: postgres # Default
      POSTGRES_PASSWORD: sm123 # Default
      PGDATA: /config/DB # Default
      POSTGRES_HOST: 127.0.0.1 # Default
      POSTGRES_DB: StreamMaster # Default
      BACKUP_VERSIONS_TO_KEEP: 5 # Default
      POSTGRES_USER_FILE: /var/run/secrets/postgres-user # Optional, see Secrets
      POSTGRES_PASSWORD_FILE: /var/run/secrets/postgres-password # Optional, see Secrets
    restart: unless-stopped
    secrets: # Optional, see Secrets
      - postgres-user
      - postgres-password
    volumes:
      - ~/.streammaster:/config
      - ~/.streammaster/tv-logos:/config/tv-logos

  secrets: # Optional, see Secrets
    postgres-user:
      file: postgres-user.txt
    postgres-password:
      file: postgres-password.txt
```

---

### Förklaring av konfigurationen 🔍

- **Image**: Anger vilken StreamMaster-bild som ska användas.
- **Ports**:
  - `7095`: Standard HTTP-port för StreamMasters webbgränssnitt .
  - `7096`: HTTPS-port (SSL) (valfritt).
- **Environment Variables**: Följande miljövariabler har förinställda värden som kan anpassas efter behov.
  - `PUID` och `PGID`: Ange de användar- och grupp-ID som behållaren ska använda för att säkerställa filbehörigheter (standard: `1000`).
  - `POSTGRES_USER`: Definierar PostgreSQL-databasanvändaren (standard: `postgres`).
  - `POSTGRES_PASSWORD`: Lösenord för PostgreSQL-användaren (standard: `sm123`).
  - `PGDATA`: Katalog där PostgreSQL-data lagras (standard: `/config/DB`).
  - `POSTGRES_HOST`: Värdadressen för PostgreSQL-servern (standard: `127.0.0.1`).
  - `POSTGRES_DB`: Namn på PostgreSQL-databasen (standard: `StreamMaster`).
  - `BACKUP_VERSIONS_TO_KEEP`: Antal säkerhetskopieringsversioner som ska sparas (standard: `5`).
  - `POSTGRES_USER_FILE`: Sökväg till en hemlig fil som innehåller PostgreSQL-användaren. Om det anges kommer detta värde att åsidosätta `POSTGRES_USER`.
  - `POSTGRES_PASSWORD_FILE`: Sökväg till en hemlig fil som innehåller PostgreSQL-lösenordet. Om det anges kommer detta värde att åsidosätta `POSTGRES_PASSWORD`.
- **Volumes**:
  - `~/.streammaster:/config`: Lagrar konfigurationsdata.
  - `~/.streammaster/tv-logos:/config/tv-logos`: Lagrar anpassade TV-logotyper.

> **Tips**: Justera filsökvägarna om du föredrar en annan katalog för dina konfigurationsfiler.

---

## Hemligheter 🔐

Du kan förbättra säkerheten genom att använda Docker-hemligheter för att lagra känsliga data som PostgreSQL-referenser. Definiera hemligheter i filerna `postgres-user.txt` och `postgres-password.txt`, använd sedan `POSTGRES_USER_FILE` och `POSTGRES_PASSWORD_FILE` i `docker-compose.yml` för att referera till dessa filer.

Exempel:

```yaml
secrets:
  postgres-u:
    file: ./postgres-user.txt
  postgres-p:
    file: ./postgres-password.txt
```

---

## TV Logotyper Katalog 📺

### Steg 1: Lägg till logotyper

StreamMaster kan använda egna logotyper som finns i en särskild mapp. Placera alla `.jpg`- eller `.png`-filer i katalogen `tv-logos`, strukturera efter behov. StreamMaster kommer att skanna denna katalog vid uppstart.

Till exempel kommer en logotyp som sparats som `countries/albania/abc-al.png` att visas i StreamMaster som `countries-albania-abc-al`.

---

### Steg 2: Starta StreamMaster

För att starta StreamMaster, navigera till katalogen med din `docker-compose.yml`-fil och kör:

```bash
docker-compose up -d
```

- Flaggan `-d` kör tjänsten i fristående läge, vilket innebär att den körs i bakgrunden.

---

{%
   include-markdown "../includes/_footer.md"
%}
