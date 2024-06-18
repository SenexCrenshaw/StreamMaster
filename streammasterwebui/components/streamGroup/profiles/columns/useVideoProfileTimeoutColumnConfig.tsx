import { VideoProfileColumnConfigProps, useVideoProfileColumnConfig } from './useVideoProfileColumnConfig';

export const useVideoProfileTimeoutColumnConfig = (props?: VideoProfileColumnConfigProps) => {
  const test = useVideoProfileColumnConfig({ ...props, field: 'Timeout', header: 'Timeout' });
  return test;
};
