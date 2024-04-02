import { type DataTableFilterMeta } from 'primereact/datatable';

export interface LazyTableState {
  filters: DataTableFilterMeta;
  first: number;
  jsonFiltersString: string | null | undefined;
  page: number;
  rows: number;
  sortField?: string;
  sortOrder?: -1 | 0 | 1 | null | undefined;
  sortString: string;
}
