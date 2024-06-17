import { useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileTVGIdColumnConfig = () => {
  const test = useFileProfileColumnConfig({ field: 'TVGId', header: 'TVG Id' });
  return test;
};
