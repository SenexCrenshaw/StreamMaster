import { type DataTableExpandedRows, type DataTableFilterMeta, type DataTableValue } from 'primereact/datatable';
import { useLocalStorage } from 'primereact/hooks';
import { useState } from 'react';

import { AdditionalFilterProperties } from '@lib/common/common';

import { useSelectedSMChannel } from '@lib/redux/slices/selectedSMChannel';
import { useSelectedSMStream } from '@lib/redux/slices/selectedSMStream';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { useShowHidden } from '@lib/redux/slices/useShowHidden';
import { useShowSelections } from '@lib/redux/slices/useShowSelections';
import { useSortInfo } from '@lib/redux/slices/useSortInfo';
import { ColumnMeta, PagedTableInformation } from './DataSelectorTypes';

const useDataSelectorState2 = <T extends DataTableValue>(id: string, selectedItemsKey: string, channelDataSelectorKey: string, selectedSMStreamKey: string) => {
  const { sortInfo, setSortInfo } = useSortInfo(id);
  const { selectAll, setSelectAll } = useSelectAll(id);
  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMChannel(channelDataSelectorKey);
  const { selectedSMStream, setSelectedSMStream } = useSelectedSMStream(selectedSMStreamKey);
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<T>(selectedItemsKey);
  const { showHidden } = useShowHidden(id);
  const { showSelections, setShowSelections } = useShowSelections(id);

  const [rowClick, setRowClick] = useLocalStorage<boolean>(false, `${id}-rowClick`);
  const [visibleColumns, setVisibleColumns] = useLocalStorage<ColumnMeta[] | undefined | null>(null, `${id}-visibleColumns`);

  const [pagedInformation, setPagedInformation] = useState<PagedTableInformation>();
  const [previousDataSource, setPreviousDataSource] = useState<T[] | undefined>();
  const [dataSource, setDataSource] = useState<T[]>();
  const [first, setFirst] = useState<number>(0);
  const [page, setPage] = useState<number>(1);
  const [additionalFilterProperties, setAdditionalFilterProperties] = useState<AdditionalFilterProperties | undefined>();

  const [rows, setRows] = useState<number>(25);
  const [filters, setFilters] = useState<DataTableFilterMeta>({});
  const [expandedRows, setExpandedRows] = useState<DataTableExpandedRows>();
  const [videoStreamIsReadOnlys, setVideoStreamIsReadOnlys] = useState<VideoStreamIsReadOnly[]>([]);

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
      setAdditionalFilterProps: setAdditionalFilterProperties,
      setDataSource,
      setExpandedRows,
      setFilters,
      setFirst,
      setPage,
      setPagedInformation,
      setPrevDataSource: setPreviousDataSource,
      setRowClick,
      setRows,
      setSelectAll,
      setSelectSelectedItems,
      setShowSelections,
      setSortField,
      setSortOrder,
      setVideoStreamIsReadOnlys,
      setVisibleColumns,
      setSelectedSMChannel,
      setSelectedSMStream
    },
    state: {
      additionalFilterProps: additionalFilterProperties,
      dataSource,
      expandedRows,
      filters,
      first,
      page,
      pagedInformation,
      prevDataSource: previousDataSource,
      rowClick,
      rows,
      selectAll,
      selectSelectedItems,
      selectedSMChannel,
      showHidden,
      showSelections,
      sortField,
      sortOrder,
      videoStreamIsReadOnlys,
      visibleColumns,
      selectedSMStream
    }
  };
};

export default useDataSelectorState2;
