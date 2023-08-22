import './DataSelector.css';

import { FilterMatchMode } from 'primereact/api';
import { Button } from 'primereact/button';
import { Column } from 'primereact/column';

import { type DataTableFilterMeta } from 'primereact/datatable';
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
import { type ReactNode, type SyntheticEvent } from 'react';
import { memo, useCallback, useEffect, useMemo, useRef, useState, type CSSProperties } from 'react';

import { type SMDataTableFilterMetaData } from '../../common/common';
import { ExportComponent, HeaderLeft, MultiSelectCheckbox, camel2title, getTopToolOptions, isEmptyObject } from '../../common/common';
import StreamMasterSetting from '../../store/signlar/StreamMasterSetting';

import { Tooltip } from 'primereact/tooltip';
import { v4 as uuidv4 } from 'uuid';

import { type ColumnAlign, type ColumnFieldType, type DataSelectorSelectionMode } from './DataSelectorTypes';
import { type ColumnMeta } from './DataSelectorTypes';
import { useLocalStorage } from 'primereact/hooks';

export type LazyTableState = {
  filterString: string;
  filters: DataTableFilterMeta;
  first: number;
  page: number;
  rows: number;
  sortField?: string;
  sortOrder?: -1 | 0 | 1 | null | undefined;
  sortString: string;
}

