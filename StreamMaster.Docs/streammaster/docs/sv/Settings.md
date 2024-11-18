# StreamMaster Inställningar Referens

Denna tabell beskriver StreamMaster-inställningarna, inklusive inställningsnamn, engelskt visningsnamn, beskrivning och standardvärde för enkel referens.

| Inställningsnyckel            | Visningsnamn                | Beskrivning                                                                             | Standardvärde                      |
| ----------------------------- | --------------------------- | --------------------------------------------------------------------------------------- | ---------------------------------- |
| `AdminPassword`               | Admin Password              | Lösenord för admin-användaren.                                                          | (tom)                              |
| `AdminUserName`               | Admin Username              | Användarnamn för admin-användaren.                                                      | (tom)                              |
| `ArtworkSize`                 | Artwork Size                | Standardstorlek för illustrationer.                                                     | Medium                             |
| `AuthenticationMethod`        | Method                      | Autentiseringsmetod                                                                     | `None`                             |
| `AutoSetEPG`                  | Auto Set EPG                | Automatisk inställning av EPG (elektronisk programguide).                               | `false`                            |
| `BackupEnabled`               | Enable Backups              | Aktivera eller inaktivera säkerhetskopiering av systemet.                               | `true`                             |
| `BackupInterval`              | Interval                    | Backup-intervall i timmar.                                                              | `4`                                |
| `BackupVersionsToKeep`        | Versions                    | Antal säkerhetskopior som sparas innan äldre säkerhetskopior skrivs över.               | `18`                               |
| `CleanURLs`                   | No URLs in Logs             | Ta bort webbadresser från loggning.                                                     | `true`                             |
| `ClientReadTimeOutSeconds`    | Client Read Timeout         | Timeout i sekunder för klientläsningar.                                                 | `10`                               |
| `ClientUserAgent`             | Client User Agent           | Sträng för användaragent för klienten.                                                  | `VLC/3.0.20-git LibVLC/3.0.20-git` |
| `DBBatchSize`                 | DB Batch Size               | Antal objekt som ska bearbetas per databasbatch.                                        | `100`                              |
| `DefaultCommandProfileName`   | Default Command Profile     | Standardkommandoprofil för streaming.                                                   | `Default`                          |
| `DefaultCompression`          | Default Compression         | Standardkomprimering för M3U/EPG-filer.                                                 | `gz`                               |
| `DefaultOutputProfileName`    | Default Output Profile      | Standardutgångsprofil för streaming.                                                    | `Default`                          |
| `DeviceID`                    | ALL HDHR ID                 | ID för HDHR-enhet och förmåga.                                                          | `device1`                          |
| `DummyRegex`                  | Dummy Regex                 | Regex för att sätta EPG till dummy om den matchar kanalens EPG.                         | `(no tvg-id)`                      |
| `EnableSSL`                   | Enable SSL                  | Aktivera SSL för säkra anslutningar.                                                    | `false`                            |
| `FFMPegExecutable`            | FFMPeg Executable           | Sökväg eller namn på den körbara FFMPeg-filen.                                          | `ffmpeg`                           |
| `FFProbeExecutable`           | FFProbe Executable          | Sökväg eller namn på den körbara filen FFProbe.                                         | `ffprobe`                          |
| `GlobalStreamLimit`           | Global Stream Limit         | Begränsning för anpassade streams som inte hör till en M3U-spellista.                   | `1`                                |
| `IconCacheExpirationDays`     | Icon Cache Expiration Days  | Antal dagar innan ikonens cache upphör att gälla.                                       | `7`                                |
| `LogoCache`                   | Cache Logos                 | Cache logotyper lokalt för snabbare åtkomst.                                            | `None`                             |
| `M3U8OutPutProfile`           | Default M3U8 Output Profile | Profil att använda när källan är M3U8.                                                  | `SMFFMPEG`                         |
| `MaxConcurrentDownloads`      | Max Concurrent Downloads    | Högsta tillåtna antal samtidiga nedladdningar.                                          | `8`                                |
| `MaxConnectRetry`             | Connection Retry Limit      | Maximalt antal försök att ta emot data från källströmmen.                               | `20`                               |
| `MaxConnectRetryTimeMS`       | Retry Timeout in MS         | Timeout för varje nytt försök att ansluta, i millisekunder.                             | `200`                              |
| `MaxLogFileSizeMB`            | Max Log Size                | Maximal storlek på loggfiler i MB.                                                      | `1`                                |
| `MaxLogFiles`                 | Max Log Files               | Maximalt antal loggfiler som ska sparas.                                                | `10`                               |
| `MaxStreamReStart`            | Max Stream Restarts         | Maximalt antal gånger en stream ska startas om automatiskt.                             | `3`                                |
| `MaxSubscribedLineups`        | Max Subscribed Lineups      | Maximalt tillåtna abonnerade uppställningar.                                            | `4`                                |
| `NameRegex`                   | Name Blacklist              | Svartlistningsmönster med hjälp av regex.                                               | (tom lista)                        |
| `PrettyEPG`                   | Format EPG output           | Formaterar EPG:n för bättre läsbarhet.                                                  | `false`                            |
| `ReadTimeOutMs`               | Read Timeout (ms)           | Timeout i millisekunder för läsning av data.                                            | `0`                                |
| `ShowClientHostNames`         | Show Client Hostnames       | Visa klientens värdnamn i statuspanelen.                                                | `false`                            |
| `ShowIntros`                  | Show Intros                 | Visa intron aldrig, en gång i början eller alltid.                                      | `None`                             |
| `ShowMessageVideos`           | Show Message Videos         | Visa videor med budskapsinnehåll.                                                       | `false`                            |
| `SSLCertPassword`             | SSL Certificate Password    | Lösenord för SSL-certifikatet.                                                          | (tom)                              |
| `SSLCertPath`                 | SSL Certificate Path        | Filsökväg för SSL-certifikatet.                                                         | (tom)                              |
| `ShutDownDelay`               | Shutdown Delay (ms)         | Fördröjning i millisekunder för att stänga av strömmar efter att klienter slutat titta. | `1000`                             |
| `StreamStartTimeoutMs`        | Stream Start Timeout (ms)   | Timeout i millisekunder för start av strömmar.                                          | `0`                                |
| `UrlBase`                     | URL Base                    | Bas-URL för applikationen.                                                              | (tom)                              |
| `UiFolder`                    | UI Folder                   | Katalogsökväg för användargränssnittsresurser.                                          | `wwwroot`                          |
| `VideoStreamAlwaysUseEPGLogo` | Always use EPG Logo         | Använd alltid EPG-logotypen för videoströmmar.                                          | `true`                             |

Denna tabell ger en organiserad bild av varje inställning i StreamMaster, ordnad alfabetiskt efter `Setting Key`.
