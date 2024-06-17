import { useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileChannelIdColumnConfig = () => {
  const test = useFileProfileColumnConfig({ field: 'ChannelId', header: 'Channel Id' });
  return test;
};
