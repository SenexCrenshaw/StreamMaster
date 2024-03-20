import { camel2title, isEmptyObject } from '@lib/common/common';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import useSettings from '@lib/useSettings';
import { areArraysEqual } from '@mui/base';
import { Button } from 'primereact/button';
import { Column, ColumnFilterElementTemplateOptions } from 'primereact/column';
import {
  DataTable,
  DataTableExpandedRows,
  DataTableRowExpansionTemplate,
  type DataTablePageEvent,
  type DataTableRowClickEvent,
  type DataTableRowData,
  type DataTableSelectAllChangeEvent,
  type DataTableSelectionMultipleChangeEvent,
  type DataTableSelectionSingleChangeEvent,
  type DataTableStateEvent,
  type DataTableValue
} from 'primereact/datatable';
import { Suspense, memo, useCallback, useEffect, useMemo, useRef, type CSSProperties, type ReactNode } from 'react';

import { type ColumnAlign, type ColumnFieldType, type ColumnMeta, type DataSelectorSelectionMode } from './DataSelectorTypes';
import bodyTemplate from './bodyTemplate';
import generateFilterData from './generateFilterData';
import getEmptyFilter from './getEmptyFilter';
import getHeader from './getHeader';
import getRecord from './getRecord';
import isPagedTableDto from './isPagedTableDto';
import useDataSelectorState2 from './useDataSelectorState2';

import AddButton from '@components/buttons/AddButton';
import StringTracker from '@components/inputs/StringTracker';

import SMStreamDataSelectorValue from '@features/streameditor/SMStreamDataSelectorValue';
import { GetApiArgument, PagedResponse, QueryHook, SMChannelDto, SMStreamDto, SMStreamSMChannelRequest } from '@lib/apiDefs';
import { PagedResponseDto } from '@lib/common/dataTypes';
import { Checkbox } from 'primereact/checkbox';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
import ResetButton from '../buttons/ResetButton';
import TableHeader from './TableHeader';
import getRecordString from './getRecordString';
import { useSetQueryFilter } from './useSetQueryFilter';

