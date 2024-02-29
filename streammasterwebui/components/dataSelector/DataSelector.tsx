import { GetApiArgument, QueryHook, camel2title, getTopToolOptions, isEmptyObject } from '@lib/common/common';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import useSettings from '@lib/useSettings';
import { areArraysEqual } from '@mui/base';
import { skipToken } from '@reduxjs/toolkit/dist/query/react';
import { Button } from 'primereact/button';
import { Column } from 'primereact/column';
import {
  DataTable,
  type DataTableExpandedRows,
  type DataTablePageEvent,
  type DataTableRowClickEvent,
  type DataTableRowData,
  type DataTableRowToggleEvent,
  type DataTableSelectAllChangeEvent,
  type DataTableSelectionMultipleChangeEvent,
  type DataTableSelectionSingleChangeEvent,
  type DataTableStateEvent,
  type DataTableValue
} from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useRef, type CSSProperties, type ReactNode } from 'react';

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

import { PagedResponseDto } from '@lib/common/dataTypes';
import BanButton from '../buttons/BanButton';
import ResetButton from '../buttons/ResetButton';
import { TriSelectShowSelection } from '../selectors/TriSelectShowSelection';
import { useSetQueryFilter } from './useSetQueryFilter';

const DataSelector = <T extends DataTableValue>(props: DataSelectorProps<T>) => {
  const debug = false;
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

  useSetQueryFilter(props.id, props.columns, state.first, state.filters, state.page, state.rows, props.selectedStreamGroupId);

  const { queryFilter } = useQueryFilter(props.id);

  const tableReference = useRef<DataTable<T[]>>(null);

  const setting = useSettings();

  if (debug && props.id === 'streamgroupeditor-StreamGroupSelectedVideoStreamDataSelector') {
    console.log(props.id, props.selectedStreamGroupId, props.selectedItemsKey);
    console.log(queryFilter);
  }

  const { data, isLoading, isFetching } = props.queryFilter
    ? props.queryFilter(queryFilter ?? skipToken)
    : { data: undefined, isFetching: false, isLoading: false };

  const onSetSelection = useCallback(
    (e: T | T[], overRideSelectAll?: boolean): T | T[] | undefined => {
      // if (e === undefined) {
      //   return;
      // }

      let selected: T[] = Array.isArray(e) ? e : [e];

      if (state.selectSelectedItems === selected) {
        return;
      }

      if (props.selectionMode === 'single') {
        selected = selected.slice(0, 1);
      }

      setters.setSelectSelectedItems(selected);
      const all = overRideSelectAll || state.selectAll;

      if (props.onSelectionChange) {
        // props.onSelectionChange(props.selectionMode === 'single' ? selected : selected, all);
        props.onSelectionChange(selected, all);
      }

      return e;
    },
    [state.selectSelectedItems, state.selectAll, props, setters]
  );

  const selectedData = useCallback(
    (inputData: T[]): T[] => {
      if (props.showSelections !== true) {
        return inputData;
      }

      if (state.showSelections === null) {
        return inputData;
      }

      if (state.showSelections === true) {
        return state.selectSelectedItems;
      }

      if (!state.selectSelectedItems) {
        return [] as T[];
      }

      const returnValue = inputData.filter((d) => !state.selectSelectedItems?.some((s) => s.id === d.id));

      return returnValue;
    },
    [props.showSelections, state.selectSelectedItems, state.showSelections]
  );

  useEffect(() => {
    if (debug && props.id === 'streamgroupeditor-StreamGroupSelectedVideoStreamDataSelector') {
      console.log('data', data);
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
        if (debug && props.id === 'streamgroupeditor-StreamGroupSelectedVideoStreamDataSelector') {
          console.log('data', (data as PagedResponseDto<T>).data);
        }
        if (state.selectAll && data !== undefined) {
          setters.setSelectSelectedItems((data as PagedResponseDto<T>).data as T[]);
        }
      }

      if (data.pageNumber > 1 && data.totalPageCount === 0) {
        const newData = { ...data };

        newData.pageNumber += -1;
        newData.first = (newData.pageNumber - 1) * newData.pageSize;
        setters.setPage(newData.pageNumber);
        setters.setFirst(newData.first);
        setters.setPagedInformation(newData);
      } else {
        setters.setPagedInformation(data);
      }
    }
  }, [data, props.id, setters, state.dataSource, state.selectAll]);

  useEffect(() => {
    if (!props.dataSource) {
      return;
    }

    const newData = selectedData(props.dataSource);

    if (!state.dataSource || (state.dataSource && !areArraysEqual(newData, state.dataSource))) {
      if (props.reorderable) {
        setters.setDataSource([...newData].sort((a, b) => a.rank - b.rank));
      } else {
        setters.setDataSource(newData);
      }

      setters.setPagedInformation(undefined);

      if (state.selectAll) {
        onSetSelection(newData);
      }
    }
  }, [onSetSelection, props.dataSource, props.reorderable, selectedData, setters, state.dataSource, state.selectAll]);

  const onRowReorder = (changed: T[]) => {
    setters.setDataSource(changed);

    if (state.prevDataSource === undefined) {
      setters.setPrevDataSource(changed);
    }

    props.onRowReorder?.(changed);
  };

  const rowClass = useCallback(
    (changed: DataTableRowData<T[]>) => {
      // const isLoading2 = getRecord(changed as T, 'isLoading');

      const isHidden = getRecord(changed as T, 'isHidden');

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (props.videoStreamIdsIsReadOnly !== undefined && props.videoStreamIdsIsReadOnly.length > 0) {
        const isReadOnly = props.videoStreamIdsIsReadOnly.find((vs) => vs === getRecord(changed as T, 'id'));

        if (isReadOnly !== undefined) {
          return 'videostreamSelected';
        }
      }

      return {};
    },
    [props.videoStreamIdsIsReadOnly]
  );

  const exportCSV = () => {
    tableReference.current?.exportCSV({ selectionOnly: false });
  };

  useEffect(() => {
    if (props.reset === true) {
      tableReference.current?.reset();
      props.OnReset?.();
    }
  }, [props, props.reset]);

  useEffect(() => {
    if (!props.scrollTo) {
      return;
    }

    const scroller = tableReference.current?.getVirtualScroller();
    // console.log('Scroll to', props.scrollTo ?? 0, 'smooth');
    // scroller?.scrollTo({ behavior: 'auto', left: 0, top: props.scrollTo })
    // scroller?.scrollInView({ behavior: 'auto', left: 0, top: props.scrollTo })
    scroller?.scrollToIndex(props.scrollTo ?? 0, 'smooth');
  }, [props.scrollTo]);

  const sourceRenderHeader = useMemo(() => {
    if (!props.headerLeftTemplate && !props.headerRightTemplate) {
      return null;
    }

    return (
      <TableHeader
        dataSelectorProps={props}
        enableExport={props.enableExport ?? false}
        exportCSV={exportCSV}
        headerName={props.headerName}
        onMultiSelectClick={props.onMultiSelectClick}
        rowClick={state.rowClick}
        setRowClick={setters.setRowClick}
      />
    );
  }, [props, state.rowClick, setters.setRowClick]);

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const getSelectionMultipleMode = useMemo((): any => {
    // 'single' | 'checkbox' | 'multiple' | null => {
    if (props.selectionMode === 'multiple') {
      return 'checkbox';
    }

    return 'single';
  }, [props.selectionMode]);

  const onSelectionChange = useCallback(
    (e: DataTableSelectionMultipleChangeEvent<T[]> | DataTableSelectionSingleChangeEvent<T[]>) => {
      if (e.value === null || e.value === undefined || e.value.length === 0 || !Array.isArray(e.value)) {
        return;
      }

      if (props.selectionMode === 'single') {
        if (e.value !== undefined && Array.isArray(e.value)) {
          if (e.value.length > 1) {
            onSetSelection(e.value[1]);
          } else {
            onSetSelection(e.value[0]);
          }
        } else {
          onSetSelection(e.value);
        }
        return;
      }

      if (props.selectionMode === 'multiple') {
        if (Array.isArray(e.value)) {
          onSetSelection(e.value);
        } else {
          onSetSelection([e.value]);
        }

        return;
      }

      let sel = [] as T[];

      if (Array.isArray(e.value)) {
        const single1 = e.value.slice(-1, e.value.length);

        sel = single1;
      } else {
        sel = [e.value];
      }

      if (props.reorderable === true && props.onSelectionChange) {
        props.onSelectionChange(sel, state.selectAll);
      }

      onSetSelection(sel);
    },
    [onSetSelection, props, state.selectAll]
  );

  const getAlign = useCallback((align: ColumnAlign | null | undefined, fieldType: ColumnFieldType): ColumnAlign => {
    if (fieldType === 'image') {
      return 'center';
    }

    if (fieldType === 'isHidden') {
      return 'center';
    }

    if (align === undefined || align === null) {
      return 'left';
    }

    return align;
  }, []);

  const getAlignHeader = useCallback((align: ColumnAlign | undefined, fieldType: ColumnFieldType): ColumnAlign => {
    if (fieldType === 'image') {
      return 'center';
    }

    if (fieldType === 'isHidden') {
      return 'center';
    }

    if (!align) {
      return 'center';
    }

    return align;
  }, []);

  const getFilter = useCallback((filter: boolean | undefined, fieldType: ColumnFieldType): boolean | undefined => {
    if (fieldType === 'image') {
      return false;
    }

    return filter;
  }, []);

  const getStyle = useCallback((col: ColumnMeta): CSSProperties | undefined => {
    if (col.fieldType === 'blank') {
      return {
        maxWidth: '1rem',
        width: '1rem'
      } as CSSProperties;
    }

    if (col.fieldType === 'image') {
      return {
        maxWidth: '5rem',
        width: '5rem'
      } as CSSProperties;
    }

    if (col.fieldType === 'm3ulink' || col.fieldType === 'epglink' || col.fieldType === 'url') {
      return {
        maxWidth: '3rem',
        width: '3rem'
      } as CSSProperties;
    }

    if (col.width !== undefined && col.width !== '') {
      return {
        ...col.style,
        flexGrow: 0,
        flexShrink: 1,
        maxWidth: col.width,
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        whiteSpace: 'nowrap',
        width: col.width
      } as CSSProperties;
    }

    return {
      ...col.style,
      flexGrow: 0,
      flexShrink: 1,
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap'
    } as CSSProperties;
  }, []);

  const rowGroupHeaderTemplate = useCallback(
    (row: T) => {
      if (!props.groupRowsBy) {
        return <div />;
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
    },
    [props]
  );

  const multiselectHeader = () => {
    if (props.showSelections === true) {
      return <TriSelectShowSelection dataKey={props.id} />;
    }

    if (props.disableSelectAll === true) {
      return <div className="text-xs text-white text-500" />;
    }

    return (
      <div className="text-xs text-white text-500">
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
  };

  const rowReorderHeader = () => (
    <div className=" text-xs text-white text-500">
      <ResetButton
        disabled={state.prevDataSource === undefined}
        onClick={() => {
          if (state.prevDataSource !== undefined) {
            setters.setDataSource(state.prevDataSource);
            setters.setPrevDataSource(undefined);
          }
        }}
        tooltip="Reset Order"
      />
    </div>
  );

  const onSort = (event: DataTableStateEvent) => {
    if (!event.sortField || event.sortField === 'selected') {
      return;
    }

    const sortOrder = [1, 0, -1].includes(event.sortOrder ?? 1) ? event.sortOrder : 0;

    setters.setSortOrder(sortOrder ?? 1);

    // Try finding the column by field directly.
    let matchingColumn = props.columns.find((column) => column.field === event.sortField);

    // If not found, try finding by header, case-insensitively.
    if (!matchingColumn) {
      matchingColumn = props.columns.find((column) => column.header?.toLocaleLowerCase() === event.sortField.toLocaleLowerCase());
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

    if (props.selectedItemsKey === 'selectSelectedVideoStreamDtoItems') {
      // console.log(props.selectedItemsKey);
      // console.log(props.selectedItemsKey);
      // setters.setSelectSelectedItems([]);
    }

    setters.setFilters(newFilters);
  };

  const onSelectAllChange = (event: DataTableSelectAllChangeEvent) => {
    if (event.checked === undefined) {
      return;
    }

    const newSelectAll = event.checked;

    setters.setSelectAll(newSelectAll);
    if (newSelectAll === true) {
      setters.setSelectSelectedItems([]);
    }
    // props.onSelectAllChange?.(newSelectAll);

    if (newSelectAll && state.dataSource) {
      onSetSelection(state.dataSource, true);
    } else {
      onSetSelection([]);
    }
  };

  return (
    <div className="dataselector flex w-full min-w-full  justify-content-start align-items-center">
      <div className={`${props.className === undefined ? '' : props.className} min-h-full w-full surface-overlay`}>
        <DataTable
          cellSelection={false}
          dataKey={props.dataKey ?? 'id'}
          editMode="cell"
          emptyMessage={props.emptyMessage}
          expandableRowGroups={props.groupRowsBy !== undefined && props.groupRowsBy !== ''}
          expandedRows={state.expandedRows}
          exportFilename={props.exportFilename ?? 'streammaster'}
          filterDelay={500}
          filterDisplay={props.columns.some((a) => a.filter !== undefined) ? 'row' : undefined}
          filters={isEmptyObject(state.filters) ? getEmptyFilter(props.columns, state.showHidden) : state.filters}
          first={state.pagedInformation ? state.pagedInformation.first : state.first}
          header={sourceRenderHeader}
          lazy={props.dataSource === undefined}
          loading={props.isLoading === true || isFetching === true || isLoading === true}
          // metaKeySelection={false}
          onFilter={onFilter}
          onPage={onPage}
          onRowClick={(e) => props.onRowClick?.(e)}
          onRowReorder={(e) => {
            onRowReorder(e.value);
          }}
          onRowToggle={(e: DataTableRowToggleEvent) => setters.setExpandedRows(e.data as DataTableExpandedRows)}
          onSelectAllChange={props.reorderable || props.disableSelectAll === true ? undefined : onSelectAllChange}
          onSelectionChange={(e: DataTableSelectionMultipleChangeEvent<T[]> | DataTableSelectionSingleChangeEvent<T[]>) => {
            if (props.reorderable === true) {
              return;
            }
            onSelectionChange(e);
          }}
          onSort={onSort}
          paginator={props.enablePaginator ?? true}
          paginatorClassName="text-xs p-0 m-0 withpadding"
          paginatorTemplate="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
          ref={tableReference}
          removableSort={false}
          reorderableRows={props.reorderable}
          resizableColumns
          rowClassName={rowClass}
          rowGroupHeaderTemplate={rowGroupHeaderTemplate}
          rowGroupMode={props.groupRowsBy !== undefined && props.groupRowsBy !== '' ? 'subheader' : undefined}
          rows={state.rows}
          rowsPerPageOptions={[10, 25, 50, 100, 250]}
          scrollHeight="flex"
          scrollable
          selectAll={props.disableSelectAll === true ? undefined : state.selectAll}
          selection={state.selectSelectedItems}
          selectionMode={getSelectionMultipleMode}
          showGridlines
          showHeaders={props.showHeaders}
          sortField={props.reorderable ? 'rank' : state.sortField}
          sortMode="single"
          sortOrder={props.reorderable ? 0 : state.sortOrder}
          stateKey={`${props.id}-table`}
          showSelectAll={props.disableSelectAll !== true}
          stateStorage={props.enableState !== undefined && props.enableState !== true ? 'custom' : 'local'}
          stripedRows
          style={props.style}
          totalRecords={state.pagedInformation ? state.pagedInformation.totalItemCount : undefined}
          value={state.dataSource}
        >
          <Column
            className="max-w-2rem p-0 justify-content-center align-items-center"
            field="rank"
            header={rowReorderHeader}
            hidden={!props.reorderable}
            rowReorder
            style={{ width: '2rem' }}
          />
          <Column
            align="center"
            alignHeader="center"
            className={`justify-content-center align-items-center multiselect ${props.selectionMode}`}
            header={multiselectHeader}
            headerStyle={{ padding: '0px', width: '3rem' }}
            hidden={
              !props.showSelections &&
              props.selectionMode !== 'multiple' &&
              props.selectionMode !== 'checkbox' &&
              props.selectionMode !== 'multipleNoRowCheckBox'
            }
            resizeable={false}
            selectionMode="multiple"
          />
          {props.columns.map((col) => (
            <Column
              align={getAlign(col.align, col.fieldType)}
              alignHeader={getAlignHeader(col.align, col.fieldType)}
              className={col.className}
              body={(e) => (col.bodyTemplate ? col.bodyTemplate(e) : bodyTemplate(e, col.field, col.fieldType, setting.defaultIcon, col.camelize))}
              editor={col.editor}
              field={col.field}
              filter={getFilter(col.filter, col.fieldType)}
              filterElement={col.filterElement}
              // filterMenuStyle={{ width: '14rem' }}
              filterPlaceholder={col.fieldType === 'epg' ? 'EPG' : col.header ? col.header : camel2title(col.field)}
              header={getHeader(col.field, col.header, col.fieldType)}
              hidden={
                col.isHidden === true || (props.hideControls === true && getHeader(col.field, col.header, col.fieldType) === 'Actions') ? true : undefined
              }
              key={col.fieldType ? col.field + col.fieldType : col.field}
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
              style={getStyle(col)}
            />
          ))}
        </DataTable>
      </div>
    </div>
  );
};

DataSelector.displayName = 'dataselector';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
interface BaseDataSelectorProperties<T = any> {
  className?: string;
  columns: ColumnMeta[];
  defaultSortField: string;
  defaultSortOrder?: -1 | 0 | 1;
  emptyMessage?: ReactNode;
  enableExport?: boolean;
  enablePaginator?: boolean;
  enableState?: boolean;
  disableSelectAll?: boolean;
  exportFilename?: string;
  groupRowsBy?: string;
  headerLeftTemplate?: ReactNode;
  headerName?: string;
  headerRightTemplate?: ReactNode;
  hideControls?: boolean;
  id: string;
  isLoading?: boolean;
  dataKey?: string | undefined;

  // onLazyLoad?: (e: any) => void;
  onMultiSelectClick?: (value: boolean) => void;
  OnReset?: () => void;
  onRowClick?: (event: DataTableRowClickEvent) => void;
  onRowReorder?: (value: T[]) => void;
  onRowVisibleClick?: (value: T) => void;
  onSelectionChange?: (value: T[], selectAll: boolean) => void;
  // onValueChanged?: (value: T[]) => void;
  reorderable?: boolean;
  scrollTo?: number;
  selectedItemsKey: string;
  selectedStreamGroupId?: number;
  selectionMode?: DataSelectorSelectionMode;
  showHeaders?: boolean | undefined;
  showSelections?: boolean;
  showSelector?: boolean;
  sortField?: string;
  sortOrder?: number;
  style?: CSSProperties;
  reset?: boolean | undefined;
  videoStreamIdsIsReadOnly?: string[] | undefined;
  // virtualScrollHeight?: string | undefined;
}

type QueryFilterProperties<T> = BaseDataSelectorProperties<T> & {
  dataSource?: T[] | undefined;
  queryFilter: (filters: GetApiArgument | typeof skipToken) => ReturnType<QueryHook<PagedResponseDto<T> | T[]>>;
};

type DataSourceProperties<T> = BaseDataSelectorProperties<T> & {
  dataSource: T[] | undefined;
  queryFilter?: (filters: GetApiArgument | typeof skipToken) => ReturnType<QueryHook<PagedResponseDto<T> | T[]>>;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type DataSelectorProps<T = any> = DataSourceProperties<T> | QueryFilterProperties<T>;

export interface PagedTableInformation {
  first: number;
  pageNumber: number;
  pageSize: number;
  totalItemCount: number;
  totalPageCount: number;
}

export interface PagedDataDto<T> {
  data?: T[];
}

// eslint-disable-next-line @typescript-eslint/ban-types
export type PagedTableDto<T> = PagedDataDto<T> & PagedTableInformation & {};

export default memo(DataSelector);
