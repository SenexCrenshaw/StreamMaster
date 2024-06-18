import { OutputProfileColumnConfigProps, useOutputProfileColumnConfig } from './useOutputProfileColumnConfig';

export const useOutputProfileChannelIdColumnConfig = (props?: OutputProfileColumnConfigProps) => {
  const test = useOutputProfileColumnConfig({ ...props, field: 'ChannelId', header: 'Channel Id' });
  return test;
};
