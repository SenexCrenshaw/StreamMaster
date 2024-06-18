import { FileProfileColumnConfigProps } from './useFileProfileColumnConfig';
import { useStreamGroupProfileColumnConfig } from './useStreamGroupProfileColumnConfig';

export const useStreamGroupProfileFileProfileColumnConfig = (props?: FileProfileColumnConfigProps) => {
  const columnConfig = useStreamGroupProfileColumnConfig({ ...props, field: 'FileProfileName', header: 'File Profile' });
  return columnConfig;
};
