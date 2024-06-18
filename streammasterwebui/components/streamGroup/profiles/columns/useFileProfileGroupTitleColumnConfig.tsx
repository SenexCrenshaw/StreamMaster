import { FileProfileColumnConfigProps, useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileGroupTitleColumnConfig = (props?: FileProfileColumnConfigProps) => {
  const test = useFileProfileColumnConfig({ ...props, field: 'GroupTitle', header: 'Group Title' });
  return test;
};
