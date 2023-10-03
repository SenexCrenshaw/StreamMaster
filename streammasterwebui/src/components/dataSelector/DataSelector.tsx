import './DataSelector.css';

import { areArraysEqual } from '@mui/base';
import { Button } from 'primereact/button';
import { Column } from 'primereact/column';
import { DataTable, type DataTableExpandedRows, type DataTablePageEvent, type DataTableRowClickEvent, type DataTableRowData, type DataTableRowToggleEvent, type DataTableSelectAllChangeEvent, type DataTableSelectionMultipleChangeEvent, type DataTableSelectionSingleChangeEvent, type DataTableStateEvent, type DataTableValue } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useRef, type CSSProperties, type ReactNode } from 'react';
import { type PagedResponseDto } from '../selectors/BaseSelector';
import { type ColumnAlign, type ColumnFieldType, type ColumnMeta, type DataSelectorSelectionMode } from './DataSelectorTypes';
import TableHeader from './TableHeader';
import bodyTemplate from './bodyTemplate';
import generateFilterData from './generateFilterData';
import getEmptyFilter from './getEmptyFilter';
import getHeader from './getHeader';
import getRecord from './getRecord';
import getRecordString from './getRecordString';
import isPagedTableDto from './isPagedTableDto';
import useDataSelectorState from './useDataSelectorState';

import StreamMasterSetting from '@/lib/StreamMasterSetting';
import { GetApiArg, QueryHook, camel2title, getTopToolOptions, isEmptyObject } from '@/lib/common/common';
import { skipToken } from '@reduxjs/toolkit/dist/query/react';
import { useQueryFilter } from '../../../lib/redux/slices/useQueryFilter';
import BanButton from '../buttons/BanButton';
import ResetButton from '../buttons/ResetButton';
import { useSetQueryFilter } from './useSetQueryFilter';

