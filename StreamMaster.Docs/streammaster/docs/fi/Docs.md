# 📘 Streammaster -dokumentaatio

Tässä asiakirjassa hahmotellaan vaiheet [Streammaster] (https://github.com/senexcrenshaw/streammaster) -projektin määrittämiseen ja hallintaan MKDOCS: n avulla.Asennus sisältää tuen kansainvälistymiselle (I18N) ja MKDOCS -materiaaliteeman käytön modernin ilmeen ja tunteen suhteen.

## Miksi osallistua dokumentointiin?

Dokumentaatio on ensimmäinen asia, jonka käyttäjät ja kehittäjät viittaavat Streammaster -käyttäessään tai siihen osallistuessaan.Auttamalla parantamaan ja ylläpitämään dokumentaatiota varmistat, että Streammaster on edelleen saatavissa ja helppokäyttöinen yhteisölle.

Palautuksesi dokumentaatioon:
- Auta muita oppimaan ja käyttämään Streammasteria tehokkaammin.
- Paranna muiden kuin kotoperäisten englanninkielisten selkeyttä paremmalla i18n-tuella.
- Antaa kehittäjille helpommin osallistua projektiin.

Jopa pienillä päivityksillä, kuten kirjoitusvirheiden korjaamisella tai ohjeiden selventämisellä, on suuri ero!

## 🛠 Edellytykset

Jotta voit luoda ja palvella asiakirjoja paikallisesti, tarvitset Pythonin asennettuna.Varmista, että Pythonin pakettipäällikkö "Pip" on myös saatavana.

### MkDocsin ja laajennusten asentaminen

Asentaaksesi MkDocs ja vaadittavat laajennukset i18n: lle ja Themingille, suorita seuraava komento:

`` `Bash
Python -M Pip asentaa mkDocs MkDocs-I18n Mkdocs-Material MkDocs -Static-I18n
`` `

Tämä asentaa seuraavan:

- `mkDocs`: Staattinen sivuston generaattori.
- `MkDocs-I18n`: Kansainvälistymisen käsittelemiseksi.
- `MkDocs-Material`: Suosittu teema, jolla on moderni muotoilu.
-`MkDocs-static-I18n`: lisää staattista kansainvälistymistukea.

## Paikallinen kehitys

Noudata näitä vaiheita rakentaaksesi ja palvellaksesi dokumentaatiota paikallisesti kehityksen aikana.

### Dokumenttien rakentaminen

Staattisen asiakirjan luominen:

`` `Bash
mkDocs rakenne
`` `

### asiakirjojen palveleminen paikallisesti

Suorita kehityspalvelin, joka tarkkailee muutoksia ja lataa uudelleen automaattisesti:

`` `Bash
Mkdocs palvelee
`` `

Tämä isännöi dokumentaatiota paikallisesti `http: // localhost: 8000`.

## Tuotannon rakennus

Kun olet valmis ottamaan käyttöön dokumentaation tuotantoon, varmista, että puhdistat edellisen rakennuksen ja aseta sivuston URL -osoite oikein.Suorita seuraava komento:

`` `Bash
MkDocs Build-Clean--Site-Url https://senexcrenshaw.github.io/streammaster/
`` `

Tämä rakentaa dokumentoinnin puhtaan version ja asettaa oikean URL -osoitteen tuotantosivustolle.

## 📝 Osallistuminen dokumentointiin

Dokumentaatiotiedostot suoritetaan `streammaster.docs \ streammaster` -kansiossa [Streammaster -arkistossa] (https://github.com/senexcrenshaw/streammaster).

Osallistuminen asiakirjoihin:

- ** Luo uusi haara ** muutoksiin.
- ** Varmista, että englanti (`en`) päivitetään aina **.Englanti toimii ensisijaisena kielenä, ja kaikki sisältö on päivitettävä englanniksi.
- ** Tarjoa parhaat mahdolliset käännökset ** muille tuetuille kielille, kuten espanjalle (`ES`), ranskaksi (` fr`), saksalle (`de`) ja muille tuetuille kielille.Vaikka näiden käännösten ei tarvitse olla täydellisiä, niiden tulisi pyrkiä heijastamaan tarkasti englanninkielistä sisältöä.
- Englanninkieliset tiedostot elävät `docs/fi`.
- Käännökset elävät heidän hakemistojensa alla (esim. "Docs/Es" espanjalle, `Docs/Fr` ranskan kielelle jne.).
- ** Testi ** Kaikki muutokset suorittamalla `mkDocs palvelee paikallisesti sekä englanninkieliselle versiolle että päivitetyille käännöksille.
- ** Lähetä vetopyyntö (PR) ** tarkistettavaksi.

### Aloita 3 helppoa vaihetta!

1. Haarukka arkisto ja klooni se paikalliselle koneellesi.
2. Luo uusi haara muutoksiin.
3. Varmista, että englanti (`en`) päivitetään ja tarjoa parhaat mahdolliset käännökset muille tuetuille kielille, ja lähetä sitten PR.

Se on se!🎉 Olet osallistunut Streammasteriin!

## Vinkkejä hyvän dokumentoinnin kirjoittamiseen

- ** Ole selkeä ja tiivis: ** Keskity pääpisteisiin ja vältä liian teknistä kieltä mahdollisuuksien mukaan.
- ** Käytä esimerkkejä: ** Koodinpätkät tai visuaaliset apuvälineet auttavat selventämään monimutkaisia ​​käsitteitä.
- ** Ole johdonmukainen: ** Pidä ääni ja terminologia yhdenmukainen kaikissa osioissa.
- ** Testaa kaikki: ** Varmista, että kaikki koodiesimerkit, komennot ja ohjeet toimivat odotetusti.

## Avustajan tunnustus 🌟

Arvostamme kaikkia panoksia riippumatta siitä kuinka pieni!Kaikki avustajat lisätään Streammaster -dokumentaatio Hall of Fame, esitelty alla:

[Näytä kaikki avustajat] (avustajat.md)

## Tarvitsemme apuasi!🤝

Streammaster kasvaa jatkuvasti, ja tarvitsemme yhteisön apua dokumentoinnin huippuluokan pitämiseen.Mikään panos ei ole liian pieni riippumatta siitä, onko kyseessä kirjoitusvirheitä, lisäämällä esimerkkejä tai sisällön kääntämistä.

Hyppää sisään, ja teemme Streammaster paremmin yhdessä!✨

## Tarvitsetko apua vai onko sinulla kysymyksiä?Liity mukaan Discordiin!🎮

Jos sinulla on kysyttävää, tukea tai keskusteluja, voit liittyä viralliseen ** Streammaster Discord -palvelimeen **.

👉 [Liity Streammaster Discord] (https://discord.gg/gfz7thhg2) 👈

Olemme täällä auttamassa, ja löydät aktiivisen kehittäjien ja käyttäjien yhteisön.Voit vapaasti kysyä kysymyksiä, ilmoittaa kysymyksistä tai keskustella uusista ideoista Streammasterin parantamiseksi!