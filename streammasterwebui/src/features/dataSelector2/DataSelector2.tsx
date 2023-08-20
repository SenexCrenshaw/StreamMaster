/* eslint-disable react/no-unused-prop-types */

import './DataSelector2.css';

import { FilterMatchMode } from 'primereact/api';
import { Button } from 'primereact/button';
import { Column } from 'primereact/column';
import { type DataTableRowDataArray } from 'primereact/datatable';
import { type DataTableSortEvent } from 'primereact/datatable';
import { type DataTableStateEvent } from 'primereact/datatable';
import { type DataTablePageEvent } from 'primereact/datatable';
import { type DataTableFilterMetaData } from 'primereact/datatable';
import { type DataTableFilterMeta } from 'primereact/datatable';
import { type DataTableExpandedRows } from 'primereact/datatable';
import { type DataTableRowToggleEvent } from 'primereact/datatable';
import { type DataTableSelectionChangeEvent } from 'primereact/datatable';
import { type DataTableValue } from 'primereact/datatable';

import { type DataTableRowData } from 'primereact/datatable';
import { DataTable } from 'primereact/datatable';
import { type CSSProperties } from 'react';
import React from 'react';
import { ExportComponent, HeaderLeft, MultiSelectCheckbox, areFilterMetaEqual, camel2title, getTopToolOptions } from '../../common/common';
import StreamMasterSetting from '../../store/signlar/StreamMasterSetting';

import { Tooltip } from 'primereact/tooltip';
import { v4 as uuidv4 } from 'uuid';

import { type ColumnAlign, type ColumnFieldType, type DataSelectorSelectionMode } from './DataSelectorTypes2';
import { type ColumnMeta } from './DataSelectorTypes2';
import { useLocalStorage } from 'primereact/hooks';


