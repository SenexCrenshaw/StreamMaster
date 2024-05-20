import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { type SMDataTableFilterMetaData } from '@lib/common/common';
import { FilterMatchMode } from 'primereact/api';
import { type DataTableFilterMeta } from 'primereact/datatable';

function generateFilterData(columns: ColumnMeta[], currentFilters: DataTableFilterMeta): DataTableFilterMeta {
  if (!columns || !currentFilters || Object.keys(currentFilters).length === 0) {
    return {};
  }

  const filterColumnData = (column: ColumnMeta): [string, SMDataTableFilterMetaData] => {
    const filterData = currentFilters[column.field] as SMDataTableFilterMetaData;
    const matchMode = filterData?.matchMode ?? column.filterMatchMode ?? FilterMatchMode.CONTAINS;
    const value = filterData?.value ?? '';
    return [column.field, { fieldName: column.field, matchMode, value }];
  };

  return columns.reduce<DataTableFilterMeta>((accumulator, column) => {
    const [field, filterData] = filterColumnData(column);
    accumulator[field] = filterData;
    return accumulator;
  }, {});
}

export default generateFilterData;
