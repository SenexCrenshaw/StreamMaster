type DefaultSettingType = {
  [key: string]: string;
  ffMpegOptions: string;
};

const defaultSetting: DefaultSettingType = {
  ffMpegOptions: '-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1',
};

export const getDefaultSetting = (key: string): string => {
  return defaultSetting[key] ?? '';
};

export default defaultSetting;
