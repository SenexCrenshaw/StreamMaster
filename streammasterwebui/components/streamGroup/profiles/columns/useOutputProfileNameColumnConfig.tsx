import { OutputProfileColumnConfigProps, useOutputProfileColumnConfig } from './useOutputProfileColumnConfig';

export const useOutputProfileNameColumnConfig = (props?: OutputProfileColumnConfigProps) => {
  const columnConfig = useOutputProfileColumnConfig({ ...props, field: 'Name', header: 'Name' });
  return columnConfig;
};
