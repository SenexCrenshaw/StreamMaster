import { useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileChannelNumberColumnConfig = () => {
  const test = useFileProfileColumnConfig({ field: 'ChannelNumber', header: 'Channel #' });
  return test;
};
