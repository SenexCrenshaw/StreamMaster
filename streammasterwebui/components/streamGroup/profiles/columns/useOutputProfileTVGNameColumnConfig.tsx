import { OutputProfileColumnConfigProps, useOutputProfileColumnConfig } from './useOutputProfileColumnConfig';

export const useOutputProfileTVGNameColumnConfig = (props?: OutputProfileColumnConfigProps) => {
  const columnConfig = useOutputProfileColumnConfig({ ...props, field: 'TVGName', header: 'TVG Name' });
  return columnConfig;
};
