interface DefaultSettingType {
  [key: string]: string;
  CleanURLs: string;
  DeviceID: string;
  FFMPegExecutable: string;
}

const defaultSetting: DefaultSettingType = {
  ApiKey: 'System Generated',
  AuthenticationMethod: 'None',
  BackupEnabled: 'true',
  BackupInterval: '4',
  BackupVersionsToKeep: '18',
  CleanURLs: 'true',
  ClientUserAgent: 'VLC/3.0.20-git LibVLC/3.0.20-git',
  DeviceID: 'device1',
  EnablePrometheus: 'false',
  EnableSSL: 'false',
  FFMpegOptions: '-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1',
  FFMPegExecutable: 'ffmpeg',
  GlobalStreamLimit: '1',
  MaxLogFiles: '10',
  MaxLogFileSizeMB: '1',
  ShowClientHostNames: 'false',
  StreamingClientUserAgent: 'VLC/3.0.20-git LibVLC/3.0.20-git'
};

export const getDefaultSetting = (key: string): string => defaultSetting[key] ?? '';

export default defaultSetting;
