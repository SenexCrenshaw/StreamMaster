import { SMDataTableFilterMetaData, addOrUpdateValueForField, hasValidAdditionalProps, isEmptyObject } from '@lib/common/common';

import { areGetApiArgsEqual } from '@lib/apiDefs';
import { useQueryAdditionalFilters } from '@lib/redux/slices/useQueryAdditionalFilters';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useShowHidden } from '@lib/redux/slices/useShowHidden';
import { useSortInfo } from '@lib/redux/slices/useSortInfo';
import { FilterMatchMode } from 'primereact/api';
import { DataTableFilterMeta } from 'primereact/datatable';
import { useEffect, useMemo } from 'react';
import { ColumnMeta } from './DataSelectorTypes';

function getSortString(sortInfo: any): string {
  return sortInfo && sortInfo.sortField ? `${sortInfo.sortField} ${sortInfo.sortOrder === -1 ? 'desc' : 'asc'}` : '';
}

function transformAndEnhanceFilters(
  filters: DataTableFilterMeta,
  columns: ColumnMeta[],
  showHidden: boolean | null | undefined,
  additionalFilter: any
): SMDataTableFilterMetaData[] {
  let transformedFilters: SMDataTableFilterMetaData[] = columns
    .map((column) => {
      const filter = filters[column.field] as SMDataTableFilterMetaData;
      return {
        fieldName: column.field,
        value: filter ? filter.value : '',
        matchMode: filter ? filter.matchMode : FilterMatchMode.CONTAINS
      };
    })
    .filter((filter) => filter.value && filter.value !== '[]');

  // Show/Hide logic
  if (showHidden !== null && showHidden !== undefined) {
    addOrUpdateValueForField(transformedFilters, 'isHidden', FilterMatchMode.EQUALS, String(!showHidden));
  }

  // Additional Filters
  if (hasValidAdditionalProps(additionalFilter)) {
    const { field, values, matchMode } = additionalFilter;
    if (!isEmptyObject(values)) {
      addOrUpdateValueForField(transformedFilters, field, matchMode, JSON.stringify(values));
    }
  }

  return transformedFilters;
}

export const useSetQueryFilter = (
  id: string,
  columns: ColumnMeta[],
  first: number,
  filters: DataTableFilterMeta,
  page: number,
  rows: number,
  streamGroupId?: number
) => {
  const { sortInfo } = useSortInfo(id);
  const { queryAdditionalFilter } = useQueryAdditionalFilters(id);
  const { showHidden } = useShowHidden(id);
  const { queryFilter, setQueryFilter } = useQueryFilter(id);

  const { generateGetApi } = useMemo(() => {
    const sortString = getSortString(sortInfo);
    const transformedFilters = transformAndEnhanceFilters(filters, columns, showHidden, queryAdditionalFilter);

    const jsonFiltersString = JSON.stringify(transformedFilters);

    const apiState = {
      jsonFiltersString,
      orderBy: sortString,
      pageNumber: page,
      pageSize: rows,
      streamGroupId
    };

    return {
      generateGetApi: apiState
    };
  }, [sortInfo, filters, columns, showHidden, queryAdditionalFilter, page, rows, streamGroupId]);

  useEffect(() => {
    if (!areGetApiArgsEqual(generateGetApi, queryFilter)) {
      setQueryFilter(generateGetApi);
    }
  }, [generateGetApi, queryFilter, setQueryFilter]);

  return { queryFilter: generateGetApi };
};
