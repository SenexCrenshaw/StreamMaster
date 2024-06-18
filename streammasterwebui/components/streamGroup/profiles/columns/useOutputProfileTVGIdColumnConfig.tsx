import { OutputProfileColumnConfigProps, useOutputProfileColumnConfig } from './useOutputProfileColumnConfig';

export const useOutputProfileTVGIdColumnConfig = (props?: OutputProfileColumnConfigProps) => {
  const test = useOutputProfileColumnConfig({ ...props, field: 'TVGId', header: 'TVG Id' });
  return test;
};
