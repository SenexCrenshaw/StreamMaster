/* eslint-disable @typescript-eslint/no-unused-vars */
import { type DataTableFilterMeta } from 'primereact/datatable';
import { useEffect, useMemo } from 'react';
import { type MatchMode, type SMDataTableFilterMetaData } from '../../common/common';
import { hasValidAdditionalProps, type AdditionalFilterProps, addOrUpdateValueForField, isEmptyObject, removeValueForField } from '../../common/common';
import { type ColumnMeta } from './DataSelectorTypes';
import { type LazyTableState } from './DataSelectorTypes';
import generateFilterData from './generateFilterData';
import { useQueryFilter } from '../../app/slices/useQueryFilter';

const useLazyTableState = (id: string, columns: ColumnMeta[], first: number, filters: DataTableFilterMeta, showHidden: boolean | null | undefined, additionalFilterProps: AdditionalFilterProps | undefined, sortField: string, sortOrder: -1 | 0 | 1, page: number, rows: number, defaultSortField: string | undefined) => {
  const { queryFilter, setQueryFilter } = useQueryFilter(id);

  const lazyState = useMemo((): LazyTableState => {

    const newFilters = generateFilterData(columns, filters, showHidden);

    let sort = '';
    if (sortField) {
      sort = (sortOrder === -1) ? `${sortField} desc` : (sortOrder === 1) ? `${sortField} asc` : '';
    }

    const defaultState: LazyTableState = {
      filters: newFilters,
      first: first,
      jsonFiltersString: '',
      page: page,
      rows: rows,
      sortField: sortField,
      sortOrder: sortOrder,
      sortString: sort
    };

    return {
      ...defaultState,
    };
  }, [columns, filters, first, page, rows, showHidden, sortField, sortOrder]);


  const generateFilteredData = useMemo(() => {
    const toSend: SMDataTableFilterMetaData[] = Object.keys(lazyState.filters)
      .map(key => {
        const value = lazyState.filters[key] as SMDataTableFilterMetaData;
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

    const toFilter = lazyState;
    return {
      jsonFiltersString: JSON.stringify(toSend),
      orderBy: toFilter.sortString || defaultSortField,
      pageNumber: toFilter.page,
      pageSize: toFilter.rows,
    };
  }, [additionalFilterProps, defaultSortField, lazyState]);


  useEffect(() => {

    // Main Logic
    // if (isEmptyObject(filters) && !hasValidAdditionalProps(additionalFilterProps)) {
    if (!hasValidAdditionalProps(additionalFilterProps)) {
      setQueryFilter({ pageSize: 40 });
    }

    const getApi = generateFilteredData;
    setQueryFilter(getApi || { pageSize: 40 });

  }, [additionalFilterProps, setQueryFilter, generateFilteredData]);

  return { lazyState };
};

export default useLazyTableState;
