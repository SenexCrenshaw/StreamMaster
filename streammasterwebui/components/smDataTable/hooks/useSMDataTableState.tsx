import { type DataTableExpandedRows, type DataTableFilterMeta, type DataTableValue } from 'primereact/datatable';
import { useLocalStorage } from 'primereact/hooks';
import { useState } from 'react';

import { AdditionalFilterProperties } from '@lib/common/common';

import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { useShowHidden } from '@lib/redux/slices/useShowHidden';
import { useShowSelections } from '@lib/redux/slices/useShowSelections';
import { useSortInfo } from '@lib/redux/slices/useSortInfo';
import { PagedResponse } from '@lib/smAPI/smapiTypes';
import { ColumnMeta } from '../types/ColumnMeta';
// import { PagedTableInformation } from '../types/smDataTableTypes';

const SMDataTableState = <T extends DataTableValue>(id: string, selectedItemsKey?: string) => {
  const { sortInfo, setSortInfo } = useSortInfo(id);
  const { selectAll, setSelectAll } = useSelectAll(id);

  const { showHidden } = useShowHidden(id);
  const { showSelections, setShowSelections } = useShowSelections(id);

  const [rowClick, setRowClick] = useLocalStorage<boolean>(false, `${id}-rowClick`);
  const [visibleColumns, setVisibleColumns] = useLocalStorage<ColumnMeta[] | undefined | null>(null, `${id}-visibleColumns`);
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<T>(selectedItemsKey ?? id);
  const [pagedInformation, setPagedInformation] = useState<PagedResponse<T>>();
  const [previousDataSource, setPreviousDataSource] = useState<T[] | undefined>();
  const [dataSource, setDataSource] = useState<T[]>();
  const [first, setFirst] = useState<number>(0);
  const [page, setPage] = useState<number>(1);
  const [additionalFilterProperties, setAdditionalFilterProperties] = useState<AdditionalFilterProperties | undefined>();

  const [rows, setRows] = useState<number>(25);
  const [filters, setFilters] = useState<DataTableFilterMeta>({});
  const [expandedRows, setExpandedRows] = useState<DataTableExpandedRows>();

  const setSortField = (value: string) => {
    setSortInfo({ sortField: value });
  };

  const setSortOrder = (value: -1 | 0 | 1) => {
    setSortInfo({ sortOrder: value });
  };

  const sortField = sortInfo?.sortField ?? '';
  const sortOrder = sortInfo?.sortOrder ?? 1;

  return {
    setters: {
      // setAdditionalFilterProps: setAdditionalFilterProperties,
      setDataSource,
      setExpandedRows,

      setPagedInformation,
      // setPrevDataSource: setPreviousDataSource,
      setRowClick,
      setSelectSelectedItems,
      setSelectAll,
      setShowSelections,

      setFirst,
      setPage,
      setRows,

      setFilters,
      setSortField,
      setSortOrder,
      setVisibleColumns
    },
    state: {
      // additionalFilterProps: additionalFilterProperties,
      dataSource,
      expandedRows,

      pagedInformation,
      // prevDataSource: previousDataSource,
      rowClick,

      selectAll,

      showSelections,
      first,
      page,
      rows,
      showHidden,
      filters,
      sortField,
      sortOrder,
      visibleColumns,
      selectSelectedItems
    }
  };
};

export default SMDataTableState;