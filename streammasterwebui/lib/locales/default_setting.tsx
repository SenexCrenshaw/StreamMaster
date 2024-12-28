interface DefaultSettingType {
  [key: string]: string;
}

const defaultSetting: DefaultSettingType = {
  AppendChannelName: 'true',
  'SDSettings.MaxSubscribedLineups': '4',
  APIKey: 'System Generated',
  AuthenticationMethod: 'None',
  BackupEnabled: 'true',
  BackupInterval: '4',
  BackupVersionsToKeep: '18',
  CleanURLs: 'true',
  ClientReadTimeoutMs: '0 - Disabled',
  ClientUserAgent: 'VLC/3.0.20-git LibVLC/3.0.20-git',
  DefaultPort: '7095',
  DefaultSSLPort: '7096',
  M3U8OutPutProfile: 'SMFFMPEG',
  DeviceID: 'device1',
  EnablePrometheus: 'false',
  EnableSSL: 'false',
  FFMPegExecutable: 'ffmpeg',
  FFMpegOptions: '-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1',
  FFProbeExecutable: 'ffprobe',
  ReadTimeOutMs: '0 - Disabled',
  StreamRetryHours: '1',
  StreamStartTimeoutMs: '0 - Disabled',
  StreamReadTimeOutMs: '0 - Disabled',
  StreamRetryLimit: '3',
  GlobalStreamLimit: '1',
  MaxLogFiles: '10',
  STRMBaseURL: 'http://localhost:7095',
  MaxLogFileSizeMB: '1',
  ShowClientHostNames: 'false',
  StreamingClientUserAgent: 'VLC/3.0.20-git LibVLC/3.0.20-git',
  StreamingProxyType: 'System Default',
  StreamShutDownDelayMs: '1000 - 1 Second'
};

export const getDefaultSetting = (key: string): string => defaultSetting[key] ?? '';

export default defaultSetting;