const DataSelector = <T extends DataTableValue,>(props: DataSelectorProps<T>) => {
  const tableRef = useRef<DataTable<T[]>>(null);

  const tooltipClassName = useMemo(() => "menuitemds-" + uuidv4(), []);
  const [selectAll, setSelectAll] = useState<boolean>(false);
  const [rowClick, setRowClick] = useLocalStorage<boolean>(false, props.id + '-rowClick');

  // const [selections, setSelections] = useLocalStorage<T[]>([] as T[], props.id + '-selections');

  const [selections, setSelections] = useState<T[]>([] as T[]);
  const [pagedInformation, setPagedInformation] = useState<PagedTableInformation>();
  const [dataSource, setDataSource] = useState<PagedDataDto<T>>();

  const [sortOrder, setSortOrder] = useLocalStorage<-1 | 0 | 1 | null | undefined>(1, props.id + '-tempSortOrder');
  const [sortField, setSortField] = useLocalStorage<string>('', props.id + '-tempSortField');
  const [filters, setFilters] = useLocalStorage<DataTableFilterMeta>({}, props.id + '-tempfilters');
  const [first, setFirst] = useLocalStorage<number>(0, props.id + '-tempsetFirst');
  const [page, setPage] = useLocalStorage<number>(1, props.id + '-tempsetPage');
  const [rows, setRows] = useLocalStorage<number>(25, props.id + '-tempsetRows');


  const [expandedRows, setExpandedRows] = useState<DataTableExpandedRows>();

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

  function isPagedTableDto(value: PagedTableDto<T> | T[] | undefined): value is PagedTableDto<T> {
    if (!value || Array.isArray(value)) {
      return false;
    }


    return (
      value &&
      (value.data === undefined || Array.isArray(value.data)) &&
      typeof value.first === 'number' &&
      typeof value.pageNumber === 'number' &&
      typeof value.pageSize === 'number' &&
      typeof value.totalItemCount === 'number' &&
      typeof value.totalPageCount === 'number' &&
      typeof value.totalRecords === 'number'
    );
  }

  const isLoading = useMemo(() => {
    if (props.isLoading) {
      return true;
    }

    if (rowClick === undefined) {
      return true;
    }

    return false;

  }, [props.isLoading, rowClick]);



  const generateFilterData = (currentFilters: DataTableFilterMeta) => {
    if (!props.columns || !currentFilters) {
      return {};
    }

    return props.columns.reduce<DataTableFilterMeta>((obj, item: ColumnMeta) => {
      if (item.field === 'isHidden') {

        return {
          ...obj,
          [item.field]: {
            fieldName: item.field,
            matchMode: FilterMatchMode.EQUALS,
            value: props.showHidden === null ? null : !props.showHidden
          },
        } as DataTableFilterMeta;
      }

      let value = '';
      if (Object.keys(currentFilters).length > 0) {
        const test = currentFilters[item.field] as SMDataTableFilterMetaData;
        if (test !== undefined) {
          value = test.value;
        }
      }

      return {
        ...obj,
        [item.field]: {
          fieldName: item.field,
          matchMode: item.filterMatchMode ?? FilterMatchMode.CONTAINS,
          value: value
        },
      } as DataTableFilterMeta;
    }, {});
  };

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

  const imageBodyTemplate = useCallback((data: T, fieldName: string) => {
    const record = getRecordString(data, fieldName);

    return (
      <div className="flex flex-nowrap justify-content-center align-items-center p-0">
        <img
          alt={record ?? 'Logo'}
          className="max-h-1rem max-w-full p-0"
          onError={(e: SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = setting.defaultIcon))}
          src={`${encodeURI(record ?? '')}`}
          style={{
            objectFit: 'contain',
          }}
        />
      </div>
    );
  }, [getRecordString, setting.defaultIcon]);

  const onValueChanged = useCallback((data: DataTableRowDataArray<T[]>) => {
    if (!data) {
      return;
    }

    props?.onValueChanged?.(data);

  }, [props]);

  const streamsBodyTemplate = useCallback((activeCount: string, totalCount: string) => {
    if (activeCount === null || totalCount === undefined) {
      return null;
    }

    return (
      <div className="flex align-items-center gap-2" >
        {activeCount}/{totalCount}
      </div>
    );

  }, []);


  const linkIcon = (url: string) => {
    return (
      <a href={url} rel="noopener noreferrer" target="_blank">
        <i className="pi pi-bookmark-fill" />
      </a>
    );
  };

  const linkTemplate = useCallback((link: string) => (
    <div>
      <div className="flex justify-content-center align-items-center">
        {linkIcon(link)}
      </div>
    </div>
  ), []);

  const epgSourceTemplate = useCallback((tvgid: string) => {
    return (
      <div>
        <div className="flex justify-content-start">
          {tvgid}
        </div>
      </div>
    );
  }, []);

  const bodyTemplate = useCallback((data: T, fieldName: string, fieldType: ColumnFieldType, camelize?: boolean) => {

    if (fieldName === undefined || fieldName === '') {
      return <div />;
    }

    // Helper function for 'isHidden' fieldType
    const renderIsHidden = (record: boolean) => {
      if (record !== true) {
        return <i className="pi pi-eye text-green-500" />;
      }

      return <i className="pi pi-eye-slash text-red-500" />;
    };

    // Simplify the rendering logic using a switch statement
    switch (fieldType) {
      case 'blank':
        return <div />;
      case 'm3ulink':
        return linkTemplate(getRecordString(data, 'm3ULink'));
      case 'epglink':
        return linkTemplate(getRecordString(data, 'xmlLink'));
      case 'url':
        return linkTemplate(getRecordString(data, 'hdhrLink'));
      case 'epg':
        return epgSourceTemplate(getRecordString(data, 'user_Tvg_ID'));
      case 'image':
        return imageBodyTemplate(data, fieldName);
      case 'streams':
        const activeCount = getRecord(data, 'activeCount');
        const totalCount = getRecord(data, 'totalCount');
        return streamsBodyTemplate(activeCount, totalCount);
      case 'isHidden':
        return renderIsHidden(getRecord(data, fieldName));
      case 'deleted':
        const toDisplay = getRecord(data, 'isHidden');
        return (
          <span className={`flex ${toDisplay !== true ? 'bg-green-900' : 'bg-red-900'} min-w-full min-h-full justify-content-center align-items-center`}>
            {toDisplay}
          </span>
        );
      default:
        let displayValue = JSON.stringify(getRecord(data, fieldName));
        if (displayValue.startsWith('"') && displayValue.endsWith('"')) {
          displayValue = displayValue.substring(1, displayValue.length - 1);
        }

        if (camelize) {
          displayValue = camel2title(displayValue);
        }

        return (
          <span style={{ display: 'block', overflow: 'hidden', padding: '0rem !important', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
            {displayValue}
          </span>
        );
    }
  }, [epgSourceTemplate, getRecord, getRecordString, imageBodyTemplate, linkTemplate, streamsBodyTemplate]);

  useEffect(() => {
    if (!props.dataSource || isEmptyObject(props.dataSource)) {
      return;
    }

    if (isPagedTableDto(props.dataSource)) {
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

    return (
      <div className="flex grid flex-row w-full flex-wrap grid align-items-center w-full col-12 h-full p-0 debug">
        <div className="flex col-2 text-orange-500 h-full text-sm align-items-center p-0 debug">
          {props.name}
          <MultiSelectCheckbox
            onMultiSelectClick={props.onMultiSelectClick}
            props={props}
            rowClick={rowClick}
            setRowClick={setRowClick}
          />
        </div>
        <div className="flex col-10 h-full align-items-center p-0 px-2 m-0 debug">
          <div className="grid mt-2 flex flex-nowrap flex-row justify-content-between align-items-center col-12 px-0">
            <HeaderLeft props={props} />
            {props.headerRightTemplate}
            {props.enableExport && <ExportComponent exportCSV={exportCSV} />}
          </div>
        </div>
      </div>
    );
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


  const getHeader = useCallback((field: string, header: string | undefined, fieldType: ColumnFieldType | undefined): ReactNode => {
    if (!fieldType) {
      return header ? header : camel2title(field);
    }

    switch (fieldType) {
      case 'blank':
        return <div />;
      case 'epg':
        return 'EPG';
      case 'm3ulink':
        return 'M3U';
      case 'epglink':
        return 'XMLTV';
      case 'url':
        return 'HDHR URL';
      case 'streams':
        return (
          <>
            <Tooltip target={"." + tooltipClassName} />
            <div
              className={tooltipClassName + " border-white"}
              data-pr-at="right+5 top"
              data-pr-hidedelay={100}
              data-pr-my="left center-2"
              data-pr-position="right"
              data-pr-showdelay={200}
              data-pr-tooltip="Active/Total Count"
            >
              Streams<br />(active/total)
            </div>
          </>
        );
      default:
        return header ? header : camel2title(field);
    }
  }, [tooltipClassName]);


  const multiselectHeader = () => {
    return (
      <div className="absolute top-0 left-50 text-xs text-white text-500" />
    );
  }

  const onFilter = (event: DataTableStateEvent) => {
    const newFilters = generateFilterData(event.filters);
    setFilters(newFilters);
    props.onFilter?.(lazyState({ filters: newFilters }));
  }


  const onPage = (event: DataTablePageEvent) => {
    const {
      tempPage = 0,
      tempFirst = 0,
      tempRows = 25
    } = event;

    const adjustedPage = tempPage + 1;

    setPage(adjustedPage);
    setFirst(tempFirst);
    setRows(tempRows);

    props.onFilter?.(lazyState({ first, page: adjustedPage, rows }));
  };

  const onSort = (event: DataTableSortEvent) => {
    // Set the sort order regardless of other conditions.
    setSortOrder(event.sortOrder);

    // If the sortField is 'selected' or absent, update the sortField and exit early.
    if (!event.sortField || event.sortField === 'selected') {
      setSortField(event.sortField);
      return;
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

    // Call the onFilter prop if it exists.
    props.onFilter?.(lazyState({ sortField: sort, sortOrder: event.sortOrder }));
  };

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
          filters={filters}
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
              body={((e) => col.bodyTemplate ? col.bodyTemplate(e) : bodyTemplate(e, col.field, col.fieldType, col.camelize))}
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
  hideControls: false,
  key: undefined,
  name: '',
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
  headerRightTemplate?: ReactNode;
  hideControls?: boolean;
  id: string;
  isLoading?: boolean;
  key?: string | undefined;
  name?: string;
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
