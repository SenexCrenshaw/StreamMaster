import { FileProfileColumnConfigProps, useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileChannelIdColumnConfig = (props?: FileProfileColumnConfigProps) => {
  const test = useFileProfileColumnConfig({ ...props, field: 'ChannelId', header: 'Channel Id' });
  return test;
};
