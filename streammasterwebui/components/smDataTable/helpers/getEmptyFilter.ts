import { FilterMatchMode } from 'primereact/api';
import { type DataTableFilterMeta } from 'primereact/datatable';
import { ColumnMeta } from '../types/ColumnMeta';

function getEmptyFilter(columns: ColumnMeta[], showHidden: boolean | null | undefined): DataTableFilterMeta {
  const filter = columns.reduce<DataTableFilterMeta>((object, item: ColumnMeta) => {
    if (item.field === 'iIHidden') {
      return {
        ...object,
        [item.field]: {
          fieldName: item.field,
          matchMode: FilterMatchMode.EQUALS,
          value: showHidden === null ? null : !showHidden
        }
      } as DataTableFilterMeta;
    }

    const value = '';

    return {
      ...object,
      [item.field]: {
        fieldName: item.field,
        matchMode: item.filterMatchMode ?? FilterMatchMode.CONTAINS,
        value
      }
    } as DataTableFilterMeta;
  }, {});

  return filter;
}

export default getEmptyFilter;
