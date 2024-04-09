import StringTracker from '@components/inputs/StringTracker';
import { camel2title, isEmptyObject } from '@lib/common/common';

import useSettings from '@lib/useSettings';
import { areArraysEqual } from '@mui/base';
import { Button } from 'primereact/button';
import { Column, ColumnFilterElementTemplateOptions } from 'primereact/column';
import {
  DataTable,
  DataTableExpandedRows,
  DataTablePageEvent,
  DataTableRowToggleEvent,
  DataTableSelectionMultipleChangeEvent,
  DataTableSelectionSingleChangeEvent,
  DataTableStateEvent,
  type DataTableRowData,
  type DataTableValue
} from 'primereact/datatable';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';

import generateFilterData from '@components/dataSelector/generateFilterData';
import { PagedResponse } from '@lib/smAPI/smapiTypes';
import { Checkbox } from 'primereact/checkbox';
import TableHeader from './helpers/TableHeader';
import bodyTemplate from './helpers/bodyTemplate';
import { getAlign, getHeaderFromField, setColumnToggle } from './helpers/dataSelectorFunctions';
import getEmptyFilter from './helpers/getEmptyFilter';
import getHeader from './helpers/getHeader';
import getRecord from './helpers/getRecord';
import { getStyle } from './helpers/getStyle';
import isPagedResponse from './helpers/isPagedResponse';
import useSMDataSelectorValuesState from './hooks/useSMDataTableState';
import { useSetQueryFilter } from './hooks/useSetQueryFilter';
import { ColumnMeta } from './types/ColumnMeta';
import { SMDataTableProps } from './types/smDataTableInterfaces';