const DataSelector = <T extends DataTableValue,>(props: DataSelectorProps<T>) => {
  const { state, setters } = useDataSelectorState<T>(props.id, props.selectedItemsKey);

  useEffect(() => {
    if (!props.defaultSortField) {
      return;
    }

    if ((!state.sortField || state.sortField === '') && state.sortField !== props.defaultSortField) {
      setters.setSortField(props.defaultSortField);
    }

  }, [props.defaultSortField, setters, state.sortField]);

  useEffect(() => {
    if (!props.defaultSortOrder) {
      return;
    }

    function isEqual(value1: -1 | 0 | 1, value2: -1 | 0 | 1): boolean {
      return value1 === value2;
    }

    if (!state.sortOrder && !isEqual(state.sortOrder, props.defaultSortOrder)) {
      setters.setSortOrder(props.defaultSortOrder);
    }

  }, [props.defaultSortField, props.defaultSortOrder, setters, state.sortOrder]);

  const { queryFilter } = useQueryFilter(props.id);

  useSetQueryFilter(props.id, props.columns, state.first, state.filters, state.page, state.rows, props.selectedStreamGroupId);

  const tableRef = useRef<DataTable<T[]>>(null);

  const setting = StreamMasterSetting();

  const { data, isLoading, isFetching } = props.queryFilter ? props.queryFilter(queryFilter ?? skipToken) : { data: undefined, isFetching: false, isLoading: false };

  const onsetSelection = useCallback((e: T | T[], overRideSelectAll?: boolean): T | T[] | undefined => {
    let selected: T[] = Array.isArray(e) ? e : [e];

    if (state.selectSelectedItems === selected) {
      return;
    }

    if (props.selectionMode === 'single') {
      selected = selected.slice(0, 1);
    }

    setters.setSelectSelectedItems(selected);
    const all = overRideSelectAll ? overRideSelectAll : state.selectAll;

    if (props.onSelectionChange) {
      // props.onSelectionChange(props.selectionMode === 'single' ? selected : selected, all);
      props.onSelectionChange(selected , all);
    }

    return e;
  }, [state.selectSelectedItems, state.selectAll, props, setters]);


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
          setters.setSelectSelectedItems(data);
        }
      }

      return;
    }

    if (data && isPagedTableDto<T>(data)) {
      if (!state.dataSource || (state.dataSource && !areArraysEqual(data.data, state.dataSource))) {
        setters.setDataSource((data as PagedResponseDto<T>).data);

        if (state.selectAll && data !== undefined) {
          setters.setSelectSelectedItems((data as PagedResponseDto<T>).data as T[]);
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


  }, [data, onsetSelection, props.dataSource, props.reorderable, setters, state.dataSource, state.selectAll]);

  const onRowReorder = (changed: T[]) => {
    setters.setDataSource(changed);

    if (state.prevDataSource === undefined) {
      setters.setPrevDataSource(changed);
    }

    props.onRowReorder?.(changed);

  }


  const rowClass = useCallback((changed: DataTableRowData<T[]>) => {

    // const isLoading2 = getRecord(changed as T, 'isLoading');

    const isHidden = getRecord(changed as T, 'isHidden');

    if (isHidden === true) {
      return `bg-red-900`;
    }

    if (props.videoStreamIdsIsReadOnly !== undefined && props.videoStreamIdsIsReadOnly.length > 0) {
      const isReadOnly = props.videoStreamIdsIsReadOnly.find((vs) => vs === getRecord(changed as T, 'id'));

      if (isReadOnly !== undefined) {
        return 'videostreamSelected'
      }
    }

    return {};
  }, [props.videoStreamIdsIsReadOnly]);

  const exportCSV = () => {
    tableRef.current?.exportCSV({ selectionOnly: false });
  };

  useEffect(() => {
    if (!props.scrollTo) {
      return;
    }

    const scroller = tableRef.current?.getVirtualScroller();
    console.log('Scroll to', props.scrollTo ?? 0, 'smooth');
    // scroller?.scrollTo({ behavior: 'auto', left: 0, top: props.scrollTo })
    // scroller?.scrollInView({ behavior: 'auto', left: 0, top: props.scrollTo })
    scroller?.scrollToIndex(props.scrollTo ?? 0, 'smooth');

  }, [props.scrollTo]);

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
        props.onSelectionChange(sel, state.selectAll);
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

  }, [onsetSelection, props, state.selectAll]);

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

    if (fieldType === 'image') {
      return {
        maxWidth: '5rem',
        width: '5rem',
      } as CSSProperties;
    }

    if (fieldType === 'm3ulink' || fieldType === 'epglink' || fieldType === 'url') {
      return {
        maxWidth: '3rem',
        width: '3rem',
      } as CSSProperties;
    }

    return {
      ...style,
      flexGrow: 0,
      flexShrink: 1,
      overflow: 'hidden',
      paddingLeft: '0.5rem !important',
      paddingRight: '0.5rem !important',
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
      <div className="text-xs text-white text-500" >
        <BanButton
          className="banbutton"
          disabled={(state.selectSelectedItems || []).length === 0}
          onClick={() => {
            setters.setSelectSelectedItems([]);
            setters.setSelectAll(false);
            if (props.onSelectionChange) {
              props.onSelectionChange([], state.selectAll);
            }
          }}

          tooltip={`Clear ${(state.selectSelectedItems || []).length} Selections`}
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
    const newFilters = generateFilterData(props.columns, event.filters);

    setters.setFilters(newFilters);
  }

  const onSelectAllChange = (event: DataTableSelectAllChangeEvent) => {
    const newSelectAll = event.checked;

    setters.setSelectAll(newSelectAll);
    if (newSelectAll === true) {
      setters.setSelectSelectedItems([]);
    }
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
          dataKey='id'
          editMode='cell'
          emptyMessage={props.emptyMessage}
          expandableRowGroups={props.groupRowsBy !== undefined && props.groupRowsBy !== ''}
          expandedRows={state.expandedRows}
          exportFilename={props.exportFilename ?? 'streammaster'}
          filterDelay={500}
          filterDisplay="row"
          filters={isEmptyObject(state.filters) ? getEmptyFilter(props.columns, state.showHidden) : state.filters}
          first={state.pagedInformation ? state.pagedInformation.first : state.first}
          header={sourceRenderHeader}
          key='id'
          lazy
          loading={props.isLoading === true || isFetching === true || isLoading === true}
          metaKeySelection={false}
          onFilter={onFilter}
          onPage={onPage}
          onRowClick={(e) => props.onRowClick?.(e)}
          onRowReorder={(e) => { onRowReorder(e.value) }}
          onRowToggle={(e: DataTableRowToggleEvent) => setters.setExpandedRows(e.data as DataTableExpandedRows)}
          onSelectAllChange={props.reorderable ? undefined : onSelectAllChange}
          onSelectionChange={((e) => props.reorderable ? undefined : onSelectionChange(e))}
          onSort={onSort}
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
          scrollHeight='flex'
          scrollable
          selectAll={state.selectAll}
          selection={state.selectSelectedItems}
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
          totalRecords={state.pagedInformation ? state.pagedInformation.totalItemCount : undefined}
          value={state.dataSource}
        >
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
              // filterMenuStyle={{ width: '14rem' }}
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
// DataSelector.defaultProps = {
//   defaultSortField: 'name',
//   defaultSortOrder: 1,
//   enableVirtualScroll: false,
//   headerName: '',
//   hideControls: false,
//   onSelectionChange: undefined,
//   reorderable: false,
//   selectionMode: 'single',
//   showHeaders: true
// };


type BaseDataSelectorProps<T = any> = {
  className?: string;
  columns: ColumnMeta[];
  defaultSortField?: string;
  defaultSortOrder?: -1 | 0 | 1;
  emptyMessage?: ReactNode;
  enableExport?: boolean;
  exportFilename?: string;
  groupRowsBy?: string;
  headerLeftTemplate?: ReactNode;
  headerName?: string;
  headerRightTemplate?: ReactNode;
  hideControls?: boolean;
  id: string;
  isLoading?: boolean;
  key?: string | undefined;

  // onLazyLoad?: (e: any) => void;
  onMultiSelectClick?: (value: boolean) => void;
  onRowClick?: (event: DataTableRowClickEvent) => void;
  onRowReorder?: (value: T[]) => void;
  onRowVisibleClick?: (value: T) => void;
  onSelectionChange?: (value:  T[], selectAll: boolean) => void;
  // onValueChanged?: (value: T[]) => void;
  reorderable?: boolean;
  scrollTo?: number;
  selectedItemsKey: string;
  selectedStreamGroupId?: number;
  selectionMode?: DataSelectorSelectionMode;
  showHeaders?: boolean | undefined;
  showSelector?: boolean;
  sortField?: string;
  sortOrder?: number;
  style?: CSSProperties;
  videoStreamIdsIsReadOnly?: string[] | undefined;
  // virtualScrollHeight?: string | undefined;
}


type QueryFilterProps<T> = BaseDataSelectorProps<T> & {
  dataSource?: never;
  queryFilter: (filters: GetApiArg | typeof skipToken) => ReturnType<QueryHook<PagedResponseDto<T> | T[]>>;
};

type DataSourceProps<T> = BaseDataSelectorProps<T> & {
  dataSource: T[] | undefined;
  queryFilter?: never;
};

export type DataSelectorProps<T = any> = DataSourceProps<T> | QueryFilterProps<T>;

export type PagedTableInformation = {
  first: number;
  pageNumber: number;
  pageSize: number;
  totalItemCount: number;
  totalPageCount: number;
};

export type PagedDataDto<T> = {
  data?: T[];
};

export type PagedTableDto<T> = PagedDataDto<T> & PagedTableInformation & {
};


export default memo(DataSelector);
