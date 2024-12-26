interface help_enType {
  [key: string]: string;
  FFMpegOptions: string;
}

const help_en: help_enType = {
  AppendChannelName: 'Append the channel name to the M3U URL',
  ClientReadTimeoutMs: 'Client Read Timeout MS',
  'sdSettings.alternateSEFormat': 'True: "S{0}:E{1} "  False: "s{0:D2}e{1:D2} ";',
  'SDSettings.MaxSubscribedLineups': 'Max Allowed Subscribed Lineups',
  APIKey: 'API Key',
  auth: 'Authentication',
  AuthenticationMethod: 'Authentication Method',
  backup: 'Backup',
  BackupEnabled: 'Enable Backups',
  BackupVersionsToKeep: 'Number of Backup Versions to keep',
  M3U8OutPutProfile: 'The profile to use when source is m3u8',
  BackupInterval: 'Backup Interval (hours)',
  LogoCache: 'Cache Logos to the local disk to speed things up',
  CleanURLs: 'Remove URLs from being logged',
  ShowIntros: 'Show Intros never, once at the beginning, or always',
  // ClientUserAgent: 'Client User Agent used for downloads, epg/m3u/icons/schedules direct',
  development: 'Development',
  DefaultCompression: 'Default Compression for m3u/epg files',
  DefaultPort: 'Default Port, Restart Required',
  DefaultSSLPort: 'Default SSL Port, Restart Required',
  DeviceID: 'HDHR Device ID and capability ID',
  DummyRegex: 'EPG will be set to dummy if this matches the channel EPG',
  EnableSSL: 'Enable SSL',
  EnablePrometheus: 'Enable Prometheus Metrics',
  FFMPegExecutable: 'FFMPeg Executable. The name will be searched in /config and then use the containers path',
  FFProbeExecutable: 'FFMProbe Executable. The name will be searched in /config and then use the containers path',
  FFMpegOptions: "'{streamUrl}' will be replaced with the stream URL.",
  filesEPG: 'Files / EPG',
  StreamRetryLimit: 'How many times to retry a stream',
  StreamReadTimeOutMs: 'Time to wait to read from the source stream in MS',
  StreamStartTimeoutMs: 'Time to wait to start the source stream in MS',

  general: 'General',
  GlobalStreamLimit: 'Limit for Custom Streams that do not belong to a M3U playlist',
  keywordSearch: 'Keyword Search',
  M3UIgnoreEmptyEPGID: 'Ignore Streams with an empty EPG ID or EPG ID of "Dummy"',
  maxConnectRetry: 'How many times to retry receiving data from the source stream',
  maxConnectRetryTimeMS: 'Receiving Data Retry Timeout in MS',
  MaxLogFiles: 'Max Log Files to keep',
  MaxLogFileSizeMB: 'Max Log File Size in MB',
  ShowClientHostNames: 'Show Client Host Names in status panel',
  overWriteM3UChannels: 'Overwrite M3U Channels Numbers even if they are set',
  password: 'Password',
  rememberme: 'Remember Me',
  PrettyEPG: 'Formats the EPG',
  sdPassword: 'Password - The displayed value is encypted and not the real password. Re-enter the real password to change',
  sdUserName: 'Username',
  settings: 'Settings',
  signin: 'Sign In',
  StreamShutDownDelayMs: 'Stream Shutdown Delay in miliseconds once all clients have stopped watching',
  signInSuccessful: 'Sign In Successful',
  signInUnSuccessful: 'Sign In Unsuccessful',
  signout: 'Sign Out',
  sslCertPassword: 'SSL Certificate Password',
  sslCertPath: 'SSL Certificate Path',
  streaming: 'Streaming',
  // StreamingClientUserAgent: 'Client User Agent used for video streams',
  StreamingProxyType:
    'Stream Buffer Type to use. None will just use the original M3U URLs, FFMPEG - run the stream through FFMPEG, Stream Master - run the stream through SM own proxy',
  useDummyEPGForBlanks: 'Use Dummy EPG for streams with missing EPG',
  user: 'User',
  VideoStreamAlwaysUseEPGLogo:
    'Always use EPG Logo for Video Stream. If the EPG is changed to one containing a logo then the video stream logo will be set to that'
};

export const getHelp = (key: string) => help_en[key];

export default help_en;
