import './DataSelector.css';

import { Button } from 'primereact/button';
import { Column } from 'primereact/column';

import { type DataTableSelectionMultipleChangeEvent } from 'primereact/datatable';
import { type DataTableFilterMeta } from 'primereact/datatable';
import { type DataTableSelectionSingleChangeEvent } from 'primereact/datatable';
import { type DataTableSelectAllChangeEvent } from 'primereact/datatable';
import { type DataTableStateEvent } from 'primereact/datatable';
import { type DataTablePageEvent } from 'primereact/datatable';
import { type DataTableExpandedRows } from 'primereact/datatable';
import { type DataTableRowToggleEvent } from 'primereact/datatable';
import { type DataTableValue } from 'primereact/datatable';
import { type DataTableRowData } from 'primereact/datatable';
import { DataTable } from 'primereact/datatable';
import { type ReactNode } from 'react';
import { memo, useCallback, useEffect, useMemo, useRef, type CSSProperties } from 'react';
import { removeValueForField } from '../../common/common';
import { areAdditionalFilterPropsEqual, type MatchMode } from '../../common/common';
import { addOrUpdateValueForField, type SMDataTableFilterMetaData } from '../../common/common';
import { type GetApiArg } from '../../common/common';
import { type QueryHook } from '../../common/common';
import { camel2title, getTopToolOptions, isEmptyObject } from '../../common/common';
import StreamMasterSetting from '../../store/signlar/StreamMasterSetting';
import { type LazyTableState } from './DataSelectorTypes';
import { type ColumnAlign, type ColumnFieldType, type DataSelectorSelectionMode } from './DataSelectorTypes';
import { type ColumnMeta } from './DataSelectorTypes';
import TableHeader from './TableHeader';
import bodyTemplate from './bodyTemplate';
import isPagedTableDto from './isPagedTableDto';
import generateFilterData from './generateFilterData';
import getEmptyFilter from './getEmptyFilter';
import getHeader from './getHeader';
import useDataSelectorState from './useDataSelectorState';
import getRecord from './getRecord';
import getRecordString from './getRecordString';
import { type PagedResponseDto } from '../selectors/BaseSelector';
import { areArraysEqual } from '@mui/base';
import { useQueryAdditionalFilters } from '../../app/slices/useQueryAdditionalFilters';
import BanButton from '../buttons/BanButton';
import ResetButton from '../buttons/ResetButton';

