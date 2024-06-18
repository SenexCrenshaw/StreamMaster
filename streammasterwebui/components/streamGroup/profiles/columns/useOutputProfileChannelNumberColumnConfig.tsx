import { OutputProfileColumnConfigProps, useOutputProfileColumnConfig } from './useOutputProfileColumnConfig';

export const useOutputProfileChannelNumberColumnConfig = (props?: OutputProfileColumnConfigProps) => {
  const test = useOutputProfileColumnConfig({ ...props, field: 'ChannelNumber', header: 'Channel #' });
  return test;
};
