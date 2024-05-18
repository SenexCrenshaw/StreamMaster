import { QueryHook } from '@lib/apiDefs';
import { PagedResponse, QueryStringParameters } from '@lib/smAPI/smapiTypes';
import { Column } from 'primereact/column';
import { DataTableRowClickEvent, DataTableRowData, DataTableRowEvent, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { CSSProperties, MouseEventHandler, ReactNode } from 'react';
import { ColumnMeta } from './ColumnMeta';
import { DataSelectorSelectionMode } from './smDataTableTypes';

export interface DataTableHeaderProperties {
  headerLeftTemplate?: ReactNode;
  headerRightTemplate?: ReactNode;
  selectionMode?: DataSelectorSelectionMode;
}

interface BaseDataSelectorProperties<T> extends DataTableHeaderProperties {
  addOrRemoveHeaderTemplate?: () => ReactNode;
  addOrRemoveTemplate?: (data: T) => ReactNode;
  className?: string;
  columns: ColumnMeta[];
  defaultSortField?: string;
  defaultSortOrder?: -1 | 0 | 1;
  emptyMessage?: ReactNode;
  enableClick?: boolean;
  enableExport?: boolean;
  enableHeaderWrap?: boolean;
  enablePaginator?: boolean;
  expanderHeader?: () => ReactNode;
  extraColumns?: Column[];
  headerClassName?: string;
  headerName?: string;
  id: string;
  isLoading?: boolean;
  noSourceHeader?: boolean;
  reorderable?: boolean;
  rowClass?: (data: DataTableRowData<any>) => string;
  rowExpansionTemplate?: (data: DataTableRowData<T | any>, options: DataTableRowExpansionTemplate) => React.ReactNode;
  rows?: number;
  selectRow?: boolean;
  selectedItemsKey?: string;
  showExpand?: boolean;
  showSelections?: boolean;
  showHiddenInSelection?: boolean;
  style?: CSSProperties;
  headerSize?: 'small' | 'medium' | 'large';
  onRowReorder?: (value: T[]) => void;
  onSelectionChange?: (value: T[], selectAll: boolean) => void;
  onClick?: MouseEventHandler<T> | undefined;
  onMultiSelectClick?: (value: boolean) => void;
  onRowClick?(event: DataTableRowClickEvent): void;
  onRowCollapse?(event: DataTableRowEvent): void;
  onRowExpand?(event: DataTableRowEvent): void;
}

type QueryFilterProperties<T> = BaseDataSelectorProperties<T> & {
  dataSource?: T[] | undefined;
  queryFilter: (filters: QueryStringParameters) => ReturnType<QueryHook<PagedResponse<T> | T[]>>;
};

type DataSourceProperties<T> = BaseDataSelectorProperties<T> & {
  dataSource: T[] | undefined;
  queryFilter?: (filters: QueryStringParameters) => ReturnType<QueryHook<PagedResponse<T> | T[]>>;
};

export type SMDataTableProps<T> = DataSourceProperties<T> | QueryFilterProperties<T>;
