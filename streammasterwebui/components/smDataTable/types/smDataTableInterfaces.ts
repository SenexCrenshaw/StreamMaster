import { QueryHook } from '@lib/apiDefs';
import { PagedResponse, QueryStringParameters } from '@lib/smAPI/smapiTypes';
import { Column } from 'primereact/column';
import { DataTableRowClickEvent, DataTableRowData, DataTableRowEvent, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { CSSProperties, MouseEventHandler, ReactNode } from 'react';
import { ColumnMeta } from './ColumnMeta';
import { DataSelectorSelectionMode } from './smDataTableTypes';

export interface DataTableHeaderProperties {
  headerCenterTemplate?: ReactNode;
  headerRightTemplate?: ReactNode;
  selectionMode?: DataSelectorSelectionMode;
}

interface BaseDataSelectorProperties<T> extends DataTableHeaderProperties {
  actionHeaderTemplate?: ReactNode;
  arrayKey?: string;
  className?: string;
  columns: ColumnMeta[];
  defaultSortField?: string;
  defaultSortOrder?: -1 | 0 | 1;
  emptyMessage?: ReactNode;
  enableClick?: boolean;
  enableExport?: boolean;
  enableHeaderWrap?: boolean;
  enablePaginator?: boolean;
  extraColumns?: Column[];
  headerClassName?: string;
  headerName?: string | React.ReactNode;
  headerSize?: 'small' | 'medium' | 'large';
  id: string;
  isLoading?: boolean;
  lazy?: boolean;
  noIsLoading?: boolean;
  singleExpand?: boolean;
  noSourceHeader?: boolean;
  reorderable?: boolean;
  rows?: number;
  selectedItemsKey?: string;
  selectRow?: boolean;
  setSelectedSMChannel?: boolean;
  showExpand?: boolean;
  showHiddenInSelection?: boolean;
  showSelectAll?: boolean;
  showSelected?: boolean;
  showSortSelected?: boolean;
  style?: CSSProperties;
  useSelectedItemsFilter?: boolean;
  onRowReorder?: (value: T[]) => void;
  onSelectionChange?: (value: T[], selectAll: boolean) => void;
  rowClass?: (data: DataTableRowData<any>) => string;
  rowExpansionTemplate?: (data: DataTableRowData<T | any>, options: DataTableRowExpansionTemplate) => React.ReactNode;
  addOrRemoveHeaderTemplate?: () => ReactNode;
  addOrRemoveTemplate?: (data: T) => ReactNode;
  expanderHeader?: () => ReactNode;
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

export interface SMDataTableRef {
  clearExpanded: () => void;
}

export type SMDataTableProps<T> = DataSourceProperties<T> | QueryFilterProperties<T>;
