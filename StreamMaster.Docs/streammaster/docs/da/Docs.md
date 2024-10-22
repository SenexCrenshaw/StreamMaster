# 📘 Streammaster -dokumentation

Dette dokument skitserer trinnene til at konfigurere og administrere dokumentation til [streammaster] (https://github.com/senexcrenshaw/streammaster) -projektet ved hjælp af MKDOCS.Opsætningen inkluderer support til internationalisering (I18N) og brugen af ​​MKDOCS -materialetema til et moderne look og fornemmelse.

## Hvorfor bidrage til dokumentationen?

Dokumentationen er den første ting, som brugere og udviklere henviser til, når de bruger eller bidrager til Streammaster.Ved at hjælpe med at forbedre og vedligeholde dokumentationen sikrer du, at Streammaster forbliver tilgængelig og let at bruge til samfundet.

Dine bidrag til dokumentationen:
- Hjælp andre med at lære og bruge streammaster mere effektivt.
- Forbedre klarhed for ikke-indfødte engelsktalende gennem bedre I18N-support.
- Aktiver udviklere til lettere at bidrage til projektet.

Selv små opdateringer som at korrigere skrivefejl eller afklarende instruktioner gør en stor forskel!

## 🛠 Forudsætninger

For at generere og betjene dokumentation lokalt har du brug for Python installeret.Sørg for, at Pythons pakkehåndtering ", Pythons pakkechef, også er tilgængelig.

### Installation af MKDOC'er og plugins

For at installere MKDOC'er og de krævede plugins til I18N og tema, kør følgende kommando:

`` `bash
Python -m pip install MKDOCS MKDOCS-I18N MKDOCS-Material MKDOCS-Static-I18N
`` `

Dette installerer følgende:

- `mkdocs`: Den statiske webstedsgenerator.
- `mkdocs-i18n`: til håndtering af internationalisering.
- `mkdocs-materiale ': et populært tema med et moderne design.
-`MKDOCS-Static-I18N`: tilføjer statisk internationaliseringsstøtte.

## Lokal udvikling

For at opbygge og betjene dokumentationen lokalt under udvikling, skal du følge disse trin.

### bygger dokumenterne

For at generere den statiske dokumentation:

`` `bash
MKDOCS Build
`` `

### Serverer dokumenterne lokalt

At køre en udviklingsserver, der holder øje med ændringer og automatisk genindlæser:

`` `bash
MKDOC'er tjener
`` `

Dette vil være vært for dokumentationen lokalt på `http: // localhost: 8000`.

## Produktionsopbygning

Når du er klar til at implementere dokumentationen til produktionen, skal du sikre dig, at du renser den forrige build og indstiller Webstedets URL korrekt.Kør følgende kommando:

`` `bash
MKDOCS Build--Rene-Site-url https://senexcrenshaw.github.io/streammaster/
`` `

Dette bygger en ren version af dokumentationen og indstiller den korrekte URL til produktionsstedet.

## 📝 bidrager til dokumentationen

Dokumentationsfiler lever under mappen `streammaster.docs \ streammaster` i [streammaster depot] (https://github.com/senexcrenshaw/streammaster).

At bidrage til dokumentationen:

- ** Opret en ny gren ** til dine ændringer.
- ** Sørg for, at engelsk (`en`) altid opdateres **.Engelsk fungerer som det primære sprog, og alt indhold skal opdateres på engelsk.
- ** Giv de bedst mulige oversættelser ** til andre understøttede sprog, såsom spansk (`es '), fransk (` fr`), tysk (`de`) og alle andre understøttede sprog.Selvom disse oversættelser ikke behøver at være perfekte, skal de sigte mod nøjagtigt at afspejle det engelske indhold.
- Engelsk filer lever under `Docs/En`.
- Oversættelser lever under deres respektive mapper (f.eks. 'Dokumenter/ES' til spansk, 'Dokumenter/FR' til fransk osv.).
- ** Test ** Alle ændringer ved at køre `mkdocs serveres 'lokalt til både den engelske version og eventuelle opdaterede oversættelser.
- ** Indsend en PULL -anmodning (PR) ** til gennemgang.

### Kom godt i gang i 3 lette trin!

1. gaffel depotet og klon den til din lokale maskine.
2. Opret en ny gren til dine ændringer.
3. Sørg for, at engelsk (`da`) opdateres og giver de bedst mulige oversættelser til andre understøttede sprog, og derefter indsende en PR.

Det er det!🎉 Du har bidraget til Streammaster!

## Tips til at skrive god dokumentation

- ** Vær klar og kortfattet: ** Fokuser på hovedpunkterne, og undgå alt for teknisk sprog, hvor det er muligt.
- ** Brug eksempler: ** Kodestykker eller visuelle hjælpemidler hjælper med at afklare komplekse koncepter.
- ** Vær konsekvent: ** Hold tone og terminologi konsistent på tværs af alle sektioner.
- ** Test alt: ** Sørg for, at alle kodeeksempler, kommandoer og instruktioner fungerer som forventet.

## Bidragsgenkendelsesgenkendelse 🌟

Vi værdsætter ethvert bidrag, uanset hvor lille!Alle bidragydere tilføjes til Streammaster Documentation Hall of Fame, der er vist nedenfor:

[Se alle bidragydere] (bidragydere.md)

## Vi har brug for din hjælp!🤝

Streammaster vokser konstant, og vi har brug for samfundets hjælp til at holde dokumentationen top-notch.Intet bidrag er for lille, hvad enten det er at fikse skrivefejl, tilføje eksempler eller oversætte indhold.

Spring ind, og lad os gøre Streammaster bedre sammen!✨

## har brug for hjælp eller har spørgsmål?Deltag i uenighed!🎮

For spørgsmål, support eller diskussioner, kan du deltage i den officielle ** streammaster Discord Server **.

👉 [Deltag i Streammaster Discord] (https://discord.gg/gfz7ethhg2) 👈

Vi er her for at hjælpe, og du finder et aktivt samfund af udviklere og brugere.Du er velkommen til at stille spørgsmål, rapportere spørgsmål eller diskutere nye ideer til forbedring af Streammaster!