const SMDataTable = <T extends DataTableValue>(props: SMDataTableProps<T>) => {
  const { state, setters } = useSMDataSelectorValuesState<T>(props.id, props.selectedItemsKey);
  const tableReference = useRef<DataTable<T[]>>(null);

  const [, setIsExpanded] = useState<boolean>(false);

  const { queryFilter } = useSetQueryFilter(props.id, props.columns, state.first, state.filters, state.page, state.rows);
  const { data, isLoading } = props.queryFilter ? props.queryFilter(queryFilter) : { data: undefined, isLoading: false };

  useEffect(() => {
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

    if (data && isPagedResponse<T>(data)) {
      if (!state.dataSource || (state.dataSource && !areArraysEqual(data.Data, state.dataSource))) {
        setters.setDataSource((data as PagedResponse<T>).Data);

        if (state.selectAll && data !== undefined) {
          setters.setSelectSelectedItems((data as PagedResponse<T>).Data as T[]);
        }
      }

      if (data.PageNumber > 1 && data.TotalPageCount === 0) {
        const newData = { ...data };

        newData.PageNumber += -1;
        newData.First = (newData.PageNumber - 1) * newData.PageSize;
        setters.setPage(newData.PageNumber);
        setters.setFirst(newData.First);
        setters.setPagedInformation(newData);
      } else {
        setters.setPagedInformation(data);
      }
    }
  }, [data, setters, state.dataSource, state.selectAll]);

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
        props.onSelectionChange(selected, all);
      }

      return e;
    },
    [state.selectSelectedItems, state.selectAll, props, setters]
  );

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
    // setters.setDataSource(changed);
    props.onRowReorder?.(changed);
  };

  const setting = useSettings();

  const rowClass = useCallback(
    (data: DataTableRowData<T[]>): string => {
      const isHidden = getRecord(data as T, 'isHidden');

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (props.selectRow === true && state.selectSelectedItems !== undefined) {
        const id = getRecord(data, 'id') as number;
        if (state.selectSelectedItems.some((item) => item.id === id)) {
          return 'bg-orange-900';
        }
      }

      return '';
    },
    [props.selectRow, state.selectSelectedItems]
  );

  const onColumnToggle = useCallback(
    (selectedColumns: ColumnMeta[]) => {
      const newData = setColumnToggle(props.columns, selectedColumns);
      setters.setVisibleColumns(newData);
    },
    [props.columns, setters]
  );

  const visibleColumnsTemplate = useCallback(
    (option: ColumnMeta) => {
      if (option === undefined || option === undefined) {
        return <div />;
      }
      const header = getHeaderFromField(option.field, props.columns);
      return <div>{header}</div>;
    },
    [props.columns]
  );

  const getSortIcon = useCallback(
    (field: string) => {
      if (state.sortField !== field || state.sortOrder === 0) {
        return 'pi pi-sort-alt';
      }
      return state.sortOrder === 1 ? 'pi pi-sort-amount-up' : 'pi pi-sort-amount-down';
    },
    [state.sortField, state.sortOrder]
  );

  const getFilterElement = useCallback(
    (header: string, options: ColumnFilterElementTemplateOptions) => {
      const stringTrackerkey = props.id + '-' + options.field;

      return (
        <div className="w-full">
          <StringTracker
            id={stringTrackerkey}
            onChange={async (e) => {
              options.filterApplyCallback(e);
            }}
            placeholder={header ?? ''}
            value={options.value}
          />
        </div>
      );
    },
    [props.id]
  );

  const sortButton = useCallback(
    (options: ColumnFilterElementTemplateOptions) => {
      return (
        <Button
          className="p-button-text p-0 m-0"
          onClick={() => {
            setters.setSortField(options.field);
            setters.setSortOrder(state.sortOrder === 1 ? -1 : 1);
          }}
          icon={getSortIcon(options.field)}
        />
      );
    },
    [getSortIcon, setters, state.sortOrder]
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
              checkbox: { style: { display: 'none' } },
              header: { style: { display: 'none' } }
            }}
            useOptionAsValue
          />
        );
      }

      let cl = 'sm-col-header-center';
      let justify = 'justify-content-center';

      if (col.align !== undefined) {
        cl = 'sm-col-header-' + col.align;
      }

      if (col.alignHeader !== undefined) {
        justify = 'justify-content-' + col.alignHeader;
      }

      if (props.enableHeaderWrap === true) {
        cl += '-wrap';
      }

      return (
        <div className={`flex ${justify} align-items-center gap-1`}>
          {col.filter !== true && <div className={cl}>{header}</div>}
          {col.filter === true && getFilterElement(header, options)}
          {col.sortable === true && sortButton(options)}
        </div>
      );
    },
    [props.columns, props.enableHeaderWrap, getFilterElement, sortButton, state.visibleColumns, visibleColumnsTemplate, onColumnToggle]
  );

  const showSelection = useMemo(() => {
    return props.showSelections || props.selectionMode === 'multiple' || props.selectionMode === 'checkbox' || props.selectionMode === 'multipleNoRowCheckBox';
  }, [props.selectionMode, props.showSelections]);

  const exportCSV = () => {
    tableReference.current?.exportCSV({ selectionOnly: false });
  };

  const onPage = (event: DataTablePageEvent) => {
    const adjustedPage = (event.page ?? 0) + 1;

    setters.setPage(adjustedPage);
    setters.setFirst(event.first);
    setters.setRows(event.rows);
  };

  const onFilter = (event: DataTableStateEvent) => {
    const newFilters = generateFilterData(props.columns, event.filters);

    setters.setFilters(newFilters);
  };

  const sourceRenderHeader = useMemo(() => {
    if (props.noSourceHeader === true) {
      return null;
    }

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

  const addSelection = useCallback(
    (data: T) => {
      const newSelectedItems = [...state.selectSelectedItems, data];
      setters.setSelectSelectedItems(newSelectedItems);
    },
    [setters, state.selectSelectedItems]
  );

  const removeSelection = useCallback(
    (data: T) => {
      const newSelectedItems = state.selectSelectedItems.filter((predicate) => predicate.Id !== data.Id);
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

  function selectionHeaderTemplate() {
    const isSelected = false;

    if (!isSelected) {
      return (
        <div className="flex justify-content-center align-items-center p-0 m-0">
          {showSelection && <Checkbox checked={state.selectAll} onChange={() => toggleAllSelection()} />}
        </div>
      );
    }

    return (
      <div className="flex justify-content-center align-items-center p-0 m-0">
        {showSelection && <Checkbox checked={state.selectAll} onChange={() => toggleAllSelection()} />}
      </div>
    );
  }

  const selectionTemplate = useCallback(
    (data: T) => {
      if (!showSelection) {
        return <div />;
      }

      const found = state.selectSelectedItems.some((item) => item.Id === data.Id);
      const isSelected = found ?? false;

      if (isSelected) {
        return (
          <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
            {showSelection && <Checkbox checked={isSelected} className="pl-1" onChange={() => removeSelection(data)} />}
          </div>
        );
      }

      return (
        <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
          {showSelection && <Checkbox checked={isSelected} className="pl-1" onChange={() => addSelection(data)} />}
        </div>
      );
    },
    [addSelection, removeSelection, showSelection, state.selectSelectedItems]
  );

  // const rowReorderHeader = () => (
  //   <div className=" text-xs text-white text-500">
  //     <ResetButton
  //       disabled={state.prevDataSource === undefined}
  //       onClick={() => {
  //         if (state.prevDataSource !== undefined) {
  //           setters.setDataSource(state.prevDataSource);
  //           setters.setPrevDataSource(undefined);
  //         }
  //       }}
  //       tooltip="Reset Order"
  //     />
  //   </div>
  // );

  return (
    <div
      id={props.id}
      onClick={(event: any) => {
        if (props.enableClick !== true) {
          return;
        }
        const target = event.target as HTMLDivElement;
        if (props.showExpand === true || props.rowExpansionTemplate !== undefined) {
          // console.log(target.className);
          if (target.className && target.className === 'p-datatable-wrapper') {
            setters.setExpandedRows(undefined);
          }
        }
        props.onClick?.(event);
      }}
      className=""
    >
      {/* <div className={`${props.className === undefined ? '' : props.className} h-full min-h-full w-full surface-overlay`}> */}

      <div className="h-full min-h-full w-full surface-overlay">
        <DataTable
          id={props.id}
          dataKey="Id"
          cellSelection={false}
          editMode="cell"
          filterDisplay="row"
          expandedRows={state.expandedRows}
          filters={isEmptyObject(state.filters) ? getEmptyFilter(props.columns, state.showHidden) : state.filters}
          first={state.pagedInformation ? state.pagedInformation.First : state.first}
          loading={props.isLoading === true || isLoading === true}
          onRowReorder={(e) => {
            onRowReorder(e.value);
          }}
          header={sourceRenderHeader}
          lazy
          onRowToggle={(e: DataTableRowToggleEvent) => {
            setters.setExpandedRows(e.data as DataTableExpandedRows);
          }}
          onSelectionChange={(e: DataTableSelectionMultipleChangeEvent<T[]> | DataTableSelectionSingleChangeEvent<T[]>) => {
            if (props.reorderable === true) {
              return;
            }
            onSelectionChange(e);
          }}
          onFilter={onFilter}
          onPage={onPage}
          onRowClick={props.selectRow === true ? props.onRowClick : undefined}
          paginator={props.enablePaginator === true}
          paginatorClassName="text-xs"
          paginatorTemplate="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
          ref={tableReference}
          rowClassName={props.rowClass ? props.rowClass : rowClass}
          rowExpansionTemplate={props.rowExpansionTemplate}
          rows={state.rows}
          rowsPerPageOptions={[10, 25, 50, 100, 250]}
          scrollHeight="flex"
          scrollable
          showGridlines
          size="small"
          sortField={props.reorderable ? 'rank' : state.sortField}
          sortMode="single"
          sortOrder={props.reorderable ? 0 : state.sortOrder}
          stripedRows
          style={props.style}
          value={state.dataSource}
          reorderableRows={props.reorderable}
          totalRecords={state.pagedInformation ? state.pagedInformation.TotalItemCount : undefined}
          onRowCollapse={(e) => {
            setIsExpanded(false);
            props.onRowCollapse?.(e);
          }}
          onRowExpand={(e) => {
            setIsExpanded(true);
            props.onRowExpand?.(e);
          }}
        >
          <Column
            body={props.addOrRemoveTemplate}
            className="w-2rem max-w-2rem p-0 justify-content-center align-items-center"
            field="addOrRemove"
            filter
            filterElement={props.addOrRemoveHeaderTemplate}
            hidden={!props.addOrRemoveTemplate}
            showFilterMenu={false}
            showFilterOperator={false}
            resizeable={false}
          />
          <Column
            body={showSelection ? selectionTemplate : undefined}
            className="w-2rem max-w-2rem p-0 justify-content-center align-items-center"
            field="multiSelect"
            filter
            filterElement={selectionHeaderTemplate}
            showClearButton={false}
            showFilterMatchModes={false}
            showFilterMenu={false}
            showFilterOperator={false}
            hidden={!showSelection}
            style={{ maxWidth: '2rem', width: '2rem' }}
          />

          <Column
            className={
              showSelection
                ? 'w-3rem max-w-3rem p-0 justify-content-center align-items-center sm-expander'
                : 'w-2rem max-w-2rem p-0 justify-content-center align-items-center sm-expander'
            }
            hidden={!props.showExpand || props.rowExpansionTemplate === undefined}
            showFilterMenu={false}
            showFilterOperator={false}
            resizeable={false}
            expander
          />
          <Column
            className="max-w-2rem p-0 justify-content-center align-items-center"
            field="rank"
            // header={rowReorderHeader}
            hidden={!props.reorderable}
            rowReorder
            style={{ maxWidth: '2rem', width: '2rem' }}
          />
          {props.columns &&
            props.columns
              .filter((col) => col.removed !== true)
              .map((col) => (
                <Column
                  align={getAlign(col.align, col.fieldType)}
                  // alignHeader={col.alignHeader}
                  // className={'sm-column ' + col.className}
                  filter
                  filterElement={rowFilterTemplate}
                  filterPlaceholder={col.filter === true ? (col.fieldType === 'epg' ? 'EPG' : col.header ? col.header : camel2title(col.field)) : undefined}
                  header={getHeader(col.field, col.header, col.fieldType)}
                  body={(e) => (col.bodyTemplate ? col.bodyTemplate(e) : bodyTemplate(e, col.field, col.fieldType, setting.defaultIcon, col.camelize))}
                  editor={col.editor}
                  field={col.field}
                  hidden={col.isHidden === true} //|| getHeader(col.field, col.header, col.fieldType) === 'Actions' ? true : undefined}
                  key={col.fieldType ? col.field + col.fieldType : col.field}
                  style={getStyle(col, col.noAutoStyle !== true || col.bodyTemplate !== undefined)}
                  showAddButton
                  showApplyButton
                  showClearButton
                  showFilterMatchModes
                  showFilterMenu={col.filter === true}
                  showFilterMenuOptions
                  showFilterOperator
                  sortable={props.reorderable ? false : col.sortable}
                />
              ))}
        </DataTable>
      </div>
    </div>
  );
};

SMDataTable.displayName = 'dataselectorvalues';

export default memo(SMDataTable);
