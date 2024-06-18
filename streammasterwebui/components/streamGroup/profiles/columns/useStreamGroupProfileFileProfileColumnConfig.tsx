import { FileProfileColumnConfigProps } from './useOutputProfileColumnConfig';
import { useStreamGroupProfileColumnConfig } from './useStreamGroupProfileColumnConfig';

export const useStreamGroupProfileFileProfileColumnConfig = (props?: FileProfileColumnConfigProps) => {
  const columnConfig = useStreamGroupProfileColumnConfig({ ...props, field: 'OutputProfileName', header: 'File Profile' });
  return columnConfig;
};
