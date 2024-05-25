interface DefaultSettingType {
  [key: string]: string;
  CleanURLs: string;
  DeviceID: string;
  FFMPegExecutable: string;
}

const defaultSetting: DefaultSettingType = {
  AdminPassword: '',
  AdminUserName: '',
  ApiKey: 'System Generated',
  AuthenticationMethod: 'None',
  BackupEnabled: 'true',
  BackupInterval: '4',
  BackupVersionsToKeep: '18',
  CleanURLs: 'true',
  DeviceID: 'device1',
  EnablePrometheus: 'false',
  EnableSSL: 'false',
  FFMPegExecutable: 'ffmpeg',
  MaxLogFiles: '10',
  MaxLogFileSizeMB: '1'
};

export const getDefaultSetting = (key: string): string => defaultSetting[key] ?? '';

export default defaultSetting;
