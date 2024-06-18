import { VideoProfileColumnConfigProps, useVideoProfileColumnConfig } from './useVideoProfileColumnConfig';

export const useVideoProfileParametersColumnConfig = (props?: VideoProfileColumnConfigProps) => {
  const test = useVideoProfileColumnConfig({ ...props, field: 'Parameters', header: 'Parameters' });
  return test;
};
