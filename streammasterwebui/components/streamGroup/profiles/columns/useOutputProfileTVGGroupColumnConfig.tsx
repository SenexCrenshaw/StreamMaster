import { OutputProfileColumnConfigProps, useOutputProfileColumnConfig } from './useOutputProfileColumnConfig';

export const useOutputProfileTVGGroupColumnConfig = (props?: OutputProfileColumnConfigProps) => {
  const test = useOutputProfileColumnConfig({ ...props, field: 'TVGGroup', header: 'TVG Group' });
  return test;
};
