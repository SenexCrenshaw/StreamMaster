import { type DataTableFilterMeta } from "primereact/datatable";
import { useMemo, useEffect } from "react";
import { useQueryFilter } from "../../app/slices/useQueryFilter";
import { type AdditionalFilterProps, type SMDataTableFilterMetaData, type MatchMode } from "../../common/common";
import { hasValidAdditionalProps, isEmptyObject, removeValueForField, addOrUpdateValueForField } from "../../common/common";
import { type ColumnMeta, type LazyTableState } from "./DataSelectorTypes";
import generateFilterData from "./generateFilterData";

const useSetQueryFilter = (
  id: string,
  columns: ColumnMeta[],
  first: number,
  filters: DataTableFilterMeta,
  showHidden: boolean | null | undefined,
  additionalFilterProps: AdditionalFilterProps | undefined,
  sortField: string,
  sortOrder: -1 | 0 | 1,
  page: number,
  rows: number,
) => {
  const { setQueryFilter } = useQueryFilter(id);

  const { lazyState, generateGetApi } = useMemo(() => {
    const newFilters = generateFilterData(columns, filters, showHidden);

    const sort = sortField
      ? sortOrder === -1
        ? `${sortField} desc`
        : `${sortField} asc`
      : '';

    const defaultState: LazyTableState = {
      filters: newFilters,
      first: first,
      jsonFiltersString: '',
      page: page,
      rows: rows,
      sortField: sortField,
      sortOrder: sortOrder,
      sortString: sort,
    };

    const toSend: SMDataTableFilterMetaData[] = Object.keys(defaultState.filters)
      .map(key => {
        const value = defaultState.filters[key] as SMDataTableFilterMetaData;
        return value?.value && value.value !== '[]' ? value : null;
      })
      .filter(Boolean) as SMDataTableFilterMetaData[];

    if (hasValidAdditionalProps(additionalFilterProps)) {
      const addProps = additionalFilterProps;
      if (addProps) {
        if (isEmptyObject(addProps.values)) {
          removeValueForField(toSend, addProps.field);
        } else {
          const values = JSON.stringify(addProps.values);
          addOrUpdateValueForField(toSend, addProps.field, addProps.matchMode as MatchMode, values);
        }
      }
    }

    const toFilter = defaultState;

    return {
      generateGetApi: {
        jsonFiltersString: JSON.stringify(toSend),
        orderBy: toFilter.sortString,
        pageNumber: toFilter.page,
        pageSize: toFilter.rows,
      },
      lazyState: defaultState,
    };
  }, [columns, filters, first, page, rows, showHidden, sortField, sortOrder, additionalFilterProps]);

  useEffect(() => {
    setQueryFilter(hasValidAdditionalProps(additionalFilterProps) ? generateGetApi : { pageSize: 40 });
  }, [additionalFilterProps, setQueryFilter, generateGetApi]);

  return { lazyState };
};

export default useSetQueryFilter;
