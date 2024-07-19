import { VideoProfileColumnConfigProps, useVideoProfileColumnConfig } from './useCommandProfileColumnConfig';

export const useVideoProfileTimeoutColumnConfig = (props?: VideoProfileColumnConfigProps) => {
  const test = useVideoProfileColumnConfig({ ...props, field: 'Timeout', header: 'Timeout' });
  return test;
};