const DataSelector = <T extends DataTableValue,>(props: DataSelectorProps<T>) => {
  const { state, setters } = useDataSelectorState<T>(props.id);
  const { queryAdditionalFilter } = useQueryAdditionalFilters(props.id);

  const tableRef = useRef<DataTable<T[]>>(null);

  const setting = StreamMasterSetting();

  useEffect(() => {
    if (props.streamToRemove) {

      if (state.selections.findIndex(e => e.id === props.streamToRemove) !== -1) {
        setters.setSelections(state.selections.filter(e => e.id !== props.streamToRemove));
      }

    }
  }, [props.streamToRemove, setters, state.selections]);

  const lazyState = (filters: DataTableFilterMeta): LazyTableState => {

    const newFilters = generateFilterData(props.columns, filters, props.showHidden);

    let sort = '';
    if (state.sortField) {
      sort = (state.sortOrder === -1) ? `${state.sortField} desc` : (state.sortOrder === 1) ? `${state.sortField} asc` : '';
    }

    const defaultState: LazyTableState = {
      filters: newFilters,
      first: state.first,
      jsonFiltersString: '',
      page: state.page,
      rows: state.rows,
      sortField: state.sortField,
      sortOrder: state.sortOrder,
      sortString: sort
    };

    return {
      ...defaultState,
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  };

  const filterData = useMemo((): GetApiArg => {
    // Helper functions
    const hasValidAdditionalProps = () => {
      return state.additionalFilterProps?.values;
    };

    const generateFilteredData = () => {
      const toSend: SMDataTableFilterMetaData[] = Object.keys(lazyState(state.filters).filters)
        .map(key => {
          const value = lazyState(state.filters).filters[key] as SMDataTableFilterMetaData;
          return value?.value && value.value !== '[]' ? value : null;
        })
        .filter(Boolean) as SMDataTableFilterMetaData[];

      if (hasValidAdditionalProps()) {
        const addProps = state.additionalFilterProps;
        if (addProps) {
          if (isEmptyObject(addProps.values)) {
            removeValueForField(toSend, addProps.field);
          } else {
            const values = JSON.stringify(addProps.values);
            addOrUpdateValueForField(toSend, addProps.field, addProps.matchMode as MatchMode, values);
          }
        }
      }

      const toFilter = lazyState(state.filters);
      return {
        jsonFiltersString: JSON.stringify(toSend),
        orderBy: toFilter.sortString || props.defaultSortField,
        pageNumber: toFilter.page,
        pageSize: toFilter.rows,
      };
    };

    // Main Logic
    if (isEmptyObject(state.filters) && !hasValidAdditionalProps()) {
      return { pageSize: 40 };
    }

    const getApi = generateFilteredData();
    return getApi || { pageSize: 40 };

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [state.filters, state.additionalFilterProps, state.sortField, state.sortOrder, state.page, state.rows]);

  const { data, isLoading, isFetching } = props.queryFilter ? props.queryFilter(filterData) : { data: undefined, isFetching: false, isLoading: false };

  useEffect(() => {
    if (queryAdditionalFilter?.values) {

      if (!areAdditionalFilterPropsEqual(queryAdditionalFilter, state.additionalFilterProps)) {
        setters.setAdditionalFilterProps(queryAdditionalFilter);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [queryAdditionalFilter]);


  const onsetSelection = useCallback((e: T | T[], overRideSelectAll?: boolean): T | T[] | undefined => {
    let selected: T[] = Array.isArray(e) ? e : [e];

    if (state.selections === selected) {
      return;
    }

    if (props.selectionMode === 'single') {
      selected = selected.slice(0, 1);
    }

    setters.setSelections(selected);
    const all = overRideSelectAll ? overRideSelectAll : state.selectAll;
    if (props.onSelectionChange) {
      props.onSelectionChange(props.selectionMode === 'single' ? selected[0] : selected, all, all ? state.pagedInformation?.totalRecords ?? undefined : undefined);
    }

    return e;
  }, [state.selections, state.selectAll, state.pagedInformation?.totalRecords, props, setters]);


  useEffect(() => {
    if (!props.dataSource && !data) {
      return;
    }

    if (props.dataSource) {
      if (!state.dataSource || (state.dataSource && !areArraysEqual(props.dataSource, state.dataSource))) {

        if (!props.reorderable) {
          setters.setDataSource(props.dataSource);
        } else {
          setters.setDataSource([...props.dataSource].sort((a, b) => a.rank - b.rank));
        }

        setters.setPagedInformation(undefined);
        if (state.selectAll) {
          onsetSelection(props.dataSource);
        }
      }

      return;
    }

    if (!data) {
      return;
    }

    if (Array.isArray(data)) {
      if (!state.dataSource || (state.dataSource && !areArraysEqual(data, state.dataSource))) {
        setters.setDataSource(data);
        setters.setPagedInformation(undefined);
        if (state.selectAll) {
          setters.setSelections(data);
        }
      }

      return;
    }

    if (data && isPagedTableDto<T>(data)) {
      if (!state.dataSource || (state.dataSource && !areArraysEqual(data.data, state.dataSource))) {
        setters.setDataSource((data as PagedResponseDto<T>).data);
        if (state.selectAll && data !== undefined) {
          setters.setSelections((data as PagedResponseDto<T>).data as T[]);
        }
      }

      if (data.pageNumber > 1 && data.totalPageCount == 0) {
        const newData = { ...data };
        newData.pageNumber += -1;
        newData.first = (newData.pageNumber - 1) * newData.pageSize;
        setters.setPage(newData.pageNumber);
        setters.setFirst(newData.first);
        setters.setPagedInformation(newData);
      } else {
        setters.setPagedInformation(data);
      }


      return;
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [data, props.dataSource, state.selectAll]);

  const onRowReorder = (changed: T[]) => {
    setters.setDataSource(changed);
    if (state.prevDataSource === undefined) {
      setters.setPrevDataSource(changed);
    }

    props.onRowReorder?.(changed);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }


  const rowClass = useCallback((changed: DataTableRowData<T[]>) => {
    const isHidden = getRecord(changed as T, 'isHidden');
    if (isHidden === true) {
      return `bg-red-900`;
    }

    if (props.videoStreamIdsIsReadOnly !== undefined && props.videoStreamIdsIsReadOnly.length > 0) {
      const isReadOnly = props.videoStreamIdsIsReadOnly.find((vs) => vs === getRecord(changed as T, 'id'));
      if (isReadOnly !== undefined) {
        return 'videostreamSelected' // `bg-yellow-900`;
      }
    }

    return {};
  }, [props.videoStreamIdsIsReadOnly]);

  const exportCSV = () => {
    tableRef.current?.exportCSV({ selectionOnly: false });
  };

  const sourceRenderHeader = useMemo(() => {
    if (!props.headerLeftTemplate && !props.headerRightTemplate) {
      return null;
    }

    return (<TableHeader
      dataSelectorProps={props}
      enableExport={props.enableExport ?? false}
      exportCSV={exportCSV}
      headerName={props.headerName}
      onMultiSelectClick={props.onMultiSelectClick}
      rowClick={state.rowClick}
      setRowClick={setters.setRowClick}
    />);
  }, [props, state.rowClick, setters.setRowClick]);

  const getSelectionMultipleMode = useMemo((): 'checkbox' | 'multiple' | null => {
    if (props.selectionMode === 'multiple') {
      return 'checkbox';
    }

    return null;
  }, [props.selectionMode]);

  const onSelectionChange = useCallback((e: DataTableSelectionMultipleChangeEvent<T[]> | DataTableSelectionSingleChangeEvent<T[]>) => {

    if (e.value === null || e.value === undefined) {
      return;
    }

    let sel = [] as T[];

    if (e.value instanceof Array) {
      const single1 = e.value.slice(e.value.length - 1, e.value.length);
      sel = single1;
    } else {
      sel = [e.value];
    }

    if (props.reorderable === true) {
      if (props.onSelectionChange) {
        props.onSelectionChange(sel, false, undefined);
      }
    }

    if (props.selectionMode === 'multiple') {
      if (e.value instanceof Array) {
        onsetSelection(e.value);
      } else {
        onsetSelection([e.value]);
      }

      return;
    }

    onsetSelection(sel);

  }, [onsetSelection, props]);

  const getAlign = useCallback((align: ColumnAlign | null | undefined, fieldType: ColumnFieldType): ColumnAlign => {

    if (fieldType === 'image') {
      return 'center'
    }

    if (fieldType === 'isHidden') {
      return 'center'
    }

    if (align === undefined || align === null) {
      return 'left'
    }

    return align;

  }, []);

  const getAlignHeader = useCallback((align: ColumnAlign | undefined, fieldType: ColumnFieldType): ColumnAlign => {
    if (fieldType === 'image') {
      return 'center'
    }

    if (fieldType === 'isHidden') {
      return 'center'
    }

    if (!align) {
      return 'center'
    }

    return align;
  }, []);

  const getFilter = useCallback((filter: boolean | undefined, fieldType: ColumnFieldType): boolean | undefined => {
    if (fieldType === 'image') {
      return false;
    }

    return filter;
  }, [])

  const getStyle = useCallback((style: CSSProperties | undefined, fieldType: ColumnFieldType | undefined): CSSProperties | undefined => {

    if (fieldType === 'blank') {
      return {
        maxWidth: '1rem',
        width: '1rem',
      } as CSSProperties;
    }

    if (fieldType === 'image' || fieldType === 'm3ulink' || fieldType === 'epglink' || fieldType === 'url') {
      return {
        maxWidth: '5rem',
        width: '5rem',
      } as CSSProperties;
    }

    return {
      ...style,
      flexGrow: 0,
      flexShrink: 1,
      // maxWidth: '10rem',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap',

    } as CSSProperties;
  }, []);

  const rowGroupHeaderTemplate = useCallback((row: T) => {
    if (!props.groupRowsBy) {
      return (
        <div />
      );
    }

    const record = getRecordString(row, props.groupRowsBy);
    return (
      <span className="vertical-align-middle ml-2 font-bold line-height-3">
        {record}
        <Button
          className={row.isHidden ? 'p-button-danger' : ''}
          icon={`pi pi-eye ${row.isHidden ? 'text-red-500' : 'text-green-500'}`}
          onClick={() => props?.onRowVisibleClick?.(row)}
          rounded
          text
          tooltip="Set Hidden"
          tooltipOptions={getTopToolOptions}
        />
      </span>
    );
  }, [props]);

  const multiselectHeader = () => {
    return (
      <div className=" text-xs text-white text-500" >
        <BanButton
          disabled={state.selections.length === 0}
          onClick={() => {
            setters.setSelections([]);
            setters.setSelectAll(false);
            if (props.onSelectionChange) {
              props.onSelectionChange([], false, undefined);
            }
          }}

          tooltip='Clear Selections'
        />

      </div>
    );
  }

  const rowReorderHeader = () => {
    return (
      <div className=" text-xs text-white text-500" >
        <ResetButton
          disabled={state.prevDataSource === undefined}
          onClick={() => {
            if (state.prevDataSource !== undefined) {
              setters.setDataSource(state.prevDataSource);
              setters.setPrevDataSource(undefined);
            }
          }}
          tooltip='Reset Order'
        />

      </div>
    );
  }

  const onSort = (event: DataTableStateEvent) => {
    if (!event.sortField || event.sortField === 'selected') {
      return;
    }

    const sortOrder = [1, 0, -1].includes(event.sortOrder ?? 1) ? event.sortOrder : 0;
    setters.setSortOrder(sortOrder ?? 1);

    // Try finding the column by field directly.
    let matchingColumn = props.columns.find(column => column.field === event.sortField);

    // If not found, try finding by header, case-insensitively.
    if (!matchingColumn) {
      matchingColumn = props.columns.find(column =>
        column.header?.toLocaleLowerCase() === event.sortField.toLocaleLowerCase()
      );
    }

    // Set the sort field based on the matched column, or default to an empty string.
    const sort = matchingColumn?.field ?? '';
    setters.setSortField(sort);
  };

  const onPage = (event: DataTablePageEvent) => {
    const adjustedPage = (event.page ?? 0) + 1;
    setters.setPage(adjustedPage);
    setters.setFirst(event.first);
    setters.setRows(event.rows);
    if (state.prevDataSource !== undefined) {
      setters.setPrevDataSource(undefined);
    }
  };

  const onFilter = (event: DataTableStateEvent) => {
    const newFilters = generateFilterData(props.columns, event.filters, props.showHidden);
    setters.setFilters(newFilters);
  }

  const onSelectAllChange = (event: DataTableSelectAllChangeEvent) => {
    const newSelectAll = event.checked;
    setters.setSelectAll(newSelectAll);

    // props.onSelectAllChange?.(newSelectAll);

    if (newSelectAll && state.dataSource) {
      onsetSelection(state.dataSource, true);
    } else {
      onsetSelection([]);
    }
  };

  return (

    <div className='dataselector flex w-full min-w-full  justify-content-start align-items-center' >
      <div className={`${props.className !== undefined ? props.className : ''} min-h-full w-full surface-overlay`}>
        <DataTable
          cellSelection={false}
          dataKey='id' // {props.key !== undefined && props.key !== '' ? props.key : 'id'}
          editMode='cell'
          emptyMessage={props.emptyMessage}
          expandableRowGroups={props.groupRowsBy !== undefined && props.groupRowsBy !== ''}
          expandedRows={state.expandedRows}
          exportFilename={props.exportFilename ?? 'streammaster'}
          filterDelay={500}
          filterDisplay="row"
          filters={isEmptyObject(state.filters) ? getEmptyFilter(props.columns, props.showHidden) : state.filters}
          first={state.pagedInformation ? state.pagedInformation.first : state.first}
          header={sourceRenderHeader}
          key='id' // {props.key !== undefined && props.key !== '' ? props.key : 'id'}
          lazy
          loading={isLoading === true || isFetching === true || props.isLoading === true}
          metaKeySelection={false}
          onFilter={onFilter}
          onPage={onPage}
          onRowReorder={(e) => {
            onRowReorder(e.value)
          }}
          onRowToggle={(e: DataTableRowToggleEvent) => setters.setExpandedRows(e.data as DataTableExpandedRows)}
          onSelectAllChange={props.reorderable ? undefined : onSelectAllChange}
          onSelectionChange={((e) => props.reorderable ? undefined : onSelectionChange(e))}
          onSort={onSort}
          // onValueChange={(e) => { onValueChanged(e); }}
          paginator
          paginatorClassName='text-xs p-0 m-0 withpadding'
          paginatorTemplate="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
          ref={tableRef}
          removableSort={false}
          reorderableRows={props.reorderable}
          resizableColumns
          rowClassName={rowClass}
          rowGroupHeaderTemplate={rowGroupHeaderTemplate}
          rowGroupMode={props.groupRowsBy !== undefined && props.groupRowsBy !== '' ? 'subheader' : undefined}
          rows={state.rows}
          rowsPerPageOptions={[25, 50, 100, 250]}
          scrollHeight={props.enableVirtualScroll === true ? props.virtualScrollHeight !== undefined ? props.virtualScrollHeight : '400px' : 'flex'}
          scrollable
          selectAll={state.selectAll}
          selection={state.selections}
          selectionMode={getSelectionMultipleMode}
          showGridlines
          showHeaders={props.showHeaders}
          sortField={props.reorderable ? 'rank' : state.sortField}
          sortMode='single'
          sortOrder={props.reorderable ? 0 : state.sortOrder}
          stateKey={`${props.id}-table`}
          stateStorage="local"
          stripedRows
          style={props.style}
          totalRecords={state.pagedInformation ? state.pagedInformation.totalRecords : undefined}
          value={state.dataSource}
          virtualScrollerOptions={props.enableVirtualScroll === true ? { itemSize: 16, orientation: 'vertical' } : undefined}
        >
          {/* <Column
            body={<i className="pi pi-chevron-right" />}
            className='max-w-1rem p-0 justify-content-center align-items-center'
            field='selector'
            header=""
            hidden={!props.showSelector}
            style={{ width: '1rem' }}
          /> */}
          <Column
            className='max-w-2rem p-0 justify-content-center align-items-center'
            field='rank'
            header={rowReorderHeader}
            hidden={!props.reorderable}
            rowReorder
            style={{ width: '2rem' }}
          />
          <Column
            align='center'
            alignHeader='center'
            className={`justify-content-center align-items-center multiselect ${props.selectionMode}`}
            header={multiselectHeader}
            headerStyle={{ padding: '0px', width: '3rem' }}
            hidden={props.selectionMode !== 'multiple' && props.selectionMode !== 'checkbox' && props.selectionMode !== 'multipleNoRowCheckBox'}
            resizeable={false}
            selectionMode="multiple"
          />
          {props.columns.map((col) => (

            <Column
              align={getAlign(col.align, col.fieldType)}
              alignHeader={getAlignHeader(col.align, col.fieldType)}
              body={((e) => col.bodyTemplate ? col.bodyTemplate(e) : bodyTemplate(e, col.field, col.fieldType, setting.defaultIcon, col.camelize))}
              editor={col.editor}
              field={col.field}
              filter={getFilter(col.filter, col.fieldType)}
              filterElement={col.filterElement}
              filterMenuStyle={{ width: '14rem' }}
              filterPlaceholder={col.fieldType === 'epg' ? 'EPG' : col.header ? col.header : camel2title(col.field)}
              header={getHeader(col.field, col.header, col.fieldType)}
              hidden={col.isHidden === true || (props.hideControls === true && getHeader(col.field, col.header, col.fieldType) === 'Actions') ? true : undefined}
              key={!col.fieldType ? col.field : col.field + col.fieldType}
              onCellEditComplete={col.handleOnCellEditComplete}
              resizeable={col.resizeable}
              showAddButton
              showApplyButton
              showClearButton
              showFilterMatchModes
              showFilterMenu={col.filterElement === undefined}
              showFilterMenuOptions
              showFilterOperator
              sortable={props.reorderable ? false : col.sortable}
              style={getStyle(col.style, col.fieldType)}
            />

          ))}


        </DataTable>
      </div>
    </div >

  );
};

DataSelector.displayName = 'dataselector';
DataSelector.defaultProps = {
  defaultSortField: 'name',
  enableVirtualScroll: false,
  headerName: '',
  hideControls: false,
  onSelectionChange: undefined,
  reorderable: false,
  selectionMode: 'single',
  showHeaders: true,
  showHidden: null
};


// eslint-disable-next-line @typescript-eslint/no-explicit-any
type BaseDataSelectorProps<T = any> = {
  className?: string;
  columns: ColumnMeta[];
  defaultSortField?: string;
  emptyMessage?: ReactNode;
  enableExport?: boolean;
  enableVirtualScroll?: boolean | undefined;
  exportFilename?: string;
  groupRowsBy?: string;
  headerLeftTemplate?: ReactNode;
  headerName?: string;
  headerRightTemplate?: ReactNode;
  hideControls?: boolean;
  id: string;
  isLoading?: boolean;
  key?: string | undefined;
  onMultiSelectClick?: (value: boolean) => void;
  onRowReorder?: (value: T[]) => void;
  onRowVisibleClick?: (value: T) => void;
  onSelectionChange?: (value: T | T[], selectAll: boolean, totalSelected: number | undefined) => void;
  // onValueChanged?: (value: T[]) => void;
  reorderable?: boolean;
  selectionMode?: DataSelectorSelectionMode;
  showHeaders?: boolean | undefined;
  showHidden?: boolean | null | undefined;
  showSelector?: boolean;
  streamToRemove?: string;
  style?: CSSProperties;
  videoStreamIdsIsReadOnly?: string[] | undefined;
  virtualScrollHeight?: string | undefined;
}

type QueryFilterProps<T> = BaseDataSelectorProps<T> & {
  dataSource?: never;
  queryFilter: (filters: GetApiArg) => ReturnType<QueryHook<PagedResponseDto<T> | T[]>>;
};

type DataSourceProps<T> = BaseDataSelectorProps<T> & {
  dataSource: T[] | undefined;
  queryFilter?: never;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type DataSelectorProps<T = any> = DataSourceProps<T> | QueryFilterProps<T>;

export type PagedTableInformation = {
  first: number;
  pageNumber: number;
  pageSize: number;
  totalItemCount: number;
  totalPageCount: number;
  totalRecords: number;
};

export type PagedDataDto<T> = {
  data?: T[];
};

export type PagedTableDto<T> = PagedDataDto<T> & PagedTableInformation & {
};


export default memo(DataSelector);
