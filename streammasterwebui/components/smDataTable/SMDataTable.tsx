import StringTracker from '@components/inputs/StringTracker';
import SMButton from '@components/sm/SMButton';
import { SMTriSelectShowHidden } from '@components/sm/SMTriSelectShowHidden';
import { SMTriSelectShowSelect } from '@components/sm/SMTriSelectShowSelect';
import { SMTriSelectShowSelected } from '@components/sm/SMTriSelectShowSelected';
import generateFilterData from '@components/smDataTable/helpers/generateFilterData';
import { camel2title, isEmptyObject } from '@lib/common/common';
import { useSMContext } from '@lib/signalr/SMProvider';
import { PagedResponse } from '@lib/smAPI/smapiTypes';
import { Checkbox } from 'primereact/checkbox';
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
import TableHeader from './helpers/TableHeader';
import bodyTemplate from './helpers/bodyTemplate';
import { getAlign, getHeaderFromField, setColumnToggle } from './helpers/dataSelectorFunctions';
import { getColumnStyles } from './helpers/getColumnStyles';
import getEmptyFilter from './helpers/getEmptyFilter';
import getHeader from './helpers/getHeader';
import getRecord from './helpers/getRecord';
import isPagedResponse from './helpers/isPagedResponse';
import useSMDataSelectorValuesState from './hooks/useSMDataTableState';
import { useSetQueryFilter } from './hooks/useSetQueryFilter';
import { ColumnMeta } from './types/ColumnMeta';
import { SMDataTableProps } from './types/smDataTableInterfaces';

