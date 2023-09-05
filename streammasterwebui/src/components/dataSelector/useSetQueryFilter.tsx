import { type DataTableFilterMeta } from "primereact/datatable";
import { useMemo, useEffect } from "react";
import { useQueryFilter } from "../../app/slices/useQueryFilter";
import { type SMDataTableFilterMetaData, type MatchMode, areGetApiArgsEqual } from "../../common/common";
import { hasValidAdditionalProps, isEmptyObject, removeValueForField, addOrUpdateValueForField } from "../../common/common";
import { type ColumnMeta, type LazyTableState } from "./DataSelectorTypes";
import generateFilterData from "./generateFilterData";
import { useQueryAdditionalFilters } from "../../app/slices/useQueryAdditionalFilters";
import { useSortInfo } from "../../app/slices/useSortInfo";
import { useShowHidden } from "../../app/slices/useShowHidden";
import { FilterMatchMode } from "primereact/api";

const useSetQueryFilter = (
  id: string,
  columns: ColumnMeta[],
  first: number,
  filters: DataTableFilterMeta,
  page: number,
  rows: number,
) => {

  const { sortInfo } = useSortInfo(id);
  const { queryAdditionalFilter } = useQueryAdditionalFilters(id);
  const { queryFilter, setQueryFilter } = useQueryFilter(id);
  const { showHidden } = useShowHidden(id);

  const { lazyState, generateGetApi } = useMemo(() => {
    const newFilters = generateFilterData(columns, filters);

    const sort = sortInfo ? sortInfo.sortField
      ? sortInfo.sortOrder === -1
        ? `${sortInfo.sortField} desc`
        : `${sortInfo.sortField} asc`
      : '' : '';

    const defaultState: LazyTableState = {
      filters: newFilters,
      first: first,
      jsonFiltersString: '',
      page: page,
      rows: rows,
      sortField: 'id',
      sortOrder: 1,
      sortString: sort,
    };

    const toSend: SMDataTableFilterMetaData[] = Object.keys(defaultState.filters)
      .map(key => {
        const value = defaultState.filters[key] as SMDataTableFilterMetaData;

        return value?.value && value.value !== '[]' ? value : null;
      })
      .filter(Boolean) as SMDataTableFilterMetaData[];

    if (showHidden === null) {
      removeValueForField(toSend, 'isHidden');
    } else if (showHidden !== undefined) {

      addOrUpdateValueForField(toSend, 'isHidden', FilterMatchMode.EQUALS, !showHidden);
    }

    if (hasValidAdditionalProps(queryAdditionalFilter)) {
      const addProps = queryAdditionalFilter;

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
  }, [columns, filters, sortInfo, first, page, rows, showHidden, queryAdditionalFilter]);

  useEffect(() => {
    const newApi = generateGetApi;// hasValidAdditionalProps(additionalFilterProps) ? generateGetApi : { pageSize: 40 };

    if (!areGetApiArgsEqual(newApi, queryFilter)) {
      // console.log('useSetQueryFilter', id, newApi);
      setQueryFilter(newApi);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [queryAdditionalFilter, setQueryFilter, generateGetApi]);

  return { lazyState };
};

export default useSetQueryFilter;