const DataSelector2 = <T extends DataTableValue>(props: DataSelector2Props<T>) => {
  const debug = false;
  const { state, setters } = useDataSelectorState2<T>(props.id, props.selectedItemsKey, props.selectedSMChannelKey, props.selectedSMStreamKey);

  useEffect(() => {
    if (props.columns === undefined) {
      return;
    }

    if (state.visibleColumns !== null) {
      return;
    }
    setters.setVisibleColumns(props.columns);
  }, [props.columns, setters, state.visibleColumns]);

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

  const { data, isLoading } = props.queryFilter ? props.queryFilter(queryFilter) : { data: undefined, isLoading: false };

  const onSetSelection = useCallback(
    (e: T | T[], overRideSelectAll?: boolean): T | T[] | undefined => {
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

  useEffect(() => {
    if (!data) {
      return;
    }

    if (data && data.data && isPagedTableDto<T>(data)) {
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
  }, [data, debug, props.id, setters, state.dataSource, state.selectAll]);

  const onRowReorder = (changed: T[]) => {
    setters.setDataSource(changed);

    if (state.prevDataSource === undefined) {
      setters.setPrevDataSource(changed);
    }

    props.onRowReorder?.(changed);
  };

  const rowClass = useCallback(
    (data: DataTableRowData<T[]>) => {
      // const isLoading2 = getRecord(changed as T, 'isLoading');

      const isHidden = getRecord(data as T, 'isHidden');

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (props.selectRow === true && state.selectedSMChannel !== undefined) {
        const id = getRecord(data, 'id') as number;
        if (id === state.selectedSMChannel.id) {
          return 'bg-orange-900';
        }
      }

      if (props.selectRow === true && state.selectedSMStream !== undefined) {
        const id = getRecordString(data, 'id');
        if (id === state.selectedSMStream.id) {
          return 'bg-yellow-900';
        }
      }

      if (props.videoStreamIdsIsReadOnly !== undefined && props.videoStreamIdsIsReadOnly.length > 0) {
        const isReadOnly = props.videoStreamIdsIsReadOnly.find((vs) => vs === getRecord(data as T, 'id'));

        if (isReadOnly !== undefined) {
          return 'videostreamSelected';
        }
      }

      return {};
    },
    [props.videoStreamIdsIsReadOnly, state.selectedSMChannel, state.selectedSMStream]
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
    scroller?.scrollToIndex(props.scrollTo ?? 0, 'smooth');
  }, [props.scrollTo]);

  const sourceRenderHeader = useMemo(() => {
    return (
      <TableHeader
        dataSelectorProps={props}
        enableExport={props.enableExport ?? true}
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
    if (props.selectionMode === 'multiple') {
      return 'checkbox';
    }

    return 'single';
  }, [props.selectionMode]);

  const onSelectionChange = useCallback(
    (e: DataTableSelectionMultipleChangeEvent<T[]> | DataTableSelectionSingleChangeEvent<T[]>) => {
      if (e.value === null || e.value === undefined || !Array.isArray(e.value)) {
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

    if (newSelectAll && state.dataSource) {
      onSetSelection(state.dataSource, true);
    } else {
      onSetSelection([]);
    }
  };

  const getSortIcon = useCallback(
    (field: string) => {
      if (state.sortField !== field || state.sortOrder === 0) {
        return 'pi pi-sort-alt';
      }

      return state.sortOrder === 1 ? 'pi pi-sort-amount-up' : 'pi pi-sort-amount-down';
    },
    [state.sortField, state.sortOrder]
  );

  const getHeaderFromField = useCallback(
    (field: string): string => {
      let col = props.columns.find((column) => column.field === field);
      let header = '';

      if (col) {
        header = getHeader(col.field, col.header, col.fieldType) as string;
      }
      return header;
    },
    [props.columns]
  );

  const visibleColumnsTemplate = useCallback(
    (option: ColumnMeta) => {
      if (option === undefined || option === undefined) {
        return <div />;
      }
      const header = getHeaderFromField(option.field);
      return <div>{header}</div>;
    },
    [getHeaderFromField]
  );

  const onColumnToggle = useCallback(
    (selectedColumns: ColumnMeta[]) => {
      const newData = [...props.columns];

      newData
        .filter((col) => col.removable === true)
        .forEach((col) => {
          col.removed = true;
        });

      if (selectedColumns !== undefined) {
        selectedColumns.forEach((col) => {
          let propscol = newData.find((column) => column.field === col.field);
          if (propscol) {
            console.log('Setting ' + propscol.field + ' false');
            propscol.removed = false;
          }
        });
      }

      setters.setVisibleColumns(newData);
    },
    [props.columns, setters]
  );

  const rowFilterTemplate = useCallback(
    (options: ColumnFilterElementTemplateOptions) => {
      let col = props.columns.find((column) => column.field === options.field);
      let header = '';

      if (col) {
        header = getHeader(col.field, col.header, col.fieldType) as string;
      } else {
        return <div />;
      }

      if (col.fieldType === 'actions') {
        let selectedCols = [] as ColumnMeta[];

        let cols = props.columns.filter((col) => col.removable === true); //.map((col) => col.field);
        if (cols.length > 0 && state.visibleColumns !== null && state.visibleColumns !== undefined) {
          selectedCols = state.visibleColumns.filter((col) => col.removable === true && col.removed !== true);
        } else {
          return <div />;
        }

        return (
          <MultiSelect
            className="multiselectcolumn"
            value={selectedCols}
            options={cols}
            optionLabel="field"
            itemTemplate={visibleColumnsTemplate}
            selectedItemTemplate={visibleColumnsTemplate}
            maxSelectedLabels={0}
            onChange={(e: MultiSelectChangeEvent) => {
              onColumnToggle(e.value);
            }}
            showSelectAll={false}
            pt={{
              header: { style: { display: 'none' } },
              checkbox: { style: { display: 'none' } }
            }}
            useOptionAsValue
          />
        );
      }

      if (col.filter !== true) {
        return <div>{header}</div>;
      }

      const stringTrackerkey = props.id + '-' + options.field;

      return (
        <div className="flex justify-content-end">
          <StringTracker
            id={stringTrackerkey}
            onChange={async (e) => {
              options.filterApplyCallback(e);
            }}
            placeholder={header ?? ''}
            value={options.value}
          />

          <Button
            className="p-button-text p-0 m-0"
            onClick={() => {
              setters.setSortField(options.field);
              setters.setSortOrder(state.sortOrder === 1 ? -1 : 1);
            }}
            icon={getSortIcon(options.field)}
          />
        </div>
      );
    },
    [getSortIcon, onColumnToggle, props.columns, props.id, setters, state.sortOrder, state.visibleColumns, visibleColumnsTemplate]
  );

  const addSelection = useCallback(
    (data: T) => {
      const newSelectedItems = [...state.selectSelectedItems, data];
      setters.setSelectSelectedItems(newSelectedItems);
    },
    [setters, state.selectSelectedItems]
  );

  function toggleAllSelection() {
    if (state.selectAll) {
      setters.setSelectAll(false);
      setters.setSelectSelectedItems([]);
    } else {
      setters.setSelectAll(true);
      setters.setSelectSelectedItems(state.dataSource ?? []);
    }
  }

  const showSelection = useMemo(() => {
    return props.showSelections || props.selectionMode === 'multiple' || props.selectionMode === 'checkbox' || props.selectionMode === 'multipleNoRowCheckBox';
  }, [props.selectionMode, props.showSelections]);

  const addOrRemoveTemplate = useCallback(
    (data: T) => {
      const found = state.selectSelectedItems.some((item) => item.id === data.id);
      const isSelected = found ?? false;
      let toolTip = 'Add Channel';
      if (state.selectedSMChannel !== undefined) {
        toolTip = 'Add Stream To ' + state.selectedSMChannel.name;
        return (
          <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
            {props.onStreamAdd !== undefined && (
              <AddButton
                iconFilled={false}
                onClick={() => props.onStreamAdd?.({ smStreamId: data.id, smChannelId: state.selectedSMChannel?.id ?? 0 })}
                tooltip={toolTip}
              />
            )}
            {showSelection && <Checkbox checked={isSelected} className="pl-1" onChange={() => addSelection(data)} />}
          </div>
        );
      }

      return (
        <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
          {props.onChannelAdd !== undefined && <AddButton iconFilled={false} onClick={() => props.onChannelAdd?.(data)} tooltip={toolTip} />}
          {showSelection && <Checkbox checked={isSelected} className="pl-1" onChange={() => addSelection(data)} />}
        </div>
      );
    },
    [addSelection, props, showSelection, state.selectSelectedItems, state.selectedSMChannel]
  );

  function addOrRemoveHeaderTemplate() {
    const isSelected = false;

    if (!isSelected) {
      return (
        <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
          {/* <AddButton iconFilled={false} onClick={() => console.log('AddButton')} tooltip="Add All Channels" /> */}
          {showSelection && <Checkbox checked={state.selectAll} className="pl-1" onChange={() => toggleAllSelection()} />}
        </div>
      );
    }

    return (
      <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
        <AddButton iconFilled={false} onClick={() => console.log('AddButton')} />
        {showSelection && <Checkbox checked={state.selectAll} className="pl-1" onChange={() => toggleAllSelection()} />}
      </div>
    );
  }

  const rowExpansionTemplate = useCallback(
    (data: DataTableRowData<T[]>, options: DataTableRowExpansionTemplate) => {
      return (
        <div className="border-2 border-round-lg border-200 ml-3 m-1">
          <Suspense fallback={<div>Loading...</div>}>
            <SMStreamDataSelectorValue
              selectedSMChannelKey={props.selectedSMChannelKey}
              data={data.smStreams}
              isLoading={isLoading}
              id={data.id + '-streams'}
            />
          </Suspense>
        </div>
      );
    },
    [isLoading, props.selectedSMChannelKey]
  );

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
          lazy
          loading={props.isLoading === true || isLoading === true}
          // metaKeySelection={false}
          onFilter={onFilter}
          onPage={onPage}
          onRowExpand={(e: any) => {
            setters.setSelectedSMChannel(e.originalEvent.data as SMChannelDto);
          }}
          onRowCollapse={(e: any) => {
            setters.setSelectedSMChannel(undefined);
          }}
          onRowClick={(e) => {
            if (props.selectRow === true) {
              const isChannel = 'smStreams' in e.data;

              if (isChannel) {
                if (state.selectedSMChannel !== undefined && e.data !== undefined && e.data.id === state.selectedSMChannel.id) {
                  setters.setSelectedSMChannel(undefined);
                } else {
                  setters.setSelectedSMChannel(e.data as SMChannelDto);
                }
              } else {
                if (state.selectedSMStream !== undefined && e.data !== undefined && e.data.id === state.selectedSMStream.id) {
                  setters.setSelectedSMStream(undefined);
                } else {
                  setters.setSelectedSMStream(e.data as SMStreamDto);
                }
              }
            }
            props.onRowClick?.(e);
          }}
          onRowReorder={(e) => {
            onRowReorder(e.value);
          }}
          onRowToggle={(e: any) => {
            setters.setExpandedRows(e.data as DataTableExpandedRows);
          }}
          onSelectAllChange={props.reorderable || props.disableSelectAll === true ? undefined : onSelectAllChange}
          onSelectionChange={(e: DataTableSelectionMultipleChangeEvent<T[]> | DataTableSelectionSingleChangeEvent<T[]>) => {
            if (props.reorderable === true) {
              return;
            }
            onSelectionChange(e);
          }}
          rowExpansionTemplate={rowExpansionTemplate}
          onSort={onSort}
          paginator={props.enablePaginator ?? true}
          paginatorClassName="text-xs p-0 m-0 withpadding"
          paginatorTemplate="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
          ref={tableReference}
          removableSort={false}
          reorderableRows={props.reorderable}
          resizableColumns
          rowClassName={rowClass}
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
          // stateKey={`${props.id}-table`}
          showSelectAll={props.disableSelectAll !== true}
          stateStorage="custom"
          // stateStorage={props.enableState !== undefined && props.enableState !== true ? 'custom' : 'local'}
          stripedRows
          style={props.style}
          totalRecords={state.pagedInformation ? state.pagedInformation.totalItemCount : undefined}
          value={state.dataSource}
          // pt={{
          //   headerRow: { style: { display: 'none' } }
          // }}
        >
          <Column
            body={addOrRemoveTemplate}
            className={
              showSelection
                ? 'w-3rem max-w-3rem p-0 justify-content-center align-items-center'
                : 'w-2rem max-w-2rem p-0 justify-content-center align-items-center'
            }
            field="addOrRemove"
            filter
            filterElement={addOrRemoveHeaderTemplate}
            hidden={!props.onStreamAdd || !props.onChannelAdd}
            showFilterMenu={false}
            showFilterOperator={false}
            resizeable={false}
            // style={{ width: '3rem', maxWidth: '3rem' }}
          />
          <Column
            // body={ExpandTemplate}
            className={
              showSelection
                ? 'w-3rem max-w-3rem p-0 justify-content-center align-items-center'
                : 'w-2rem max-w-2rem p-0 justify-content-center align-items-center'
            }
            // field="expand"
            // filter
            // filterElement={<div />}

            hidden={!props.showExpand}
            showFilterMenu={false}
            showFilterOperator={false}
            resizeable={false}
            expander
            // style={{ width: '3rem', maxWidth: '3rem' }}
          />
          <Column
            className="max-w-2rem p-0 justify-content-center align-items-center"
            field="rank"
            header={rowReorderHeader}
            hidden={!props.reorderable}
            rowReorder
            style={{ width: '2rem', maxWidth: '2rem' }}
          />
          {state.visibleColumns &&
            state.visibleColumns
              .filter((col) => col.removed !== true)
              .map((col) => (
                <Column
                  align={getAlign(col.align, col.fieldType)}
                  alignHeader={getAlignHeader(col.align, col.fieldType)}
                  className={col.className}
                  body={(e) => (col.bodyTemplate ? col.bodyTemplate(e) : bodyTemplate(e, col.field, col.fieldType, setting.defaultIcon, col.camelize))}
                  editor={col.editor}
                  field={col.field}
                  filter //={getFilter(col.filter, col.fieldType)}
                  filterElement={col.filterElement ?? rowFilterTemplate}
                  // filterMenuStyle={{ width: '14rem' }}
                  filterPlaceholder={col.fieldType === 'epg' ? 'EPG' : col.header ? col.header : camel2title(col.field)}
                  // header={getHeader(col.field, col.header, col.fieldType)}
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
                  showFilterMenu={col.filterElement === undefined && col.filter === true}
                  showFilterMenuOptions
                  showFilterOperator
                  // sortable={props.reorderable ? false : col.sortable}
                  style={getStyle(col)}
                />
              ))}
        </DataTable>
      </div>
    </div>
  );
};

DataSelector2.displayName = 'dataselector2';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
interface BaseDataSelector2Properties<T = any> {
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
  selectRow?: boolean;
  id: string;
  isLoading?: boolean;
  dataKey?: string | undefined;
  // setSMChannel?: boolean;
  // setSMStream?: boolean;
  selectedSMChannelKey: string;
  selectedSMStreamKey: string;
  // onLazyLoad?: (e: any) => void;
  onChannelAdd?: (value: T) => void;
  onStreamAdd?: (value: SMStreamSMChannelRequest) => void;
  onDelete?: (value: T) => void;
  onMultiSelectClick?: (value: boolean) => void;
  OnReset?: () => void;
  onRowClick?: (event: DataTableRowClickEvent) => void;
  onRowReorder?: (value: T[]) => void;
  onRowVisibleClick?: (value: T) => void;
  onSelectionChange?: (value: T[], selectAll: boolean) => void;
  // onValueChanged?: (value: T[]) => void;
  reorderable?: boolean;
  showExpand?: boolean;
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

type QueryFilterProperties<T> = BaseDataSelector2Properties<T> & {
  queryFilter: (params: GetApiArgument | undefined) => ReturnType<QueryHook<PagedResponse<T> | undefined>>;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type DataSelector2Props<T = any> = QueryFilterProperties<T>;

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

export default memo(DataSelector2);
