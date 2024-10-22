# 📘 StreamMaster -Dokumentation

Dieses Dokument beschreibt die Schritte zum Einrichten und Verwalten von Dokumentationen für das Projekt [StreamMaster] (https://github.com/senexcenshaw/streammaster) mit MKDOCs.Das Setup beinhaltet die Unterstützung für Internationalisierung (I18N) und die Verwendung von MKDOCS -Materialthema für ein modernes Erscheinungsbild.

## Warum zur Dokumentation beitragen?

Die Dokumentation ist das erste, worauf Benutzer und Entwickler bei der Verwendung oder zum Beitrag zu StreamMaster verweisen.Indem Sie die Dokumentation verbessern und aufrechterhalten, stellen Sie sicher, dass StreamMaster für die Community zugänglich und einfach zu bedienen ist.

Ihre Beiträge zur Dokumentation:
- Helfen Sie anderen, StreamMaster effektiver zu lernen und zu verwenden.
- Verbesserung der Klarheit für nicht einheimische englische Sprecher durch bessere Unterstützung von i18n.
- Ermöglichen Sie den Entwicklern, leichter zum Projekt beizutragen.

Selbst kleine Updates wie die Korrektur von Tippfehler oder das Kläranweisungen machen einen großen Unterschied!

## 🛠 Voraussetzungen

Um Dokumentationen lokal zu generieren und zu bedienen, benötigen Sie Python installiert.Stellen Sie sicher, dass "Pip", Python's Package Manager, ebenfalls verfügbar ist.

### Installieren von MKDOCs und Plugins

Um MKDOCs und die erforderlichen Plugins für I18N und Theming zu installieren, führen Sie den folgenden Befehl aus:

`` `bash
Python-M PIP Installation MKDOCS MKDOCS-I18N MKDOCS-MATERIAL MKDOCS-STATIC-I18N
`` `

Dies installiert Folgendes:

- `mkdocs`: Der statische Site -Generator.
- `mkdocs-i18n`: zur Behandlung von Internationalisierung.
- `mkdocs-material`: Ein beliebtes Thema mit einem modernen Design.
-`mkdocs-static-i18n`: Fügt statische Internationalisierung Unterstützung hinzu.

## Lokale Entwicklung

Befolgen Sie diese Schritte, um die Dokumentation während der Entwicklung vor Ort zu erstellen und zu bedienen.

### Bauen der Dokumente

Um die statische Dokumentation zu generieren:

`` `bash
Mkdocs Build
`` `

### Die Dokumente vor Ort bedienen

Um einen Entwicklungsserver auszuführen, der nach Änderungen beobachtet und automatisch nachgeladen wird:

`` `bash
Mkdocs dienen
`` `

Dadurch werden die Dokumentation lokal unter `http: // localhost: 8000` gehostet.

## Produktion Build

Wenn Sie bereit sind, die Dokumentation für die Produktion bereitzustellen, stellen Sie sicher, dass Sie den vorherigen Build reinigen und die URL der Website korrekt einstellen.Führen Sie den folgenden Befehl aus:

`` `bash
Mkdocs Build-Clean ----Site-url https://senexcenshaw.github.io/streammaster/
`` `

Dies erstellt eine saubere Version der Dokumentation und legt die richtige URL für die Produktionsstelle fest.

## 📝 beiträgt zur Dokumentation bei

Dokumentationsdateien live unter dem Ordner "StreamMaster.docs \ StreamMaster" im [StreamMaster -Repository] (https://github.com/senexcenshaw/streammaster).

Um zur Dokumentation beizutragen:

- ** Erstellen Sie einen neuen Zweig ** für Ihre Änderungen.
- ** Stellen Sie sicher, dass Englisch (`en`) immer aktualisiert wird **.Englisch dient als primäre Sprache, und alle Inhalte müssen auf Englisch aktualisiert werden.
- ** Bieten Sie die bestmöglichen Übersetzungen ** für andere unterstützte Sprachen wie Spanisch (`Es`), Französisch (` fr`), Deutsch (`de`) und andere unterstützte Sprachen.Obwohl diese Übersetzungen nicht perfekt sein müssen, sollten sie darauf abzielen, den englischen Inhalt genau widerzuspiegeln.
- Englische Dateien live unter `docs/en`.
- Übersetzungen leben in ihren jeweiligen Verzeichnissen (z. B. "docs/es" für Spanisch, "docs/fr" für Französisch usw.).
- ** Test ** Alle Änderungen durch Ausführen von `mkdocs dienen sowohl für die englische Version als auch für alle aktualisierten Übersetzungen.
- ** Senden Sie eine Pull -Anfrage (PR) ** zur Überprüfung.

### Start in 3 einfache Schritte!

1. Geben Sie das Repository auf und klonen Sie es auf Ihre lokale Maschine.
2. Erstellen Sie einen neuen Zweig für Ihre Änderungen.
3. Stellen Sie sicher, dass Englisch (`en`) aktualisiert wird und die bestmöglichen Übersetzungen für andere unterstützte Sprachen bereitstellt, und dann eine PR einreichen.

Das ist es!🎉 Sie haben zum StreamMaster beigetragen!

## Tipps zum Schreiben einer guten Dokumentation

- ** Seien Sie klar und präzise: ** Konzentrieren Sie sich auf die Hauptpunkte und vermeiden Sie nach Möglichkeit übermäßig technische Sprache.
- ** Verwenden Sie Beispiele: ** Code -Snippets oder visuelle Hilfsmittel helfen dabei, komplexe Konzepte zu klären.
- ** Seien Sie konsistent: ** Halten Sie den Ton und die Terminologie in allen Abschnitten konsistent.
- ** alles testen: ** Stellen Sie sicher, dass alle Code -Beispiele, -befehle und Anweisungen wie erwartet funktionieren.

## Mitwirkende Anerkennung 🌟

Wir schätzen jeden Beitrag, egal wie klein!Alle Mitwirkenden werden in die unten vorgestellte StreamMaster -Dokumentation Hall of Fame hinzugefügt:

[Alle Mitwirkenden anzeigen] (Mitwirkende.md)

## Wir brauchen Ihre Hilfe!🤝

StreamMaster wächst ständig und wir brauchen die Hilfe der Community, um die Dokumentation erstklassig zu halten.Kein Beitrag ist zu klein, unabhängig davon, ob es sich um Tippfehler handelt, Beispiele hinzuzufügen oder Inhalte zu übersetzen.

Springen Sie hinein und lassen Sie uns Streammaster zusammen besser machen!✨

## Benötigen Sie Hilfe oder haben Sie Fragen?Begleiten Sie uns auf Zwietracht!🎮

Für Fragen, Unterstützung oder Diskussionen können Sie sich dem offiziellen ** StreamMaster Discord Server ** anschließen.

👉 [Join Streammaster Discord] (https://discord.gg/gfz7ethhg2) 👈

Wir sind hier, um zu helfen, und Sie werden eine aktive Community von Entwicklern und Benutzern finden.Fühlen Sie sich frei, Fragen zu stellen, Probleme zu melden oder neue Ideen zur Verbesserung von StreamMaster zu diskutieren!