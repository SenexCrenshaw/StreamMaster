import { useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileTVGGroupColumnConfig = () => {
  const test = useFileProfileColumnConfig({ field: 'TVGGroup', header: 'TVG Group' });
  return test;
};
