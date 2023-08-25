import { type DataTableExpandedRows } from 'primereact/datatable';
import { type DataTableFilterMeta } from 'primereact/datatable';
import { type DataTableValue } from 'primereact/datatable';
import { useLocalStorage } from 'primereact/hooks';
import { useState } from 'react';
import { type PagedTableInformation } from './DataSelector';
import { type AdditionalFilterProps } from '../../common/common';
import { type VideoStreamIsReadOnly } from '../../store/iptvApi';

const useDataSelectorState = <T extends DataTableValue,>(id: string) => {
  const [selectAll, setSelectAll] = useState<boolean>(false);
  const [rowClick, setRowClick] = useLocalStorage<boolean>(false, id + '-rowClick');
  const [selections, setSelections] = useState<T[]>([] as T[]);
  const [pagedInformation, setPagedInformation] = useState<PagedTableInformation>();
  const [prevDataSource, setPrevDataSource] = useState<T[] | undefined>(undefined);
  const [dataSource, setDataSource] = useState<T[]>();
  const [sortOrder, setSortOrder] = useState<-1 | 0 | 1>(1);
  const [sortField, setSortField] = useState<string>('');
  const [first, setFirst] = useState<number>(0);
  const [page, setPage] = useState<number>(1);
  const [additionalFilterProps, setAdditionalFilterProps] = useState<AdditionalFilterProps | undefined>(undefined);

  const [rows, setRows] = useState<number>(25);
  const [filters, setFilters] = useLocalStorage<DataTableFilterMeta>({}, id + '-tempfilters');
  const [expandedRows, setExpandedRows] = useState<DataTableExpandedRows>();
  const [videoStreamIsReadOnlys, setVideoStreamIsReadOnlys] = useState<VideoStreamIsReadOnly[]>([]);

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
      sortField,
      sortOrder,
      videoStreamIsReadOnlys
    }
  };
};

export default useDataSelectorState;
