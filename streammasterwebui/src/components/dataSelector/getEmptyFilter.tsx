import { FilterMatchMode } from "primereact/api";
import { type DataTableFilterMeta } from "primereact/datatable";
import { type ColumnMeta } from "./DataSelectorTypes";

function getEmptyFilter(columns: ColumnMeta[], showHidden: boolean | null | undefined): DataTableFilterMeta {

  var filter = columns.reduce<DataTableFilterMeta>((obj, item: ColumnMeta) => {
    if (item.field === 'isHidden') {

      return {
        ...obj,
        [item.field]: {
          fieldName: item.field,
          matchMode: FilterMatchMode.EQUALS,
          value: showHidden === null ? null : !showHidden
        },
      } as DataTableFilterMeta;
    }

    let value = '';

    return {
      ...obj,
      [item.field]: {
        fieldName: item.field,
        matchMode: item.filterMatchMode ?? FilterMatchMode.CONTAINS,
        value: value
      },
    } as DataTableFilterMeta;
  }, {});

  return filter;
}

export default getEmptyFilter;
