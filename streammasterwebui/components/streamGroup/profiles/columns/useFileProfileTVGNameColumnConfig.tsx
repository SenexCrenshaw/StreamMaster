import { useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileTVGNameColumnConfig = () => {
  const test = useFileProfileColumnConfig({ field: 'TVGName', header: 'TVG Name' });
  return test;
};
