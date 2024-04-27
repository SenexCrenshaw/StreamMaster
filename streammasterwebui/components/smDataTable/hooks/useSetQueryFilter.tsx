import { SMDataTableFilterMetaData, addOrUpdateValueForField, hasValidAdditionalProps, isEmptyObject } from '@lib/common/common';

import { GetApiArgument, areGetApiArgsEqual } from '@lib/apiDefs';
import { useQueryAdditionalFilters } from '@lib/redux/slices/useQueryAdditionalFilters';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useShowHidden } from '@lib/redux/slices/useShowHidden';
import { useSortInfo } from '@lib/redux/slices/useSortInfo';
import { EPGFileDto } from '@lib/smAPI/smapiTypes';
import { FilterMatchMode } from 'primereact/api';
import { DataTableFilterMeta } from 'primereact/datatable';
import { useEffect, useMemo } from 'react';
import { ColumnMeta } from '../types/ColumnMeta';

function getSortString(sortInfo: any): string {
  return sortInfo && sortInfo.sortField ? `${sortInfo.sortField} ${sortInfo.sortOrder === -1 ? 'desc' : 'asc'}` : '';
}

function transformAndEnhanceFilters(
  filters: DataTableFilterMeta,
  columns: ColumnMeta[],
  showHidden: boolean | null | undefined,
  additionalFilter: any
): SMDataTableFilterMetaData[] {
  let transformedFilters: SMDataTableFilterMetaData[] = [];

  columns.forEach((column) => {
    const filter = filters[column.field] as SMDataTableFilterMetaData;

    // Check if the filter has a value and is not an empty array
    if (filter?.value && filter.value !== '[]') {
      if (column.field === 'EPGId') {
        const epgIds = (filter.value as EPGFileDto[]).filter((x) => x.EPGNumber !== -99).map((x) => x.EPGNumber.toString() + '-');

        if (epgIds.length > 0) {
          transformedFilters.push({
            fieldName: column.field,
            matchMode: FilterMatchMode.STARTS_WITH,
            value: epgIds
          });
        }

        const found = (filter.value as EPGFileDto[]).some((x) => x.EPGNumber === -99);
        if (found) {
          transformedFilters.push({
            fieldName: column.field,
            matchMode: FilterMatchMode.NOT_CONTAINS,
            value: '-'
          });
        }
      } else {
        transformedFilters.push({
          fieldName: column.field,
          matchMode: filter.matchMode,
          value: filter.value
        });
      }
    }
  });

  // Show/Hide logic
  if (showHidden !== null && showHidden !== undefined) {
    addOrUpdateValueForField(transformedFilters, 'IsHidden', FilterMatchMode.EQUALS, String(!showHidden));
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
  StreamGroupId?: number
) => {
  const { sortInfo } = useSortInfo(id);
  const { queryAdditionalFilter } = useQueryAdditionalFilters(id);
  const { showHidden } = useShowHidden(id);
  const { queryFilter, setQueryFilter } = useQueryFilter(id);

  const { generateGetApi } = useMemo(() => {
    const sortString = getSortString(sortInfo);
    if (columns.some((searchElement) => searchElement.field === 'EPGId')) {
      console.log(id);
    }
    const transformedFilters = transformAndEnhanceFilters(filters, columns, showHidden, queryAdditionalFilter);

    const JSONFiltersString = JSON.stringify(transformedFilters);

    const apiState: GetApiArgument = {
      JSONFiltersString,
      OrderBy: sortString,
      PageNumber: page,
      PageSize: rows,
      StreamGroupId
    };

    return {
      generateGetApi: apiState
    };
  }, [sortInfo, columns, filters, showHidden, queryAdditionalFilter, page, rows, StreamGroupId, id]);

  useEffect(() => {
    if (!areGetApiArgsEqual(generateGetApi, queryFilter)) {
      setQueryFilter(generateGetApi);
    }
  }, [generateGetApi, queryFilter, setQueryFilter]);

  return { queryFilter: generateGetApi };
};
