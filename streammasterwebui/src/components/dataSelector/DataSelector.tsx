import './DataSelector.css';

import { Button } from 'primereact/button';
import { Column } from 'primereact/column';


import { type DataTableSelectionSingleChangeEvent } from 'primereact/datatable';
import { type DataTableSelectAllChangeEvent } from 'primereact/datatable';
import { type DataTableRowDataArray } from 'primereact/datatable';
import { type DataTableSortEvent } from 'primereact/datatable';
import { type DataTableStateEvent } from 'primereact/datatable';
import { type DataTablePageEvent } from 'primereact/datatable';

import { type DataTableExpandedRows } from 'primereact/datatable';
import { type DataTableRowToggleEvent } from 'primereact/datatable';
import { type DataTableValue } from 'primereact/datatable';

import { type DataTableRowData } from 'primereact/datatable';
import { DataTable } from 'primereact/datatable';
import { type ReactNode } from 'react';
import { memo, useCallback, useEffect, useMemo, useRef, type CSSProperties } from 'react';
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


const DataSelector = <T extends DataTableValue,>(props: DataSelectorProps<T>) => {
  const {
    selectAll, setSelectAll,
    rowClick, setRowClick,
    selections, setSelections,
    pagedInformation, setPagedInformation,
    dataSource, setDataSource,
    sortOrder, setSortOrder,
    sortField, setSortField,
    first, setFirst,
    page, setPage,
    rows, setRows,
    filters, setFilters,
    expandedRows, setExpandedRows,
  } = useDataSelectorState<T>(props.id);

  const tableRef = useRef<DataTable<T[]>>(null);

  let tempSort = '';
  let tempSortOrder: -1 | 0 | 1 = 1;
  let tempFirst = 0;
  let tempPage = 1;
  let tempRows = 25;

  const setting = StreamMasterSetting();

  const lazyState = useCallback((overrides?: Partial<LazyTableState>): LazyTableState => {
    console.log('lazyState', overrides);

    const effectiveSortField = overrides?.sortField ?? sortField;
    const effectiveSortOrder = overrides?.sortOrder ?? sortOrder;

    let sort = '';
    if (effectiveSortField) {
      sort = (effectiveSortOrder === -1) ? `${effectiveSortField} desc` : (effectiveSortOrder === 1) ? `${effectiveSortField} asc` : '';
    }

    const defaultState: LazyTableState = {
      filters: filters,
      filterString: JSON.stringify(filters),
      first: first,
      page: page,
      rows: rows,
      sortField: sortField,
      sortOrder: sortOrder,
      sortString: sort
    };

    return {
      ...defaultState,
      ...overrides,
      filterString: JSON.stringify(overrides?.filters ?? filters)
    }
  }, [filters, first, page, rows, sortField, sortOrder]);


  const isLoading = useMemo(() => {
    if (props.isLoading) {
      return true;
    }

    if (rowClick === undefined) {
      return true;
    }

    return false;

  }, [props.isLoading, rowClick]);


  const getRecord = useCallback((data: T, fieldName: string) => {
    type ObjectKey = keyof typeof data;
    const record = data[fieldName as ObjectKey];
    return record;
  }, []);

  const getRecordString = useCallback((data: T, fieldName: string): string => {
    type ObjectKey = keyof typeof data;
    const record = data[fieldName as ObjectKey];
    let toDisplay = JSON.stringify(record);

    if (!toDisplay || toDisplay === undefined || toDisplay === '') {
      // console.log("toDisplay is empty for " + fieldName)
      return '';
    }

    if (toDisplay.startsWith('"') && toDisplay.endsWith('"')) {
      toDisplay = toDisplay.substring(1, toDisplay.length - 1);
    }

    return toDisplay;
  }, []);

  const onValueChanged = useCallback((data: DataTableRowDataArray<T[]>) => {
    if (!data) {
      return;
    }

    props?.onValueChanged?.(data);

  }, [props]);

  useEffect(() => {
    if (!props.dataSource || isEmptyObject(props.dataSource)) {
      return;
    }

    if (Array.isArray(props.dataSource)) {
      setDataSource({ data: props.dataSource });
      setPagedInformation(undefined);
    } else if (isPagedTableDto(props.dataSource)) {
      setDataSource({ data: (props.dataSource as PagedTableDto<T>).data });
      setPagedInformation(props.dataSource);
    } else {
      setDataSource({ data: props.dataSource });
      setPagedInformation(undefined);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.dataSource]);

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const onRowReorder = useCallback((data: T[]) => {
    // setDataSource(data);
    // props.onSelectionChange?.(data);
  }, []);


  const rowClass = useCallback((data: DataTableRowData<T[]>) => {
    const isHidden = getRecord(data as T, 'isHidden');
    if (isHidden === true) {
      return `bg-red-900`;
    }

    return {};
  }, [getRecord]);


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
      rowClick={rowClick}
      setRowClick={setRowClick}
    />);
  }, [props, rowClick, setRowClick]);

  const onsetSelection = useCallback((e: T | T[]): T | T[] | undefined => {
    let selected: T[] = Array.isArray(e) ? e : [e];

    if (props.selectionMode === 'single') {
      selected = selected.slice(0, 1);
    }

    setSelections(selected);

    if (props.onSelectionChange) {
      props.onSelectionChange(props.selectionMode === 'single' ? selected[0] : selected);
    }

    return e;
  }, [props, setSelections]);


  const getSelectionMultipleMode = useMemo((): 'checkbox' | 'multiple' | null => {

    return 'multiple';

  }, []);


  const onSelectionChange = useCallback((e: DataTableSelectionSingleChangeEvent<T[]>) => {
    if (e.value === null || e.value === undefined) {
      return;
    }


    if (e.value instanceof Array && e.value.length > 0) {
      const single1 = e.value.slice(e.value.length - 1, e.value.length);
      onsetSelection(single1);
    } else {
      onsetSelection([e.value]);
    }

  }, [onsetSelection]);

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

  const rowGroupHeaderTemplate = useCallback((data: T) => {
    if (!props.groupRowsBy) {
      return (
        <div />
      );
    }

    const record = getRecordString(data, props.groupRowsBy);
    return (
      <span className="vertical-align-middle ml-2 font-bold line-height-3">
        {record}
        <Button
          className={data.isHidden ? 'p-button-danger' : ''}
          icon={`pi pi-eye ${data.isHidden ? 'text-red-500' : 'text-green-500'}`}
          onClick={() => props?.onRowVisibleClick?.(data)}
          rounded
          text
          tooltip="Set Hidden"
          tooltipOptions={getTopToolOptions}
        />
      </span>
    );
  }, [getRecordString, props]);

  const multiselectHeader = () => {
    return (
      <div className="absolute top-0 left-50 text-xs text-white text-500" />
    );
  }

  const onSort = (event: DataTableSortEvent) => {
    // If the sortField is 'selected' or absent, update the sortField and exit early.
    if (!event.sortField || event.sortField === 'selected') {
      // setSortField('');
      return;
    }

    // Set the sort order regardless of other conditions.
    switch (event.sortOrder) {
      case -1:
        tempSortOrder = -1;
        setSortOrder(-1);
        break;
      case 0:
        tempSortOrder = 0;
        setSortOrder(0);
        break;
      case 1:
        tempSortOrder = 1;
        setSortOrder(1);
        break;
      default:
        tempSortOrder = 0;
        setSortOrder(0);
    }

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
    setSortField(sort);
    tempSort = sort;

    console.log('onSort', sort, event.sortOrder)
    // Call the onFilter prop if it exists.
    props.onFilter?.(lazyState({ sortField: sort, sortOrder: event.sortOrder }));
  };

  const onPage = (event: DataTablePageEvent) => {

    const adjustedPage = (event.page ?? 0) + 1;

    tempPage = adjustedPage;
    setPage(adjustedPage);

    tempFirst = event.first;
    setFirst(event.first);

    tempRows = event.rows;
    setRows(event.rows);

    props.onFilter?.(lazyState({ first, page: adjustedPage, rows }));
  };

  const onFilter = (event: DataTableStateEvent) => {

    const newFilters = generateFilterData(props.columns, event.filters, props.showHidden);
    setFilters(newFilters);

    console.log('onFilter', sortField, tempSort, sortOrder, tempSortOrder)
    props.onFilter?.(lazyState({ filters: newFilters, first: tempFirst, page: tempPage, rows: tempRows, sortField: tempSort, sortOrder: tempSortOrder }));
  }

  const onSelectAllChange = (event: DataTableSelectAllChangeEvent) => {
    const newSelectAll = event.checked;
    setSelectAll(newSelectAll);
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
          expandedRows={expandedRows}
          exportFilename={props.exportFilename ?? 'streammaster'}
          filterDelay={500}
          filterDisplay="row"
          filters={isEmptyObject(filters) ? getEmptyFilter(props.columns, props.showHidden) : filters}
          first={pagedInformation ? pagedInformation.first : undefined}
          groupRowsBy={props.groupRowsBy}
          header={sourceRenderHeader}
          key={props.key !== undefined && props.key !== '' ? props.key : undefined}
          lazy
          loading={isLoading}
          metaKeySelection={false}
          onFilter={onFilter}
          onPage={onPage}
          onRowReorder={(e) => onRowReorder(e.value)}
          onRowToggle={(e: DataTableRowToggleEvent) => setExpandedRows(e.data as DataTableExpandedRows)}
          onSelectAllChange={onSelectAllChange}
          // eslint-disable-next-line @typescript-eslint/no-explicit-any
          onSelectionChange={((e: any) => onSelectionChange(e))}
          onSort={onSort}
          onValueChange={(e) => { onValueChanged(e); }}
          paginator
          paginatorClassName='text-xs p-0 m-0 withpadding'
          paginatorTemplate="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
          ref={tableRef}
          reorderableRows={props.reorderable}
          resizableColumns
          rowClassName={rowClass}
          rowGroupHeaderTemplate={rowGroupHeaderTemplate}
          rowGroupMode={props.groupRowsBy !== undefined && props.groupRowsBy !== '' ? 'subheader' : undefined}
          rows={rows}
          rowsPerPageOptions={[25, 50, 100, 250]}
          scrollHeight={props.enableVirtualScroll === true ? props.virtualScrollHeight !== undefined ? props.virtualScrollHeight : '400px' : 'flex'}
          scrollable
          selectAll={selectAll}
          selection={selections}
          selectionMode={getSelectionMultipleMode}
          showGridlines
          showHeaders={props.showHeaders}
          sortField={sortField}
          sortMode='single'
          sortOrder={sortOrder}
          stateKey={`${props.id}-table`}
          stateStorage="local"
          stripedRows
          style={props.style}
          totalRecords={pagedInformation ? pagedInformation.totalRecords : undefined}
          value={dataSource?.data}
          virtualScrollerOptions={props.enableVirtualScroll === true ? { itemSize: 16, orientation: 'vertical' } : undefined}
        >
          <Column
            body={<i className="pi pi-chevron-right" />}
            className='max-w-1rem p-0 justify-content-center align-items-center'
            field='selector'
            header=""
            hidden={!props.showSelector}
            style={{ width: '1rem' }}
          />
          <Column
            className='max-w-2rem p-0 justify-content-center align-items-center'
            field='rank'
            hidden={!props.reorderable}
            rowReorder
            style={{ width: '2rem' }}
          />
          <Column
            align='center'
            alignHeader='center'
            className={`justify-content-center align-items-center multiselect ${props.selectionMode}`}
            field='getSelectionMode'
            header={multiselectHeader}
            headerStyle={{ padding: '0px', width: '3rem' }}
            hidden={props.selectionMode !== 'multiple' && props.selectionMode !== 'checkbox' && props.selectionMode !== 'multipleNoRowCheckBox'}
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
              showAddButton
              showApplyButton
              showClearButton
              showFilterMatchModes
              showFilterMenu={col.filterElement === undefined}
              showFilterMenuOptions
              showFilterOperator
              sortable={props.groupRowsBy === undefined || props.groupRowsBy === '' ? col.sortable : false}
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
export type DataSelectorProps<T = any> = {
  className?: string;
  columns: ColumnMeta[];
  dataSource: PagedTableDto<T> | T[] | undefined;
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
  onFilter?: (event: LazyTableState) => void;
  onMultiSelectClick?: (value: boolean) => void;
  onRowVisibleClick?: (value: T) => void;
  onSelectionChange?: (value: T | T[]) => void;
  onValueChanged?: (value: T[]) => void;
  reorderable?: boolean;
  selectionMode?: DataSelectorSelectionMode;
  showHeaders?: boolean | undefined;
  showHidden?: boolean | null | undefined;
  showSelector?: boolean;
  style?: CSSProperties;
  virtualScrollHeight?: string | undefined;
}

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
