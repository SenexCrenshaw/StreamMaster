import { FileProfileColumnConfigProps, useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileTVGGroupColumnConfig = (props?: FileProfileColumnConfigProps) => {
  const test = useFileProfileColumnConfig({ ...props, field: 'TVGGroup', header: 'TVG Group' });
  return test;
};
