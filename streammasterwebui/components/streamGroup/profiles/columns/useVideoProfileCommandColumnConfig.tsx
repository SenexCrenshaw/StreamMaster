import { VideoProfileColumnConfigProps, useVideoProfileColumnConfig } from './useVideoProfileColumnConfig';

export const useVideoProfileCommandColumnConfig = (props?: VideoProfileColumnConfigProps) => {
  const test = useVideoProfileColumnConfig({ ...props, field: 'Command', header: 'Command' });
  return test;
};
