import { areGetApiArgsEqual } from '@lib/apiDefs';
import { SMDataTableFilterMetaData, addOrUpdateValueForField, hasValidAdditionalProps, isEmptyObject } from '@lib/common/common';
import { useQueryAdditionalFilters } from '@lib/redux/hooks/queryAdditionalFilters';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useShowHidden } from '@lib/redux/hooks/showHidden';
import { useSortInfo } from '@lib/redux/hooks/sortInfo';
import { ChannelGroupDto, EPGFileDto, QueryStringParameters } from '@lib/smAPI/smapiTypes';
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
      } else if (column.field === 'Group') {
        let ids = filter.value;
        if (filter.value.length > 0 && filter.value[0].hasOwnProperty('Name')) {
          ids = (filter.value as ChannelGroupDto[]).map((x) => x.Name);
        }
        if (ids.length > 0) {
          transformedFilters.push({
            fieldName: column.field,
            matchMode: FilterMatchMode.CONTAINS,
            value: ids
          });
        }
      } else if (column.field === 'M3UFileName') {
        let ids = filter.value;
        if (filter.value.length > 0 && filter.value[0].hasOwnProperty('Name')) {
          ids = (filter.value as ChannelGroupDto[]).map((x) => x.Name);
        }
        if (ids.length > 0) {
          transformedFilters.push({
            fieldName: column.field,
            matchMode: FilterMatchMode.CONTAINS,
            value: ids
          });
        }
      } else {
        transformedFilters.push({
          fieldName: column.field,
          matchMode: FilterMatchMode.CONTAINS,
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
      if (field === 'Group' && values.length > 0) {
        addOrUpdateValueForField(transformedFilters, field, FilterMatchMode.CONTAINS, JSON.stringify(values));
      }
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
  const { queryAdditionalFilters } = useQueryAdditionalFilters(id);
  const { showHidden } = useShowHidden(id);
  const { queryFilter, setQueryFilter } = useQueryFilter(id);

  const { generateGetApi } = useMemo(() => {
    const sortString = getSortString(sortInfo);
    const transformedFilters = transformAndEnhanceFilters(filters, columns, showHidden, queryAdditionalFilters);

    const JSONFiltersString = JSON.stringify(transformedFilters);

    const apiState: QueryStringParameters = {
      JSONFiltersString,
      OrderBy: sortString,
      PageNumber: page,
      PageSize: rows
    };

    return {
      generateGetApi: apiState
    };
  }, [sortInfo, queryAdditionalFilters, filters, columns, showHidden, page, rows]);

  useEffect(() => {
    if (!areGetApiArgsEqual(generateGetApi, queryFilter)) {
      setQueryFilter(generateGetApi);
    }
  }, [generateGetApi, queryFilter, setQueryFilter]);

  return { queryFilter: generateGetApi };
};
