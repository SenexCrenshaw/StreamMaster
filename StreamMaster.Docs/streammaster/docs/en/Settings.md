# StreamMaster Settings Reference

This table outlines the StreamMaster settings, including the setting name, English display name, description, and default value for easy reference.

| Setting Key                   | Display Name                | Description                                                                  | Default Value                      |
| ----------------------------- | --------------------------- | ---------------------------------------------------------------------------- | ---------------------------------- |
| `AdminPassword`               | Admin Password              | Password for the admin user.                                                 | (empty)                            |
| `AdminUserName`               | Admin Username              | Username for the admin user.                                                 | (empty)                            |
| `ArtworkSize`                 | Artwork Size                | Default size for artwork.                                                    | Medium                             |
| `AuthenticationMethod`        | Method                      | Authentication Method                                                        | `None`                             |
| `AutoSetEPG`                  | Auto Set EPG                | Automatically set EPG (Electronic Program Guide).                            | `false`                            |
| `BackupEnabled`               | Enable Backups              | Enable or disable system backups.                                            | `true`                             |
| `BackupInterval`              | Interval                    | Backup interval in hours.                                                    | `4`                                |
| `BackupVersionsToKeep`        | Versions                    | Number of backup versions to keep before overwriting older backups.          | `18`                               |
| `CleanURLs`                   | No URLs in Logs             | Remove URLs from being logged.                                               | `true`                             |
| `ClientReadTimeOutSeconds`    | Client Read Timeout         | Timeout in seconds for client reads.                                         | `10`                               |
| `ClientUserAgent`             | Client User Agent           | User agent string for the client.                                            | `VLC/3.0.20-git LibVLC/3.0.20-git` |
| `DBBatchSize`                 | DB Batch Size               | Number of items to process per database batch.                               | `100`                              |
| `DefaultCommandProfileName`   | Default Command Profile     | Default command profile for streaming.                                       | `Default`                          |
| `DefaultCompression`          | Default Compression         | Default compression for M3U/EPG files.                                       | `gz`                               |
| `DefaultOutputProfileName`    | Default Output Profile      | Default output profile for streaming.                                        | `Default`                          |
| `DeviceID`                    | ALL HDHR ID                 | HDHR Device ID and capability ID.                                            | `device1`                          |
| `DummyRegex`                  | Dummy Regex                 | Regex to set EPG to dummy if it matches the channel EPG.                     | `(no tvg-id)`                      |
| `EnableSSL`                   | Enable SSL                  | Enable SSL for secure connections.                                           | `false`                            |
| `FFMPegExecutable`            | FFMPeg Executable           | Path or name of the FFMPeg executable.                                       | `ffmpeg`                           |
| `FFProbeExecutable`           | FFProbe Executable          | Path or name of the FFProbe executable.                                      | `ffprobe`                          |
| `GlobalStreamLimit`           | Global Stream Limit         | Limit for custom streams that do not belong to an M3U playlist.              | `1`                                |
| `IconCacheExpirationDays`     | Icon Cache Expiration Days  | Number of days before the icon cache expires.                                | `7`                                |
| `LogoCache`                   | Cache Logos                 | Cache logos locally to speed up access.                                      | `None`                             |
| `M3U8OutPutProfile`           | Default M3U8 Output Profile | Profile to use when the source is M3U8.                                      | `SMFFMPEG`                         |
| `MaxConcurrentDownloads`      | Max Concurrent Downloads    | Maximum number of concurrent downloads allowed.                              | `8`                                |
| `MaxConnectRetry`             | Connection Retry Limit      | Maximum number of retries for receiving data from the source stream.         | `20`                               |
| `MaxConnectRetryTimeMS`       | Retry Timeout in MS         | Timeout for each connection retry, in milliseconds.                          | `200`                              |
| `MaxLogFileSizeMB`            | Max Log Size                | Maximum size of log files in MB.                                             | `1`                                |
| `MaxLogFiles`                 | Max Log Files               | Maximum number of log files to keep.                                         | `10`                               |
| `MaxStreamReStart`            | Max Stream Restarts         | Maximum number of times a stream will automatically restart.                 | `3`                                |
| `MaxSubscribedLineups`        | Max Subscribed Lineups      | Maximum allowed subscribed lineups.                                          | `4`                                |
| `NameRegex`                   | Name Blacklist              | Blacklist patterns using regex.                                              | (empty list)                       |
| `PrettyEPG`                   | Format EPG output           | Formats the EPG for better readability.                                      | `false`                            |
| `ReadTimeOutMs`               | Read Timeout (ms)           | Timeout in milliseconds for reading data.                                    | `0`                                |
| `ShowClientHostNames`         | Show Client Hostnames       | Display client hostnames in the status panel.                                | `false`                            |
| `ShowIntros`                  | Show Intros                 | Show intros never, once at the beginning, or always.                         | `None`                             |
| `ShowMessageVideos`           | Show Message Videos         | Show videos with message content.                                            | `false`                            |
| `SSLCertPassword`             | SSL Certificate Password    | Password for the SSL certificate.                                            | (empty)                            |
| `SSLCertPath`                 | SSL Certificate Path        | File path for the SSL certificate.                                           | (empty)                            |
| `ShutDownDelay`               | Shutdown Delay (ms)         | Delay in milliseconds for shutting down streams after clients stop watching. | `1000`                             |
| `StreamStartTimeoutMs`        | Stream Start Timeout (ms)   | Timeout in milliseconds for starting streams.                                | `0`                                |
| `UrlBase`                     | URL Base                    | Base URL for the application.                                                | (empty)                            |
| `UiFolder`                    | UI Folder                   | Directory path for UI resources.                                             | `wwwroot`                          |
| `VideoStreamAlwaysUseEPGLogo` | Always use EPG Logo         | Always use the EPG logo for video streams.                                   | `true`                             |

This table provides an organized view of each setting in StreamMaster, ordered alphabetically by `Setting Key`.
