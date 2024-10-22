# 📘 Streammaster skjöl

Í þessu skjali er gerð grein fyrir skrefunum til að setja upp og stjórna skjölum fyrir [straummeistarann] (https://github.com/senexcrenshaw/streammaster) verkefni með MKDOCS.Uppsetningin felur í sér stuðning við alþjóðavæðingu (I18N) og notkun MKDOCS efnisþema fyrir nútímalegt útlit og tilfinningu.

## Af hverju að stuðla að skjölunum?

Skjölin eru það fyrsta sem notendur og verktaki vísa til þegar þeir nota eða leggja sitt af mörkum til straummeistara.Með því að hjálpa til við að bæta og viðhalda skjölunum ertu að tryggja að straummeistari sé áfram aðgengilegur og auðveldur í notkun fyrir samfélagið.

Framlög þín til skjölanna:
- Hjálpaðu öðrum að læra og nota straummeistara á skilvirkari hátt.
- Bættu skýrleika fyrir enskumælandi sem ekki eru innfæddir með betri stuðningi i18n.
- Gerðu verktaki kleift að auðveldara stuðli að verkefninu.

Jafnvel litlar uppfærslur eins og að leiðrétta prentvillur eða skýra leiðbeiningar skipta miklu máli!

## 🛠 Forsendur

Til að búa til og þjóna skjölum á staðnum þarftu Python uppsett.Gakktu úr skugga um að „Pip“, pakkastjóri Python, sé einnig fáanlegur.

### Setja upp MKDOC og viðbætur

Til að setja upp MKDOC og nauðsynlegar viðbætur fyrir i18n og þemu, keyrðu eftirfarandi skipun:

`` `bash
python -m pip Settu upp mkdocs mkdocs-i18n mkdocs-efni mkdocs-truflanir-i18n
`` `

Þetta setur upp eftirfarandi:

- `MKDOCS`: Static Site Generator.
- `MKDOCS-I18N`: Til að meðhöndla alþjóðavæðingu.
- `MKDOCS-MATERIAL`: Vinsælt þema með nútímalegri hönnun.
-`MKDOCS-DATIT-I18N`: Bætir við kyrrstæðum alþjóðavæðingarstuðningi.

## Staðbundin þróun

Fylgdu þessum skrefum til að byggja upp og þjóna skjölunum á staðnum meðan á þróun stendur.

### Að byggja skjölin

Til að búa til truflanir skjölin:

`` `bash
MKDOCS Build
`` `

### þjóna skjölunum á staðnum

Til að keyra þróunarþjón sem fylgist með breytingum og endurhleðsla sjálfkrafa:

`` `bash
MKDOCS þjóna
`` `

Þetta mun hýsa skjölin á staðnum á `http: // localhost: 8000`.

## Framleiðslubygging

Þegar þú ert tilbúinn að beita skjölunum í framleiðslu skaltu tryggja að þú hreinsir fyrri smíði og stilltu vefslóð vefsins rétt.Keyra eftirfarandi skipun:

`` `bash
MKDOCS Build-Clean--Site-url https://senexcrenshaw.github.io/streammaster/
`` `

Þetta byggir upp hreina útgáfu af skjölunum og setur rétta url fyrir framleiðslusíðuna.

## 📝 stuðla að skjölunum

Skjalaskrár í beinni útsendingu undir `streammaster.docs \ streammaster` möppunni í [Streammaster Repository] (https://github.com/senexcrenshaw/streammaster).

Til að leggja sitt af mörkum í skjölunum:

- ** Búðu til nýja grein ** fyrir breytingar þínar.
- ** Gakktu úr skugga um að enska (`en`) sé alltaf uppfærð **.Enska þjónar sem aðal tungumálið og verður að uppfæra allt efni á ensku.
- ** Veittu bestu mögulegu þýðingar ** fyrir önnur studd tungumál, svo sem spænsku (`es`), franska (` fr`), þýska (`de`) og öll önnur studd tungumál.Þó að þessar þýðingar þurfi ekki að vera fullkomnar, ættu þær að stefna að því að endurspegla nákvæmlega enska innihaldið nákvæmlega.
- Enskar skrár búa undir `skjölum/en`.
- Þýðingar lifa undir viðkomandi möppum (t.d. `skjöl/es` fyrir spænsku,` skjöl/fr` fyrir frönsku osfrv.).
- ** Próf ** Allar breytingar með því að keyra `mkdocs þjóna“ á staðnum fyrir bæði ensku útgáfuna og allar uppfærðar þýðingar.
- ** Sendu inn PUG -beiðni (PR) ** til skoðunar.

### Byrjaðu í 3 einföldum skrefum!

1. Forku geymslan og klónar það við staðbundna vélina þína.
2. Búðu til nýja útibú fyrir breytingar þínar.
3. Gakktu úr skugga um að enska (`en`) sé uppfærð og gefðu bestu mögulegu þýðingar fyrir önnur studd tungumál og sendu síðan PR.

Það er það!🎉 Þú hefur lagt sitt af mörkum til straummeistara!

## Ábendingar til að skrifa góð skjöl

- ** Vertu skýr og hnitmiðuð: ** Einbeittu þér að aðalatriðum og forðastu of tæknilegt tungumál þar sem unnt er.
- ** Notaðu dæmi: ** kóða snippets eða sjónræn hjálpartæki hjálpa til við að skýra flókin hugtök.
- ** Vertu stöðugur: ** Haltu tón og hugtökum í samræmi við alla hluta.
- ** Prófaðu allt: ** Gakktu úr skugga um að öll kóða dæmi, skipanir og leiðbeiningar virki eins og búist var við.

## Viðurkenning framlags 🌟

Við þökkum hvert framlag, sama hversu lítið!Öllum þátttakendum verður bætt við Streammaster Documentation Hall of Fame, sem er að finna hér að neðan:

[Skoða alla framlag] (framlag.md)

## Við þurfum hjálp þína!🤝

Streammaster er stöðugt að vaxa og við þurfum hjálp samfélagsins til að halda skjölunum í efsta sæti.Ekkert framlag er of lítið, hvort sem það er að laga innsláttarvillur, bæta við dæmum eða þýða efni.

Hoppaðu inn og við skulum gera straummeistara betri saman!✨

## Þarftu hjálp eða hafa spurningar?Vertu með okkur á Discord!🎮

Fyrir allar spurningar, stuðning eða umræður geturðu tekið þátt í opinberum ** straummeistara Discord Server **.

👉 [Taktu þátt í Streammaster Discord] (https://discord.gg/gfz7ethhg2) 👈

Við erum hér til að hjálpa og þú munt finna virkt samfélag verktaki og notenda.Feel frjáls til að spyrja spurninga, tilkynna mál eða ræða nýjar hugmyndir til að bæta straummeistara!