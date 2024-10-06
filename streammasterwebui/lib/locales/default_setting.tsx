interface DefaultSettingType {
  [key: string]: string;
}

const defaultSetting: DefaultSettingType = {
  'SDSettings.MaxSubscribedLineups': '4',
  ApiKey: 'System Generated',
  AuthenticationMethod: 'None',
  BackupEnabled: 'true',
  BackupInterval: '4',
  BackupVersionsToKeep: '18',
  CleanURLs: 'true',
  ClientReadTimeOutSeconds: '5',
  ClientUserAgent: 'VLC/3.0.20-git LibVLC/3.0.20-git',
  DeviceID: 'device1',
  EnablePrometheus: 'false',
  EnableSSL: 'false',
  FFMPegExecutable: 'ffmpeg',
  FFMpegOptions: '-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1',
  FFProbeExecutable: 'ffprobe',
  GlobalStreamLimit: '1',
  MaxLogFiles: '10',
  MaxLogFileSizeMB: '1',
  ShowClientHostNames: 'false',
  StreamingClientUserAgent: 'VLC/3.0.20-git LibVLC/3.0.20-git',
  StreamingProxyType: 'System Default'
};

export const getDefaultSetting = (key: string): string => defaultSetting[key] ?? '';

export default defaultSetting;
