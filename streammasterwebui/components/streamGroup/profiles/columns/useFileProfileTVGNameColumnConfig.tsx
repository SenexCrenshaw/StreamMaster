import { SelectItem } from 'primereact/selectitem';
import { useCallback } from 'react';
import { FileProfileColumnConfigProps, useFileProfileColumnConfig } from './useFileProfileColumnConfig';

export const useFileProfileTVGNameColumnConfig = (props?: FileProfileColumnConfigProps) => {
  const onChange = useCallback((value: SelectItem) => {}, []);

  const columnConfig = useFileProfileColumnConfig({ ...props, field: 'TVGName', header: 'TVG Name', onChange: onChange });
  return columnConfig;
};
