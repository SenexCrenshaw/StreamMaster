import { OutputProfileColumnConfigProps, useOutputProfileColumnConfig } from './useOutputProfileColumnConfig';

export const useOutputProfileGroupTitleColumnConfig = (props?: OutputProfileColumnConfigProps) => {
  const test = useOutputProfileColumnConfig({ ...props, field: 'GroupTitle', header: 'Group Title' });
  return test;
};
