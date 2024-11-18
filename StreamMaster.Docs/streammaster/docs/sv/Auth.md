# 🔐 Authentication

Stream Master stöder två primära autentiseringsmetoder, vilket garanterar säker åtkomst för både användargränssnittet (UI) och genererade länkar. Nedan finns en översikt av båda:

## 1. Autentisering av användargränssnitt (formulär/ingen)

Autentisering av användargränssnitt kontrollerar åtkomst till Stream Masters användargränssnitt. Det finns två alternativ för användargränssnittsautentisering, **Formulär** och **Inget**.

### 1.1 Formulär-autentisering

Med formulärbaserad autentisering måste användarna logga in med användarnamn och lösenord, vilket ökar säkerheten genom att åtkomsten begränsas till verifierade användare.

#### Ställ in

1. I Stream Master-webbgränssnittet navigerar du till avsnittet **Inställningar**.
2. Under **Inställningar** väljer du `Forms` som autentiseringsmetod.
3. Definiera inloggningsuppgifter för användare direkt i webbgränssnittet.

### 1.2 Inget (ingen autentisering)

Om du väljer **Inget** inaktiveras användargränssnittsautentisering, vilket ger obegränsad åtkomst till användargränssnittet. Det här alternativet är lämpligt för miljöer där åtkomsten kontrolleras på nätverksnivå eller där instansen används i en säker, privat miljö.

#### Ställ in

1. I Stream Master-webbgränssnittet navigerar du till **Inställningar** > **Autentisering**.
2. Ställ in autentiseringsmetoden på `None`.

> **⚠️ OBS**: Om du väljer **Inget** blir användargränssnittet tillgängligt för alla som har nätverksåtkomst till servern. Använd endast det här alternativet i betrodda och kontrollerade miljöer.

---

## 2. Autentisering av krypterad länk

Stream Master genererar HD Home Run (HDHR)-, XML- och M3U-länkar, som alla kan krypteras baserat på en **Stream Group (SG)-gruppnyckel**. Denna nyckelbaserade kryptering ger säkra länkar samtidigt som den tillåter en enklare, okrypterad länk för enkel lokal åtkomst.

### 2.1 SG Gruppnyckel för länkkryptering

Varje **Stream Group (SG)** i Stream Master har en **gruppnyckel** som används för att kryptera länkar. Du kan hantera denna nyckel i avsnittet **Strömmar** > **SG** i webbgränssnittet. Krypteringen säkrar extern åtkomst samtidigt som ett enkelt lokalt alternativ bibehålls.

#### Länktyper

- **Krypterade länkar**: Säkra HDHR-, XML- och M3U-länkar som genereras baserat på SG:s gruppnyckel.
- **Korta länkar**: Okrypterade, användarvänliga länkar avsedda för lokal åtkomst.

#### Ställ in

1. I Stream Master-webbgränssnittet navigerar du till **Strömmar** > **SG (Stream Group)**.
2. Ange en unik, stark gruppnyckel för att kryptera de länkar som genereras för gruppen.

#### Användning

- När gruppnyckeln har ställts in genereras automatiskt krypterade HDHR-, XML- och M3U-länkar med hjälp av denna nyckel.
- Om du vill återställa eller uppdatera krypteringen ändrar du gruppnyckeln. Observera att detta kräver att alla tidigare delade krypterade länkar uppdateras.

---

## 🔄 Ändra autentiserings- och länkinställningar

För att uppdatera inställningar för autentisering eller länkkryptering använder du Stream Master-webbgränssnittet i relevanta avsnitt. Ändringar tillämpas omedelbart utan att behöva redigera konfigurationsfilen manuellt.

---

{%
    include-markdown "../includes/_footer.md"
%}
