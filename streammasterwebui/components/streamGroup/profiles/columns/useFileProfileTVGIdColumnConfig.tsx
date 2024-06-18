import { FileProfileColumnConfigProps, useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileTVGIdColumnConfig = (props?: FileProfileColumnConfigProps) => {
  const test = useFileProfileColumnConfig({ ...props, field: 'TVGId', header: 'TVG Id' });
  return test;
};
