import { CommandProfileColumnConfigProps, useStreamGroupProfileColumnConfig } from './useStreamGroupProfileColumnConfig';

export const useStreamGroupProfileFileProfileColumnConfig = (props?: CommandProfileColumnConfigProps) => {
  const columnConfig = useStreamGroupProfileColumnConfig({ ...props, field: 'OutputProfileName', header: 'File Profile' });
  return columnConfig;
};
