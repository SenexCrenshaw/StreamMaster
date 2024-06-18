import { FileProfileColumnConfigProps, useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileChannelNumberColumnConfig = (props?: FileProfileColumnConfigProps) => {
  const test = useFileProfileColumnConfig({ ...props, field: 'ChannelNumber', header: 'Channel #' });
  return test;
};
