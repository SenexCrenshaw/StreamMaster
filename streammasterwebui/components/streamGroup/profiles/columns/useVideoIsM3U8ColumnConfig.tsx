import { VideoProfileColumnConfigProps, useVideoProfileColumnConfig } from './useVideoProfileColumnConfig';

export const useVideoIsM3U8ColumnConfig = (props?: VideoProfileColumnConfigProps) => {
  const test = useVideoProfileColumnConfig({ ...props, field: 'IsM3U8', header: 'IsM3U8' });
  return test;
};
