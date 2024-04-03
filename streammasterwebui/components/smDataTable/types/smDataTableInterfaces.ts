import { GetApiArgument, QueryHook } from '@lib/apiDefs';
import { PagedResponseDto } from '@lib/common/dataTypes';
import { Column } from 'primereact/column';
import { DataTableRowClickEvent, DataTableRowData, DataTableRowEvent, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { CSSProperties, MouseEventHandler, ReactNode } from 'react';
import { ColumnMeta } from './ColumnMeta';
import { DataSelectorSelectionMode } from './smDataTableTypes';

export interface DataTableHeaderProperties {
  headerRightTemplate?: ReactNode;
  headerLeftTemplate?: ReactNode;
  selectionMode?: DataSelectorSelectionMode;
}

interface BaseDataSelectorProperties<T> extends DataTableHeaderProperties {
  extraColumns?: Column[];
  noSourceHeader?: boolean;
  className?: string;
  id: string;
  columns: ColumnMeta[];
  isLoading?: boolean;
  defaultSortField?: string;
  defaultSortOrder?: -1 | 0 | 1;
  emptyMessage?: ReactNode;
  enableExport?: boolean;
  enableClick?: boolean;
  enablePaginator?: boolean;
  rows?: number;
  onRowCollapse?(event: DataTableRowEvent): void;
  rowExpansionTemplate?: (data: DataTableRowData<T | any>, options: DataTableRowExpansionTemplate) => React.ReactNode;
  addOrRemoveHeaderTemplate?: () => ReactNode;
  addOrRemoveTemplate?: (data: T) => ReactNode;
  showExpand?: boolean;
  style?: CSSProperties;
  selectedItemsKey?: string;
  onRowReorder?: (value: T[]) => void;
  reorderable?: boolean;
  showSelections?: boolean;
  onSelectionChange?: (value: T[], selectAll: boolean) => void;
  selectRow?: boolean;
  headerName?: string;
  onMultiSelectClick?: (value: boolean) => void;
  onClick?: MouseEventHandler<T> | undefined;
  onRowExpand?(event: DataTableRowEvent): void;
  onRowClick?(event: DataTableRowClickEvent): void;
  rowClass?: (data: DataTableRowData<any>) => string;
}

type QueryFilterProperties<T> = BaseDataSelectorProperties<T> & {
  dataSource?: T[] | undefined;
  queryFilter: (filters: GetApiArgument) => ReturnType<QueryHook<PagedResponseDto<T> | T[]>>;
};

type DataSourceProperties<T> = BaseDataSelectorProperties<T> & {
  dataSource: T[] | undefined;
  queryFilter?: (filters: GetApiArgument) => ReturnType<QueryHook<PagedResponseDto<T> | T[]>>;
};

export type SMDataTableProps<T> = DataSourceProperties<T> | QueryFilterProperties<T>;
