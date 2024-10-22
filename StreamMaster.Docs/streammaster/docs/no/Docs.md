# 📘 Streammaster -dokumentasjon

Dette dokumentet skisserer trinnene for å sette opp og administrere dokumentasjon for [Streammaster] (https://github.com/senexcrenshaw/streammaster) -prosjektet ved hjelp av mkdocs.Oppsettet inkluderer støtte for internasjonalisering (I18N) og bruken av MKDOCS -materialetema for et moderne utseende og følelse.

## Hvorfor bidra til dokumentasjonen?

Dokumentasjonen er det første brukere og utviklere refererer til når de bruker eller bidrar til Streammaster.Ved å bidra til å forbedre og vedlikeholde dokumentasjonen, sikrer du at Streammaster forblir tilgjengelig og enkel å bruke for samfunnet.

Dine bidrag til dokumentasjonen:
- Hjelp andre å lære og bruke Streammaster mer effektivt.
- Forbedre klarheten for ikke-innfødte engelsktalende gjennom bedre I18N-støtte.
- Gjør det mulig for utviklere å lettere bidra til prosjektet.

Selv små oppdateringer som å korrigere skrivefeil eller avklare instruksjoner utgjør en stor forskjell!

## 🛠 Forutsetninger

For å generere og servere dokumentasjon lokalt, trenger du Python installert.Forsikre deg om at `Pip`, Pythons pakkesjef, også er tilgjengelig.

### Installere mkdocs og plugins

Hvis du vil installere MKDOC -er og de nødvendige plugins for i18n og tema, kjører du følgende kommando:

`` Bash
Python -M Pip Install Mkdocs Mkdocs-I18n Mkdocs-materiale Mkdocs-Static-I18n
`` `

Dette installerer følgende:

- `mkdocs`: den statiske nettstedgeneratoren.
- `MKDOCS-I18N`: For håndtering av internasjonalisering.
- `mkdocs-material`: et populært tema med et moderne design.
-`MKDOCS-STATIC-I18N`: Legger til statisk internasjonaliseringsstøtte.

## Lokal utvikling

Følg disse trinnene for å bygge og tjene dokumentasjonen lokalt under utvikling.

### Å bygge dokumentene

For å generere den statiske dokumentasjonen:

`` Bash
MKDOCS Build
`` `

### serverer dokumentene lokalt

For å kjøre en utviklingsserver som ser etter endringer og automatisk laster inn på nytt:

`` Bash
Mkdocs serverer
`` `

Dette vil være vertskap for dokumentasjonen lokalt på `http: // localhost: 8000`.

## Produksjonsbygging

Når du er klar til å distribuere dokumentasjonen til produksjonen, må du forsikre deg om at du rengjør forrige bygg og angir nettadressen riktig.Kjør følgende kommando:

`` Bash
Mkdocs build--clean--site-url https://senexcrenshaw.github.io/streammaster/
`` `

Dette bygger en ren versjon av dokumentasjonen og angir riktig URL for produksjonsstedet.

## 📝 bidrar til dokumentasjonen

Dokumentasjonsfiler lever under mappen `StreamMaster.Docs \ Streammaster` i [StreamMaster Repository] (https://github.com/senxcrenshaw/streammaster).

For å bidra til dokumentasjonen:

- ** Lag en ny gren ** for endringene dine.
- ** Forsikre deg om at engelsk (`en`) alltid oppdateres **.Engelsk fungerer som primærspråket, og alt innhold må oppdateres på engelsk.
- ** Gi best mulig oversettelser ** for andre støttede språk, for eksempel spansk (`es`), fransk (` fr`), tysk (`de`) og alle andre støttede språk.Selv om disse oversettelsene ikke trenger å være perfekte, bør de ta sikte på å gjenspeile det engelske innholdet nøyaktig.
- Engelske filer lever under `dokumenter/en`.
- Oversettelser lever under sine respektive kataloger (f.eks. `Docs/es` for spansk,` Docs/fr` for fransk, etc.).
- ** Test ** Alle endringer ved å kjøre `mkdocs serverer` lokalt for både den engelske versjonen og eventuelle oppdaterte oversettelser.
- ** Send inn en trekkforespørsel (PR) ** for gjennomgang.

### Komme i gang i 3 enkle trinn!

1. Gaffel depotet og klon den til din lokale maskin.
2. Lag en ny gren for endringene dine.
3. Forsikre deg om at engelsk (`en`) blir oppdatert og gi best mulig oversettelser for andre støttede språk, og deretter sende inn en PR.

Det er det!🎉 Du har bidratt til Streammaster!

## Tips for å skrive god dokumentasjon

- ** Vær tydelig og kortfattet: ** Fokuser på hovedpunktene, og unngå altfor teknisk språk der det er mulig.
- ** Bruk eksempler: ** Kodeutdrag eller visuelle hjelpemidler hjelper til med å avklare komplekse konsepter.
- ** Vær konsekvent: ** Hold tone og terminologi konsistent i alle seksjoner.
- ** Test alt: ** Forsikre deg om at alle kodeeksempler, kommandoer og instruksjoner fungerer som forventet.

## bidragsyter anerkjennelse 🌟

Vi setter pris på hvert bidrag, uansett hvor lite!Alle bidragsytere vil bli lagt til Streammaster Documentation Hall of Fame, omtalt nedenfor:

[Se alle bidragsytere] (bidragsytere.md)

## Vi trenger din hjelp!🤝

Streammaster vokser stadig, og vi trenger samfunnets hjelp til å holde dokumentasjonen førsteklasses.Ingen bidrag er for lite, enten det er å fikse skrivefeil, legge til eksempler eller oversette innhold.

Hopp inn, og la oss gjøre Streammaster bedre sammen!✨

## Trenger du hjelp eller har spørsmål?Bli med på Discord!🎮

For spørsmål, støtte eller diskusjoner kan du bli med på den offisielle ** Streammaster Discord Server **.

👉 [Bli med StreamMaster Discord] (https://discord.gg/gfz7ethhg2) 👈

Vi er her for å hjelpe, og du finner et aktivt fellesskap av utviklere og brukere.Still gjerne spørsmål, rapporter om problemer eller diskuter nye ideer for å forbedre Streammaster!