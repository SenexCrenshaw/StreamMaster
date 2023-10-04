import {
  addOrUpdateValueForField,
  areGetApiArgsEqual,
  hasValidAdditionalProps,
  isEmptyObject,
  removeValueForField,
  type MatchMode,
  type SMDataTableFilterMetaData,
} from '@/lib/common/common'
import { FilterMatchMode } from 'primereact/api'
import { type DataTableFilterMeta } from 'primereact/datatable'
import { useEffect, useMemo } from 'react'
import { useQueryAdditionalFilters } from '../../../lib/redux/slices/useQueryAdditionalFilters'
import { useQueryFilter } from '../../../lib/redux/slices/useQueryFilter'
import { useShowHidden } from '../../../lib/redux/slices/useShowHidden'
import { useSortInfo } from '../../../lib/redux/slices/useSortInfo'
import { type ColumnMeta, type LazyTableState } from './DataSelectorTypes'
import generateFilterData from './generateFilterData'

export const useSetQueryFilter = (
  id: string,
  columns: ColumnMeta[],
  first: number,
  filters: DataTableFilterMeta,
  page: number,
  rows: number,
  streamGroupId?: number,
) => {
  const { sortInfo } = useSortInfo(id)
  const { queryAdditionalFilter } = useQueryAdditionalFilters(id)
  const { queryFilter, setQueryFilter } = useQueryFilter(id)
  const { showHidden } = useShowHidden(id)

  const { lazyState, generateGetApi } = useMemo(() => {
    const newFilters = generateFilterData(columns, filters)

    const sort = sortInfo
      ? sortInfo.sortField
        ? sortInfo.sortOrder === -1
          ? `${sortInfo.sortField} desc`
          : `${sortInfo.sortField} asc`
        : ''
      : ''

    const defaultState: LazyTableState = {
      filters: newFilters,
      first: first,
      jsonFiltersString: '',
      page: page,
      rows: rows,
      sortField: 'id',
      sortOrder: 1,
      sortString: sort,
    }

    const toSend: SMDataTableFilterMetaData[] = Object.keys(
      defaultState.filters,
    )
      .map((key) => {
        const value = defaultState.filters[key] as SMDataTableFilterMetaData
        return value?.value && value.value !== '[]' ? value : null
      })
      .filter(Boolean) as SMDataTableFilterMetaData[]

    if (showHidden === null) {
      removeValueForField(toSend, 'isHidden')
    } else if (showHidden !== undefined) {
      addOrUpdateValueForField(
        toSend,
        'isHidden',
        FilterMatchMode.EQUALS,
        !showHidden,
      )
    }

    if (hasValidAdditionalProps(queryAdditionalFilter)) {
      const addProps = queryAdditionalFilter

      if (addProps) {
        if (isEmptyObject(addProps.values)) {
          removeValueForField(toSend, addProps.field)
        } else {
          const values = JSON.stringify(addProps.values)

          addOrUpdateValueForField(
            toSend,
            addProps.field,
            addProps.matchMode as MatchMode,
            values,
          )
        }
      }
    }

    const toFilter = defaultState
    return {
      generateGetApi: {
        jsonFiltersString: JSON.stringify(toSend),
        orderBy: toFilter.sortString,
        pageNumber: toFilter.page,
        pageSize: toFilter.rows,
        streamGroupId: streamGroupId,
      },
      lazyState: defaultState,
    }
  }, [
    columns,
    filters,
    sortInfo,
    first,
    page,
    rows,
    streamGroupId,
    showHidden,
    queryAdditionalFilter,
  ])

  useEffect(() => {
    const newApi = generateGetApi
    if (!areGetApiArgsEqual(newApi, queryFilter)) {
      setQueryFilter(newApi)
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [queryAdditionalFilter, setQueryFilter, generateGetApi, streamGroupId])

  return { lazyState }
}
