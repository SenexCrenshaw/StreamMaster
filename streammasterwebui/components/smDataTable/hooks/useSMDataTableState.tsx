import { type DataTableExpandedRows, type DataTableValue } from 'primereact/datatable';
import { useLocalStorage } from 'primereact/hooks';
import { useState } from 'react';

import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useShowHidden } from '@lib/redux/hooks/showHidden';
import { useSortInfo } from '@lib/redux/hooks/sortInfo';

import { useFilters } from '@lib/redux/hooks/filters';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { useShowSelected } from '@lib/redux/hooks/showSelected';
import { useShowSelections } from '@lib/redux/hooks/showSelections';
import { PagedResponse } from '@lib/smAPI/smapiTypes';
import { ColumnMeta } from '../types/ColumnMeta';

const SMDataTableState = <T extends DataTableValue>(id: string, selectedItemsKey?: string) => {
  const { sortInfo, setSortInfo: hookSetSortInfo } = useSortInfo(id);
  const { showHidden } = useShowHidden(id);
  const { showSelected } = useShowSelected(id);
  const { showSelections, setShowSelections } = useShowSelections(id);
  const { isTrue: smTableIsSimple, setIsTrue: setSMTableIsSimple } = useIsTrue(id);
  const { selectAll, setSelectAll } = useSelectAll(id);
  const { selectedItems, setSelectedItems } = useSelectedItems<T>(selectedItemsKey ?? id);
  const [rowClick, setRowClick] = useLocalStorage<boolean>(false, `${id}-rowClick`);
  const [visibleColumns, setVisibleColumns] = useLocalStorage<ColumnMeta[] | undefined | null>(null, `${id}-visibleColumns`);
  const { filters, setFilters } = useFilters(id);

  const [pagedInformation, setPagedInformation] = useState<PagedResponse<T>>();
  // const [dataSource, setDataSource] = useState<T[]>();
  const [first, setFirst] = useState<number>(0);
  const [page, setPage] = useState<number>(1);

  const [rows, setRows] = useState<number>(25);

  const [expandedRows, setExpandedRows] = useState<DataTableExpandedRows>();

  const setSortField = (value: string) => {
    const toset = {
      ...sortInfo,
      sortField: value
    };

    hookSetSortInfo(toset);
  };

  const setSortOrder = (value: -1 | 0 | 1) => {
    hookSetSortInfo({ ...sortInfo, sortOrder: value });
  };

  const setSortInfo = (field: string, sortOrder: -1 | 0 | 1) => {
    hookSetSortInfo({ sortField: field, sortOrder });
  };
  const sortField = sortInfo?.sortField ?? '';
  const sortOrder = sortInfo?.sortOrder ?? 1;

  return {
    setters: {
      // setDataSource,
      setExpandedRows,
      setFilters,
      setFirst,
      setPage,
      setPagedInformation,
      setRowClick,
      setRows,
      setSelectAll,
      setSelectedItems,
      setShowSelections,
      setSMTableIsSimple,
      setSortField,
      setSortInfo,
      setSortOrder,
      setVisibleColumns
    },
    state: {
      // dataSource,
      expandedRows,
      filters,
      first,
      page,
      pagedInformation,
      rowClick,
      rows,
      selectAll,
      selectedItems,
      showHidden,
      showSelected,
      showSelections,
      smTableIsSimple,
      sortField,
      sortInfo,
      sortOrder,
      visibleColumns
    }
  };
};

export default SMDataTableState;