const DataSelector2 = <T extends DataTableValue,>(props: DataSelector2Props<T>) => {
  const tableRef = React.useRef<DataTable<T[]>>(null);

  const tooltipClassName = React.useMemo(() => "menuitemds-" + uuidv4(), []);

  const [pageSize, setPageSize] = useLocalStorage(25, props.id + '-rowsPerPage');
  const [sortOrder, setSortOrder] = useLocalStorage<-1 | 0 | 1 | null>(1, props.id + '-sortOrder');
  const [sortField, setSortField] = useLocalStorage<string>('name', props.id + '-sortField');
  const [rowClick, setRowClick] = useLocalStorage<boolean>(false, props.id + '-rowClick');
  const [selections, setSelections] = useLocalStorage<T[]>([] as T[], props.id + '-selections');
  const [sourceFilters, setSourceFilters] = useLocalStorage<DataTableFilterMeta | undefined>(undefined, props.id + '-sourceFilters');
  const [dataSource, setDataSource] = React.useState<PagedTableDto<T>>();

  const [expandedRows, setExpandedRows] = React.useState<DataTableExpandedRows>();

  const setting = StreamMasterSetting();

  const hasColumns = (columns?: ColumnMeta[]) => columns && columns.length > 0;

  const isLoading = React.useMemo(() => {
    if (props.isLoading) {
      return true;
    }

    if (rowClick === undefined) {
      return true;
    }

    return false;

  }, [props.isLoading, rowClick]);

  const onFilter = React.useCallback((event: DataTableStateEvent) => {

    setSourceFilters(event.filters);
    const pageInfo = { ...event };

    pageInfo.page = 0;
    props.onPage?.(pageInfo);
    props.onFilter?.(event);
  }, [props, setSourceFilters]);

  const generateFilterData = (columns: ColumnMeta[], currentFilters: DataTableFilterMeta) => {
    if (!columns || !currentFilters) {
      return {};
    }

    return columns.reduce<DataTableFilterMeta>((obj, item: ColumnMeta) => {
      if (item.field === 'isHidden') {

        return {
          ...obj,
          [item.field]: {
            matchMode: FilterMatchMode.EQUALS,
            value: props.showHidden === null ? null : !props.showHidden
          },
        } as DataTableFilterMeta;
      }

      let value = '';
      if (Object.keys(currentFilters).length > 0) {
        const test = currentFilters[item.field] as DataTableFilterMetaData;
        if (test !== undefined) {
          value = test.value;
        }
      }

      return {
        ...obj,
        [item.field]: {
          matchMode: item.filterMatchMode ?? FilterMatchMode.CONTAINS,
          value: value
        },
      } as DataTableFilterMeta;
    }, {});
  };

  React.useEffect(() => {
    if (!hasColumns(props.columns)) {
      return;
    }

    if (sourceFilters === undefined) {
      return;
    }

    const newFilters = generateFilterData(props.columns, sourceFilters);

    if (!areFilterMetaEqual(newFilters, sourceFilters)) {
      setSourceFilters(newFilters);
    }

    const event = {} as DataTableStateEvent;

    props.onFilter?.(event);
    event.filters = newFilters;

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.columns, props.showHidden]);

  const getRecord = React.useCallback((data: T, fieldName: string) => {
    type ObjectKey = keyof typeof data;
    const record = data[fieldName as ObjectKey];
    return record;
  }, []);

  const getRecordString = React.useCallback((data: T, fieldName: string): string => {
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

  const imageBodyTemplate = React.useCallback((data: T, fieldName: string) => {
    const record = getRecordString(data, fieldName);

    return (
      <div className="flex flex-nowrap justify-content-center align-items-center p-0">
        <img
          alt={record ?? 'Logo'}
          className="max-h-1rem max-w-full p-0"
          onError={(e: React.SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = setting.defaultIcon))}
          src={`${encodeURI(record ?? '')}`}
          style={{
            objectFit: 'contain',
          }}
        />
      </div>
    );
  }, [getRecordString, setting.defaultIcon]);

  const onValueChanged = React.useCallback((data: DataTableRowDataArray<T[]>) => {
    if (!data) {
      return;
    }

    props?.onValueChanged?.(data);

  }, [props]);

  const streamsBodyTemplate = React.useCallback((activeCount: string, totalCount: string) => {
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

  const linkTemplate = React.useCallback((link: string) => (
    <div>
      <div className="flex justify-content-center align-items-center">
        {linkIcon(link)}
      </div>
    </div>
  ), []);

  const epgSourceTemplate = React.useCallback((tvgid: string) => {
    return (
      <div>
        <div className="flex justify-content-start">
          {tvgid}
        </div>
      </div>
    );
  }, []);

  const bodyTemplate = React.useCallback((data: T, fieldName: string, fieldType: ColumnFieldType, camelize?: boolean) => {

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

  React.useEffect(() => {
    if (props.dataSource !== undefined) {
      setDataSource(props.dataSource);
    }

  }, [props.dataSource]);

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const onRowReorder = React.useCallback((data: T[]) => {
    // setDataSource(data);
    // props.onSelectionChange?.(data);
  }, []);


  const rowClass = React.useCallback((data: DataTableRowData<T[]>) => {

    const isHidden = getRecord(data as T, 'isHidden');

    // if (getRecord(data as T, 'regexMatch') === '') {

    //   const groupName = getRecord(data as T, 'name');

    //   if (groupName !== undefined && groupName !== '') {
    //     // if (streamNotHiddenCount(groupName) > 0) {
    //     //   return {};
    //     // }
    //   }
    // }

    if (isHidden === true) {
      return `bg-red-900`;
    }

    return {};
  }, [getRecord]);


  const exportCSV = () => {
    tableRef.current?.exportCSV({ selectionOnly: false });
  };


  const sourceRenderHeader = React.useMemo(() => {
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

  const onsetSelection = React.useCallback((e: T | T[]): T | T[] | undefined => {
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



  const getSelectionMode = React.useMemo(() => {
    switch (props.selectionMode) {
      case 'selectable':
        return rowClick ? 'multiple' : 'single';
      case 'multipleNoCheckBox':
        return 'multiple';
      case 'multipleNoRowCheckBox':
        return 'radiobutton';
      default:
        return props.selectionMode;
    }
  }, [props.selectionMode, rowClick]);



  const onSelectionChange = React.useCallback((e: DataTableSelectionChangeEvent<T[]>) => {
    if (e.value === null || e.value === undefined) {
      return;
    }


    if (getSelectionMode === 'single' && e.value instanceof Array && e.value.length > 0) {
      const single1 = e.value.slice(e.value.length - 1, e.value.length);
      onsetSelection(single1);
    } else {
      onsetSelection(e.value as T[]);
    }

  }, [getSelectionMode, onsetSelection]);

  const getAlign = React.useCallback((align: ColumnAlign | null | undefined, fieldType: ColumnFieldType): ColumnAlign => {

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

  const getAlignHeader = React.useCallback((align: ColumnAlign | undefined, fieldType: ColumnFieldType): ColumnAlign => {
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

  const getFilter = React.useCallback((filter: boolean | undefined, fieldType: ColumnFieldType): boolean | undefined => {
    if (fieldType === 'image') {
      return false;
    }

    return filter;
  }, [])


  const getStyle = React.useCallback((style: CSSProperties | undefined, fieldType: ColumnFieldType | undefined): CSSProperties | undefined => {

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

  const rowGroupHeaderTemplate = React.useCallback((data: T) => {
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


  const getHeader = React.useCallback((field: string, header: string | undefined, fieldType: ColumnFieldType | undefined): React.ReactNode => {
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


  const onPage = (event: DataTablePageEvent) => {

    if (event.rows !== undefined) {
      setPageSize(event.rows);
    }

    props.onPage?.(event);
  };

  const onSort = (event: DataTableSortEvent) => {

    if (event.sortField === 'selected' || event.sortOrder === undefined || event.sortField === undefined) {
      return;
    }


    setSortOrder(event.sortOrder);
    setSortField(event.sortField);



    if (props.onSort !== undefined) {

      if (event.sortField === null) {
        props.onSort?.('');
        return;
      }

      let toSend = event.sortField + " asc";

      if (event.sortOrder === 1) {
        toSend = event.sortField + " desc";
      }

      console.log("onSort:", toSend);
      props.onSort?.(toSend);
    }
  };


  return (

    <div className='dataselector2 flex w-full min-w-full  justify-content-start align-items-center' >
      <div className={`${props.className !== undefined ? props.className : ''} min-h-full w-full surface-overlay`}>
        <DataTable
          dataKey='id'
          editMode='cell'
          emptyMessage={props.emptyMessage}
          expandableRowGroups={props.groupRowsBy !== undefined && props.groupRowsBy !== ''}
          expandedRows={expandedRows}
          exportFilename={props.exportFilename ?? 'streammaster'}
          filterDelay={500}
          filterDisplay="row"
          filters={sourceFilters}
          first={dataSource?.first}
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
          onSelectionChange={((e) => onSelectionChange(e))}
          onSort={onSort}
          onValueChange={(e) => { onValueChanged(e); }}
          paginator
          paginatorClassName='text-xs p-0 m-0 withpadding'
          paginatorTemplate="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
          ref={tableRef}
          removableSort
          reorderableRows={props.reorderable}
          resizableColumns
          rowClassName={rowClass}
          rowGroupHeaderTemplate={rowGroupHeaderTemplate}
          rowGroupMode={props.groupRowsBy !== undefined && props.groupRowsBy !== '' ? 'subheader' : undefined}
          rows={pageSize}
          rowsPerPageOptions={[25, 50, 100, 250]}
          scrollHeight={props.enableVirtualScroll === true ? props.virtualScrollHeight !== undefined ? props.virtualScrollHeight : '400px' : 'flex'}
          scrollable
          selection={selections}
          selectionMode={getSelectionMode}
          showGridlines
          showHeaders={props.showHeaders}
          sortField={sortField}
          sortMode='single'
          sortOrder={sortOrder}
          stripedRows
          style={props.style}
          totalRecords={dataSource?.totalRecords}
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
          // sortField="selected"
          // sortFunction={sortFunction}
          // sortable={props.groupRowsBy === undefined || props.groupRowsBy === ''}
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
              showClearButton // ={props.showClearButton ?? col.filterType !== 'isHidden'}
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

DataSelector2.displayName = 'dataselector22';
DataSelector2.defaultProps = {
  // enableState: true,
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


/**
 * The props for the DataSelector component.
 *
 * @typeparam T The type of data being displayed in the table.
 */
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type DataSelector2Props<T = any> = {

  /**
   * The CSS class name for the component.
   */
  className?: string;
  /**
   * An array of ColumnMeta objects that define the columns of the table.
   */
  columns: ColumnMeta[];
  /**
   * An array of objects to display.
   */
  dataSource: PagedTableDto<T> | undefined;
  /**
   * A React node that is displayed when there is no data to display.
   */
  emptyMessage?: React.ReactNode;

  enableExport?: boolean;
  // eslint-disable-next-line react/no-unused-prop-types
  // enableState?: boolean | undefined;
  enableVirtualScroll?: boolean | undefined;
  exportFilename?: string;

  /**
   * The name of the field to group the rows by.
   */
  groupRowsBy?: string;
  /**
   * A React node that can be used to display additional content in the left side of the header of the table.
   */
  headerLeftTemplate?: React.ReactNode;
  /**
   * A React node that can be used to display additional content in the right side of the header of the table.
   */
  headerRightTemplate?: React.ReactNode;
  hideControls?: boolean;
  /**
   * The unique identifier of the component.
   */
  id: string;
  /**
   * Whether the component is currently loading data.
   */
  isLoading?: boolean;
  key?: string | undefined;
  /**
   * The name of the component.
   */
  name?: string;
  onFilter?: (event: DataTableFilterMeta) => void;
  /**
   * A function that is called when the multi-select button is clicked.
   */
  onMultiSelectClick?: (value: boolean) => void;
  onPage?: (event: DataTablePageEvent) => void;
  /**
   * A function that is called when a row's visibility is changed.
   */
  onRowVisibleClick?: (value: T) => void;
  /**
   * A function that is called when a row is selected.
   */
  onSelectionChange?: (value: T | T[]) => void;

  onSort?: (event: string) => void;
  /**
     * A function that is called when the value changes.
     */
  onValueChanged?: (value: T[]) => void;
  /**
   * Whether rows can be reordered.
   */
  reorderable?: boolean;
  /**
 * The currently selected row(s).
 */
  // selection?: T | T[];
  /**
     * The mode for row selection.
     */
  selectionMode?: DataSelectorSelectionMode;
  /**
   * Whether to show the selector column.
   */
  showClearButton?: boolean | undefined;
  showHeaders?: boolean | undefined;
  showHidden?: boolean | null | undefined;
  showSelector?: boolean;
  /**
   * The field to sort the data by.
   */
  // sortField?: string;
  /**
   * The inline style of the component.
   */
  style?: CSSProperties;
  virtualScrollHeight?: string | undefined;
}

export type PagedTableDto<T> = {
  data?: T[];
  first: number;
  pageNumber: number;
  pageSize: number;
  totalItemCount: number;
  totalPageCount: number;
  totalRecords: number;
};
export default React.memo(DataSelector2);
