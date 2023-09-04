import { type DataTableExpandedRows } from 'primereact/datatable';
import { type DataTableFilterMeta } from 'primereact/datatable';
import { type DataTableValue } from 'primereact/datatable';
import { useLocalStorage } from 'primereact/hooks';
import { useState } from 'react';
import { type PagedTableInformation } from './DataSelector';
import { type AdditionalFilterProps } from '../../common/common';
import { type VideoStreamIsReadOnly } from '../../store/iptvApi';
import { useSortInfo } from '../../app/slices/useSortInfo';
import { useSelectAll } from '../../app/slices/useSelectAll';
import { useShowHidden } from '../../app/slices/useShowHidden';

const useDataSelectorState = <T extends DataTableValue,>(id: string) => {
  const { sortInfo, setSortInfo } = useSortInfo(id);
  const { selectAll, setSelectAll } = useSelectAll(id);
  const { showHidden } = useShowHidden(id);

  const [rowClick, setRowClick] = useLocalStorage<boolean>(false, id + '-rowClick');
  const [selections, setSelections] = useState<T[]>([] as T[]);
  const [pagedInformation, setPagedInformation] = useState<PagedTableInformation>();
  const [prevDataSource, setPrevDataSource] = useState<T[] | undefined>(undefined);
  const [dataSource, setDataSource] = useState<T[]>();
  const [first, setFirst] = useState<number>(0);
  const [page, setPage] = useState<number>(1);
  const [additionalFilterProps, setAdditionalFilterProps] = useState<AdditionalFilterProps | undefined>(undefined);

  const [rows, setRows] = useState<number>(25);
  const [filters, setFilters] = useState<DataTableFilterMeta>({});
  const [expandedRows, setExpandedRows] = useState<DataTableExpandedRows>();
  const [videoStreamIsReadOnlys, setVideoStreamIsReadOnlys] = useState<VideoStreamIsReadOnly[]>([]);

  const setSortField = (value: string) => {
    setSortInfo({ sortField: value })
  }

  const setSortOrder = (value: -1 | 0 | 1) => {
    setSortInfo({ sortOrder: value })
  }

  const sortField = sortInfo?.sortField ?? '';
  const sortOrder = sortInfo?.sortOrder ?? 1;

  return {
    setters: {
      setAdditionalFilterProps,
      setDataSource,
      setExpandedRows,
      setFilters,
      setFirst,
      setPage,
      setPagedInformation,
      setPrevDataSource,
      setRowClick,
      setRows,
      setSelectAll,
      setSelections,
      setSortField,
      setSortOrder,
      setVideoStreamIsReadOnlys
    },
    state: {
      additionalFilterProps,
      dataSource,
      expandedRows,
      filters,
      first,
      page,
      pagedInformation,
      prevDataSource,
      rowClick,
      rows,
      selectAll,
      selections,
      showHidden,
      sortField,
      sortOrder,
      videoStreamIsReadOnlys
    }
  };
};

export default useDataSelectorState;
