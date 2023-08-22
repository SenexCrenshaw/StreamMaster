import { type DataTableExpandedRows } from 'primereact/datatable';
import { type DataTableFilterMeta } from 'primereact/datatable';
import { type DataTableValue } from 'primereact/datatable';
import { useLocalStorage } from 'primereact/hooks';
import { useState } from 'react';
import { type PagedDataDto, type PagedTableInformation } from './DataSelector';



const useDataSelectorState = <T extends DataTableValue,>(id: string) => {
  const [selectAll, setSelectAll] = useState<boolean>(false);
  const [rowClick, setRowClick] = useLocalStorage<boolean>(false, id + '-rowClick');

  const [selections, setSelections] = useState<T[]>([] as T[]);
  const [pagedInformation, setPagedInformation] = useState<PagedTableInformation>();
  const [dataSource, setDataSource] = useState<PagedDataDto<T>>();
  const [sortOrder, setSortOrder] = useState<-1 | 0 | 1>(1);
  const [sortField, setSortField] = useState<string>('');
  const [first, setFirst] = useState<number>(0);
  const [page, setPage] = useState<number>(1);
  const [rows, setRows] = useState<number>(25);
  const [filters, setFilters] = useLocalStorage<DataTableFilterMeta>({}, id + '-tempfilters');
  const [expandedRows, setExpandedRows] = useState<DataTableExpandedRows>();

  return {
    dataSource, expandedRows,
    filters, first,
    page, pagedInformation,
    rowClick, rows,
    selectAll, selections,
    setDataSource, setExpandedRows,
    setFilters, setFirst,
    setPage, setPagedInformation,
    setRowClick, setRows,
    setSelectAll, setSelections,
    setSortField, setSortOrder,
    sortField, sortOrder,
  };
}

export default useDataSelectorState;
