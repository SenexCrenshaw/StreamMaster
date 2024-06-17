import { useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileGroupTitleColumnConfig = () => {
  const test = useFileProfileColumnConfig({ field: 'GroupTitle', header: 'Group Title' });
  return test;
};
