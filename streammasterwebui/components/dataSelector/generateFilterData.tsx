import { type SMDataTableFilterMetaData } from '@lib/common/common';
import { FilterMatchMode } from 'primereact/api';
import { type DataTableFilterMeta } from 'primereact/datatable';
import { type ColumnMeta } from './DataSelectorTypes';

function generateFilterData(columns: ColumnMeta[], currentFilters: DataTableFilterMeta): DataTableFilterMeta {
  if (!columns || !currentFilters) {
    return {};
  }

  const returnValue = columns.reduce<DataTableFilterMeta>((object, item: ColumnMeta) => {
    let value = '';
    let matchMode = item.filterMatchMode ?? '';

    if (Object.keys(currentFilters).length > 0) {
      const test = currentFilters[item.field] as SMDataTableFilterMetaData;

      if (test !== undefined) {
        value = test.value;
        if (!matchMode && test.matchMode) {
          matchMode = test.matchMode;
        }
      }
    }

    return {
      ...object,
      [item.field]: {
        fieldName: item.field,
        matchMode: matchMode ?? FilterMatchMode.CONTAINS,
        value
      }
    } as DataTableFilterMeta;
  }, {});
  return returnValue;
}

export default generateFilterData;