const SMDataTable = <T extends DataTableValue>(props: SMDataTableProps<T>) => {
  const { state, setters } = useSMDataSelectorValuesState<T>(props.id, props.selectedItemsKey);
  const { settings } = useSMContext();
  const wrapperRef = useRef<HTMLDivElement>(null);
  const [, setHasScrollbar] = useState(false);
  const tableReference = useRef<DataTable<T[]>>(null);
  const [, setIsExpanded] = useState<boolean>(false);
  const { queryFilter } = useSetQueryFilter(props.id, props.columns, state.first, state.filters, state.page, state.rows);
  const { data, isLoading } = props.queryFilter ? props.queryFilter(queryFilter) : { data: undefined, isLoading: false };
  const [dataSource, setDataSource] = useState<T[]>([]);

  useEffect(() => {
    const observer = new MutationObserver((mutations) => {
      mutations.forEach((mutation) => {
        if (mutation.type === 'childList' || mutation.type === 'attributes') {
          checkScrollbar();
        }
      });
    });

    const checkScrollbar = () => {
      const element = wrapperRef.current?.querySelector('.p-datatable-wrapper');
      if (element) {
        setHasScrollbar(element.scrollHeight > element.clientHeight);
      }
    };

    if (wrapperRef.current) {
      observer.observe(wrapperRef.current, {
        attributeFilter: ['style', 'class'],
        attributes: true,
        childList: true,
        subtree: true
      });
    }

    // Run initial check
    checkScrollbar();

    return () => {
      observer.disconnect();
    };
  }, []);

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

  const onSetSelection = useCallback(
    (e: T | T[], overRideSelectAll?: boolean): T | T[] | undefined => {
      let selected: T[] = Array.isArray(e) ? e : [e];

      if (state.selectedItems === selected) {
        return;
      }

      if (props.selectionMode === 'single') {
        selected = selected.slice(0, 1);
      }

      setters.setSelectedItems(selected);
      const all = overRideSelectAll || state.selectAll;

      if (props.onSelectionChange) {
        props.onSelectionChange(selected, all);
      }

      return e;
    },
    [state.selectedItems, state.selectAll, props, setters]
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

  const onRowReorder = (changed: T[]) => {
    props.onRowReorder?.(changed);
  };

  const rowClass = useCallback(
    (data: DataTableRowData<T[]>): string => {
      const isHidden = getRecord(data as T, 'IsHidden');

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (props.selectRow === true && state.selectedItems !== undefined) {
        const id = getRecord(data, 'Id') as number;
        if (state.selectedItems.some((item) => item.Id === id)) {
          return 'channel-row-selected';
        }
      }

      return '';
    },
    [props.selectRow, state.selectedItems]
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
        return 'pi-sort-alt';
      }
      return state.sortOrder === 1 ? 'pi-sort-amount-down-alt' : 'pi-sort-amount-up';
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
            isLoading={isLoading}
            onChange={async (e) => {
              options.filterApplyCallback(e);
            }}
            placeholder={header ?? ''}
            value={options.value}
          />
        </div>
      );
    },
    [isLoading, props.id]
  );

  const sortButton = useCallback(
    (options: ColumnFilterElementTemplateOptions) => {
      return (
        <SMButton
          icon={getSortIcon(options.field)}
          onClick={() => {
            setters.setSortField(options.field);
            setters.setSortOrder(state.sortOrder === 1 ? -1 : 1);
          }}
        />
      );
    },
    [getSortIcon, setters, state.sortOrder]
  );

  const colFilterTemplate = useCallback(
    (options: ColumnFilterElementTemplateOptions) => {
      let col = props.columns.find((column) => column.field === options.field);
      let header = '';

      if (col) {
        header = getHeader(col.field, col.header, col.fieldType) as string;
      } else {
        return <div />;
      }

      let prefix = 'sm-col-header-';
      if (props.headerSize === 'small') {
        prefix = 'sm-col-small-header-';
      }

      if (props.headerSize === 'large') {
        prefix = 'sm-col-large-header-';
      }

      let cl = prefix + 'center';
      let justify = 'justify-content-center';

      if (col.align !== undefined) {
        cl = prefix + col.align;
      }

      if (col.alignHeader !== undefined) {
        justify = 'justify-content-' + col.alignHeader;
      }

      if (props.enableHeaderWrap === true) {
        cl += '-wrap';
      }

      if (col.filterElement) {
        if (col.sortable === true) {
          return (
            <div className={`flex flex-row ${justify} max-w-full flex-grow-0 overflow-hidden`}>
              <div className="max-w-full w-full flex-grow-0 overflow-hidden text-overflow-ellipsis">{col.filterElement(options)}</div>
              {col.sortable === true && (
                <div className="max-w-full flex-grow-0 overflow-hidden text-overflow-ellipsis" style={{ paddingLeft: '0.16rem', width: '1.84rem' }}>
                  {sortButton(options)}
                </div>
              )}
            </div>
          );
        }

        return (
          <div className={`flex flex-row ${justify} max-w-full flex-grow-0 overflow-hidden`}>
            <div className="max-w-full flex-grow-0 overflow-hidden text-overflow-ellipsis">{col.filterElement(options)}</div>
          </div>
        );
      }

      if (col.fieldType === 'actions') {
        if (props.actionHeaderTemplate) {
          return <div className={`flex ${justify} align-items-center align-content-center`}>{props.actionHeaderTemplate}</div>;
        }

        let selectedCols = [] as ColumnMeta[];

        let cols = props.columns.filter((col) => col.removable === true); //.map((col) => col.field);
        if (cols.length > 0 && state.visibleColumns !== null && state.visibleColumns !== undefined) {
          selectedCols = state.visibleColumns.filter((col) => col.removable === true && col.removed !== true);

          return (
            <MultiSelect
              className="multiselectcolumn w-full input-height-with-no-borders"
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

        return (
          <div className={`flex ${justify} align-items-center align-content-center input-height-with-no-borders`}>
            <div className={cl}>{header}</div>
          </div>
        );
      }

      return (
        <div className={`flex ${justify} align-items-center align-content-center`}>
          {col.filter !== true && <div className={cl}>{header}</div>}
          {col.filter === true && getFilterElement(header, options)}
          {col.sortable === true && sortButton(options)}
        </div>
      );
    },
    [
      props.columns,
      props.headerSize,
      props.enableHeaderWrap,
      props.actionHeaderTemplate,
      getFilterElement,
      sortButton,
      state.visibleColumns,
      visibleColumnsTemplate,
      onColumnToggle
    ]
  );

  const showSelection = useMemo(() => {
    return props.showSelectAll || props.selectionMode === 'multiple' || props.selectionMode === 'checkbox' || props.selectionMode === 'multipleNoRowCheckBox';
  }, [props.selectionMode, props.showSelectAll]);

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

    setters.setFilters(newFilters as any);
  };

  const arraysMatch = (arr1: T[], arr2: T[]): boolean => {
    if (arr1.length !== arr2.length) {
      return false;
    }
    for (let i = 0; i < arr1.length; i++) {
      if (arr1[props.id] !== arr2[props.id]) {
        return false;
      }
    }
    return true;
  };

  useEffect(() => {
    if (props.queryFilter) {
      if (data) {
        if (Array.isArray(data)) {
          setters.setPagedInformation(undefined);
          if (state.selectAll) {
            setters.setSelectedItems(data);
          }
          setDataSource(data);
          return;
        }

        if (isPagedResponse<T>(data)) {
          if (state.selectAll) {
            setters.setSelectedItems((data as PagedResponse<T>).Data as T[]);
          }

          if (data.PageNumber > 1 && data.TotalPageCount === 0) {
            const newData = { ...data };
            newData.PageNumber -= 1;
            newData.First = (newData.PageNumber - 1) * newData.PageSize;
            setters.setPage(newData.PageNumber);
            setters.setFirst(newData.First);
            setters.setPagedInformation(newData);
          } else {
            setters.setPagedInformation(data);
          }

          setDataSource((data as PagedResponse<T>).Data);
        }
      }
      return;
    }

    if (!props.dataSource) {
      return;
    }

    let filteredData = [...props.dataSource];

    if (state.filters) {
      Object.keys(state.filters).forEach((key) => {
        const filter = state.filters[key];
        if (filter.value) {
          filteredData = filteredData.filter((item: any) => {
            const itemValue = item[key as keyof typeof item];
            if (Array.isArray(filter.value)) {
              return typeof itemValue === 'string' && filter.value.some((val) => typeof val === 'string' && val.toLowerCase() === itemValue.toLowerCase());
            }
            return typeof itemValue === 'string' && itemValue.toLowerCase().includes(filter.value.toLowerCase());
          });
        }
      });
    }

    if (state.showSelected !== undefined && state.showSelected !== null) {
      filteredData = filteredData.filter((item: any) => {
        return state.showSelected
          ? state.selectedItems.some((selected) => selected.Id === item.Id)
          : !state.selectedItems.some((selected) => selected.Id === item.Id);
      });
    }

    if (state.sortOrder && state.sortField) {
      filteredData = filteredData.sort((a: any, b: any) => {
        const sortField = state.sortField as keyof typeof a;
        if (a[sortField] < b[sortField]) return -1 * state.sortOrder;
        if (a[sortField] > b[sortField]) return 1 * state.sortOrder;
        return 0;
      });
    }

    if (
      !state.pagedInformation ||
      state.pagedInformation.First !== state.first ||
      state.pagedInformation.PageNumber !== state.page ||
      state.pagedInformation.PageSize !== state.rows
    ) {
      const pagedInformation: PagedResponse<T> = {
        First: state.first,
        PageNumber: state.page,
        PageSize: state.rows,
        TotalItemCount: filteredData.length
      };
      setters.setPagedInformation(pagedInformation);
    }
    const pagedData = filteredData.slice(state.first, state.first + state.rows);
    if (!arraysMatch(pagedData, dataSource)) {
      setDataSource(pagedData);
    }

    // setDataSource(pagedData);
  }, [
    data,
    dataSource,
    props.dataSource,
    props.queryFilter,
    state.filters,
    state.first,
    state.page,
    state.rows,
    state.selectAll,
    state.selectedItems,
    state.showSelected,
    state.sortField,
    state.sortOrder,
    state.pagedInformation
  ]);

  const sourceRenderHeader = useMemo(() => {
    if (props.noSourceHeader === true) {
      return null;
    }

    return (
      <TableHeader
        dataSelectorProps={props}
        enableExport={props.enableExport ?? true}
        exportCSV={exportCSV}
        headerClassName={props.headerClassName}
        headerName={props.headerName}
        onMultiSelectClick={props.onMultiSelectClick}
        rowClick={state.rowClick}
        setRowClick={setters.setRowClick}
      />
    );
  }, [props, state.rowClick, setters.setRowClick]);

  const addSelection = useCallback(
    (data: T) => {
      const newSelectedItems = [...state.selectedItems, data];
      setters.setSelectedItems(newSelectedItems);
      props.onSelectionChange?.(newSelectedItems, false);
    },
    [props, setters, state.selectedItems]
  );

  const removeSelection = useCallback(
    (data: T) => {
      const newSelectedItems = state.selectedItems.filter((predicate) => predicate.Id !== data.Id);
      if (state.selectAll) {
        setters.setSelectAll(false);
      }
      setters.setSelectedItems(newSelectedItems);
      props.onSelectionChange?.(newSelectedItems, false);
    },
    [props, setters, state.selectAll, state.selectedItems]
  );

  const selectAllStatus = useMemo(() => {
    let checked = state.selectAll ? true : state.selectedItems.length > 0 ? false : null;

    return checked;
  }, [state.selectAll, state.selectedItems.length]);

  function toggleAllSelection() {
    if (state.selectAll) {
      setters.setSelectAll(false);
      setters.setSelectedItems([]);
      return;
    }

    if (selectAllStatus === null) {
      setters.setSelectAll(true);
      setters.setSelectedItems(props.dataSource ?? []);
      return;
    }

    if (selectAllStatus === true) {
      setters.setSelectAll(false);
      setters.setSelectedItems([]);
      return;
    }

    setters.setSelectAll(false);
    setters.setSelectedItems([]);
  }

  function selectionHeaderTemplate() {
    return (
      <div className="flex justify-content-center align-items-center">
        {props.showHiddenInSelection && <SMTriSelectShowHidden dataKey={props.id} />}
        {props.showSelectAll === true && (
          <SMTriSelectShowSelect selectedItemsKey={props.selectedItemsKey} id={props.id} onToggle={() => toggleAllSelection()} />
        )}
        {props.showSelected === true && <SMTriSelectShowSelected dataKey={props.id} />}
      </div>
    );
  }

  const selectionTemplate = useCallback(
    (data: T) => {
      if (!showSelection) {
        return <div />;
      }

      const found = state.selectedItems.some((item) => item.Id === data.Id);
      const isSelected = found ?? false;

      if (isSelected) {
        return (
          <div className="flex justify-content-center align-items-center p-0 m-0">
            {showSelection && <Checkbox checked={isSelected} onChange={() => removeSelection(data)} />}
          </div>
        );
      }

      return (
        <div className="flex justify-content-center align-items-center p-0 m-0">
          {showSelection && <Checkbox checked={isSelected} onChange={() => addSelection(data)} />}
        </div>
      );
    },
    [addSelection, removeSelection, showSelection, state.selectedItems]
  );

  const isLazy = useMemo(() => {
    // const la = props.lazy === true && (props.queryFilter === undefined ? undefined : true);
    // const la = props.queryFilter === undefined ? undefined : true;
    const la = props.lazy === true || (props.queryFilter === undefined ? undefined : true);

    // Logger.debug('DataTable', { datasource: state.dataSource, isLazy: la });

    return la;
  }, [props.lazy, props.queryFilter]);

  const showPageination = useMemo(() => {
    return props.enablePaginator === true; // && state.dataSource && state.dataSource.length >= state.rows;
  }, [props.enablePaginator]);

  const getClass = useMemo(() => {
    return 'sm-datatable surface-overlay';
  }, []);

  const getWrapperDiv = useMemo(() => {
    if (showPageination !== true) {
      return 'sm-standard-border-bottom';
    }
    return 'sm-standard-border-bottom-no-radius';
  }, [showPageination]);

  const getExpanderHeader = useMemo(() => {
    if (props.expanderHeader !== undefined) {
      return props.expanderHeader;
    }

    return null;
  }, [props.expanderHeader]);

  const getRowExpanderHeader = useMemo(() => {
    return (
      <div className="flex align-items-center justify-content-center">
        <span className="pi pi-equals" />
      </div>
    );
  }, []);

  const getPageTemplate = useMemo(() => {
    if (state.smTableIsSimple === true) {
      return 'FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink';
    }
    return 'RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink';
  }, [state.smTableIsSimple]);

  const getColClassName = useCallback((col: ColumnMeta) => {
    if (getAlign(col.align, col.fieldType) === 'right') {
      return col.className + ' pr-1';
    }
    return col.className;
  }, []);

  // Logger.debug('DataTable', { id: props.id, dataSource: props.dataSource });
  return (
    <div
      id={props.id}
      ref={wrapperRef}
      onClick={(event: any) => {
        if (props.enableClick !== true) {
          return;
        }
        const target = event.target as HTMLDivElement;
        if (props.showExpand === true || props.rowExpansionTemplate !== undefined) {
          if (target.className && target.className === 'p-datatable-wrapper') {
            setters.setExpandedRows(undefined);
          }
        }
        props.onClick?.(event);
      }}
      className=""
    >
      <div className={getClass}>
        {sourceRenderHeader && (
          <div>
            <div className="sm-datatable-header">{sourceRenderHeader}</div>
            <div className="layout-padding-bottom"></div>
          </div>
        )}

        <DataTable
          // id={props.id}
          dataKey="Id"
          cellSelection={false}
          editMode="cell"
          filterDisplay="row"
          expandedRows={state.expandedRows}
          filters={isEmptyObject(state.filters) ? getEmptyFilter(props.columns, state.showHidden) : state.filters}
          first={state.pagedInformation ? state.pagedInformation.First : state.first}
          loading={props.noIsLoading !== true ? props.isLoading === true || isLoading === true : false}
          onRowReorder={(e) => {
            onRowReorder(e.value);
          }}
          lazy={isLazy}
          onRowToggle={(e: DataTableRowToggleEvent) => {
            setters.setExpandedRows(e.data as DataTableExpandedRows);
          }}
          onSelectionChange={(e: DataTableSelectionMultipleChangeEvent<T[]> | DataTableSelectionSingleChangeEvent<T[]>) => {
            onSelectionChange(e);
          }}
          onFilter={onFilter}
          onPage={onPage}
          onRowClick={props.selectRow === true ? props.onRowClick : undefined}
          paginator={showPageination}
          paginatorClassName="text-xs p-0 m-0"
          paginatorTemplate={getPageTemplate}
          pt={{
            wrapper: { className: getWrapperDiv }
          }}
          ref={tableReference}
          rowClassName={props.rowClass ? props.rowClass : rowClass}
          rowExpansionTemplate={props.rowExpansionTemplate}
          rows={props.rows || state.rows}
          rowsPerPageOptions={[5, 10, 25, 50, 100, 250]}
          scrollHeight="flex"
          scrollable
          showGridlines
          size="small"
          sortField={props.reorderable ? 'rank' : state.sortField}
          sortMode="single"
          sortOrder={props.reorderable ? 0 : state.sortOrder}
          stripedRows
          style={props.style}
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
          value={dataSource} //props.dataSource !== undefined ? filteredValues : getDataFromQ} //{state.dataSource}
        >
          <Column
            body={props.addOrRemoveTemplate}
            // className="sm-w-2rem"
            field="addOrRemove"
            filter
            filterElement={props.addOrRemoveHeaderTemplate}
            hidden={!props.addOrRemoveTemplate}
            showFilterMenu={false}
            showFilterOperator={false}
            resizeable={false}
            style={getColumnStyles({ width: 24 } as ColumnMeta)}
          />

          <Column
            // className="sm-w-2rem"
            filterElement={getExpanderHeader}
            filter={props.expanderHeader !== undefined}
            hidden={!props.showExpand || props.rowExpansionTemplate === undefined}
            showFilterMenu={false}
            showFilterOperator={false}
            resizeable={false}
            expander
            style={getColumnStyles({ width: 20 } as ColumnMeta)}
          />
          <Column
            body={showSelection ? selectionTemplate : undefined}
            field="multiSelect"
            filter
            filterElement={selectionHeaderTemplate}
            showClearButton={false}
            showFilterMatchModes={false}
            showFilterMenu={false}
            showFilterOperator={false}
            hidden={!showSelection}
            style={getColumnStyles({
              maxWidth: props.showHiddenInSelection ? 42 : 20,
              minWidth: props.showHiddenInSelection ? 42 : 20,
              width: props.showHiddenInSelection ? 42 : 20
            } as ColumnMeta)}
          />
          <Column
            body={
              props.reorderable
                ? undefined
                : (e) => {
                    return <div></div>;
                  }
            }
            filter
            filterElement={getRowExpanderHeader}
            hidden={!props.reorderable}
            rowReorderIcon="pi pi-equals"
            rowReorder
            showClearButton={false}
            showFilterMatchModes={false}
            showFilterMenu={false}
            showFilterOperator={false}
            bodyClassName={'flex justify-content-center align-items-center'}
            style={getColumnStyles({ width: 12 } as ColumnMeta)}
          />
          {props.columns &&
            props.columns
              .filter((col) => col.removed !== true)
              .map((col) => (
                <Column
                  align={getAlign(col.align, col.fieldType)}
                  className={getColClassName(col)}
                  filter
                  filterElement={col.headerTemplate ? col.headerTemplate : colFilterTemplate}
                  filterPlaceholder={col.filter === true ? (col.fieldType === 'epg' ? 'EPG' : col.header ? col.header : camel2title(col.field)) : undefined}
                  // header={col.headerTemplate ? col.headerTemplate : getHeader(col.field, col.header, col.fieldType)}
                  body={(e) => (col.bodyTemplate ? col.bodyTemplate(e) : bodyTemplate(e, col.field, col.fieldType, settings.DefaultIcon, col.camelize))}
                  editor={col.editor}
                  field={col.field}
                  hidden={col.isHidden === true || col.fieldType === 'filterOnly'}
                  key={col.fieldType ? col.field + col.fieldType : col.field}
                  // style={getStyle(col, col.noAutoStyle !== true || col.bodyTemplate !== undefined)}
                  showAddButton
                  showApplyButton
                  showClearButton={false}
                  showFilterMatchModes
                  showFilterMenu={false}
                  showFilterMenuOptions
                  showFilterOperator
                  sortable={props.reorderable ? false : col.sortable}
                  style={getColumnStyles(col)}
                />
              ))}
        </DataTable>
      </div>
    </div>
  );
};

SMDataTable.displayName = 'SMDataTable';

export default memo(SMDataTable);
