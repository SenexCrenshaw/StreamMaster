import StringTracker from '@components/inputs/StringTracker';
import SMButton from '@components/sm/SMButton';
import { SMTriSelectShowHidden } from '@components/sm/SMTriSelectShowHidden';
import { SMTriSelectShowSelect } from '@components/sm/SMTriSelectShowSelect';
import { SMTriSelectShowSelected } from '@components/sm/SMTriSelectShowSelected';
import generateFilterData from '@components/smDataTable/helpers/generateFilterData';
import useSelectedSMItems from '@features/streameditor/useSelectedSMItems';
import { camel2title, isEmptyObject } from '@lib/common/common';
import { Logger } from '@lib/common/logger';
import { useSMContext } from '@lib/context/SMProvider';
import { PagedResponse, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { Checkbox } from 'primereact/checkbox';
import { Column, ColumnFilterElementTemplateOptions } from 'primereact/column';
import {
  DataTable,
  DataTableExpandedRows,
  DataTablePageEvent,
  DataTableSelectionMultipleChangeEvent,
  DataTableSelectionSingleChangeEvent,
  DataTableStateEvent,
  type DataTableRowData,
  type DataTableValue
} from 'primereact/datatable';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
import { Tooltip } from 'primereact/tooltip';
import { forwardRef, lazy, memo, Suspense, useCallback, useEffect, useImperativeHandle, useMemo, useRef, useState } from 'react';
import TableHeader from './helpers/TableHeader';
import { arraysEqualByKey } from './helpers/arraysEqual';
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
import { SMDataTableProps, SMDataTableRef } from './types/smDataTableInterfaces';

const LazyDataTable = lazy(() => import('primereact/datatable').then((module) => ({ default: module.DataTable })));

const SMDataTable = <T extends DataTableValue>(props: SMDataTableProps<T>, ref: React.Ref<SMDataTableRef<T>>) => {
  const { state, setters } = useSMDataSelectorValuesState<T>(props.id, props.selectedItemsKey);
  const { settings } = useSMContext();
  const wrapperRef = useRef<HTMLDivElement>(null);
  const [, setHasScrollbar] = useState(false);
  const tableReference = useRef<DataTable<T[]>>(null);
  const [, setIsExpanded] = useState<boolean>(false);
  const { queryFilter } = useSetQueryFilter(props.id, props.columns, state.first, state.filters, state.page, state.rows);
  const { data, isLoading } = props.queryFilter ? props.queryFilter(queryFilter) : { data: undefined, isLoading: false };
  const [dataSource, setDataSource] = useState<T[]>([]);
  const { setSelectedSMChannel } = useSelectedSMItems();

  // Logger.debug('SMDataTable', state.sortField, state.sortInfo);
  useEffect(() => {
    if (!props.defaultSortField) {
      return;
    }

    if (state.sortField) {
      return;
    }

    setters.setSortField(props.defaultSortField);
  }, [props.defaultSortField, setters, state.sortField, state.sortInfo]);

  useImperativeHandle(ref, () => ({
    clearExpanded() {
      setters.setExpandedRows(undefined);
    },
    setPagedInformation(pagedInformation: PagedResponse<T>) {
      setters.setPagedInformation(pagedInformation);
    }
  }));

  useEffect(() => {
    if (props.setSelectedSMChannel === undefined) {
      return;
    }
    if (state.expandedRows === undefined || state.expandedRows === null) {
      setSelectedSMChannel(undefined);
      return;
    }
    if (Object.keys(state.expandedRows).length === 0) {
      setSelectedSMChannel(undefined);
    } else {
      const keys = Object.keys(state.expandedRows);
      const firstKey = keys[0];
      const id = parseInt(firstKey, 10);
      const found = dataSource.find((item) => item.Id === id);
      if (found) {
        setSelectedSMChannel(found as unknown as SMChannelDto);
      }
    }
  }, [dataSource, props.setSelectedSMChannel, setSelectedSMChannel, state.expandedRows]);

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

  //T[]
  const onRowReorder = (changed: any) => {
    props.onRowReorder?.(changed);
  };

  const rowClass = useCallback(
    (data: DataTableRowData<T[]>): string => {
      const isHidden = getRecord({ data, dataKey: props.dataKey, fieldName: 'IsHidden' });

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (props.selectRow === true && state.selectedItems !== undefined) {
        const id = getRecord({ data, dataKey: props.dataKey, fieldName: 'Id' }) as number;
        if (state.selectedItems.some((item) => item.Id === id)) {
          return 'channel-row-selected';
        }
      }

      return '';
    },
    [props.dataKey, props.selectRow, state.selectedItems]
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
    (options: { field: string }) => {
      return (
        <SMButton
          icon={getSortIcon(options.field)}
          onClick={() => {
            setters.setSortField(options.field);
            setters.setSortInfo(options.field, state.sortOrder === 1 ? -1 : 1);
            //  setters.setSortOrder(state.sortOrder === 1 ? -1 : 1);
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
                // checkbox: { style: { display: 'none' } },
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

  if (props.id === 'EditSMChannelDialog') {
    Logger.debug('SMDataTable', { dataSource: props.dataSource, id: props.id });
  }
  // Helper function for handling paged responses
  const handlePagedResponse = useCallback(
    (pagedData: PagedResponse<T>) => {
      if (pagedData.PageNumber > 1 && pagedData.TotalPageCount === 0) {
        const newData = { ...pagedData };
        newData.PageNumber -= 1;
        newData.First = (newData.PageNumber - 1) * newData.PageSize;
        setters.setPage(newData.PageNumber);
        setters.setFirst(newData.First);
        setters.setPagedInformation(newData);
      } else {
        setters.setPagedInformation(pagedData);
      }
      setDataSource(pagedData.Data);
    },
    [setters]
  );

  // Helper function to handle query filter data
  const handleQueryFilterData = useCallback(
    (data: any) => {
      if (Array.isArray(data)) {
        setters.setPagedInformation(undefined);
        if (state.selectAll) {
          setters.setSelectedItems(data);
        }
        setDataSource(data);
      } else if (isPagedResponse<T>(data)) {
        if (state.selectAll) {
          setters.setSelectedItems((data as PagedResponse<T>).Data as T[]);
        }
        handlePagedResponse(data);
      }
    },
    [handlePagedResponse, setters, state.selectAll]
  );

  // Helper function to apply filters
  const applyFilters = useCallback(
    (data: T[]) => {
      if (!state.filters) return data;

      return data.filter((item: any) => {
        return Object.keys(state.filters).every((key) => {
          const filter = state.filters[key];
          const itemValue = item[key as keyof typeof item];

          if (filter.value && typeof itemValue === 'string') {
            if (Array.isArray(filter.value)) {
              return filter.value.some((val) => val.toLowerCase() === itemValue.toLowerCase());
            }
            return itemValue.toLowerCase().includes(filter.value.toLowerCase());
          }

          return true;
        });
      });
    },
    [state.filters]
  );

  // Helper function to apply selection filter
  const applySelectionFilter = useCallback(
    (data: T[]) => {
      if (state.showSelected === undefined || state.showSelected === null) return data;

      return data.filter((item: any) => {
        const isSelected = state.selectedItems.some((selected) => selected.Id === item.Id);
        return state.showSelected ? isSelected : !isSelected;
      });
    },
    [state.selectedItems, state.showSelected]
  );

  // Helper function to apply sorting
  const applySorting = useCallback(
    (data: T[]) => {
      if (!state.sortOrder || !state.sortField || !data) return data;

      const sortField = state.sortField as keyof T;

      // Create a shallow copy of the array to avoid mutating the original one
      const dataCopy = [...data];

      return dataCopy.sort((a, b) => {
        if (state.sortField === 'isSelected') {
          const selectedItems = state.selectedItems.map((item) => item.Id);
          const aSelected = selectedItems.includes(a.Id);
          const bSelected = selectedItems.includes(b.Id);
          return aSelected === bSelected ? 0 : aSelected ? -1 * state.sortOrder : 1 * state.sortOrder;
        }

        const aValue = a[sortField];
        const bValue = b[sortField];

        if (typeof aValue === 'number' && typeof bValue === 'number') {
          return (aValue < bValue ? -1 : aValue > bValue ? 1 : 0) * state.sortOrder;
        }

        return (aValue < bValue ? -1 : aValue > bValue ? 1 : 0) * state.sortOrder;
      });
    },
    [state.selectedItems, state.sortField, state.sortOrder]
  );

  // Helper function to update pagination information
  const updatePagedInformation = useCallback(
    (filteredData: T[]) => {
      if (
        !state.pagedInformation ||
        state.pagedInformation.First !== state.first ||
        state.pagedInformation.PageNumber !== state.page ||
        state.pagedInformation.PageSize !== state.rows ||
        state.pagedInformation.TotalItemCount !== filteredData.length
      ) {
        const pagedInformation: PagedResponse<T> = {
          First: state.first,
          PageNumber: state.page,
          PageSize: state.rows,
          TotalItemCount: filteredData.length
        };
        setters.setPagedInformation(pagedInformation);
      }
    },
    [setters, state.first, state.page, state.pagedInformation, state.rows]
  );

  // Helper function to update data source
  const updateDataSource = useCallback(
    (filteredData: T[]) => {
      const pagedData = filteredData.slice(state.first, state.first + state.rows);
      if (!arraysEqualByKey(pagedData, dataSource, props.arrayKey ?? 'Id')) {
        setDataSource(pagedData);
      }
    },
    [dataSource, props.arrayKey, state.first, state.rows]
  );

  useEffect(() => {
    // If a query filter is provided, handle fetching and setting data accordingly
    if (props.queryFilter && data) {
      handleQueryFilterData(data);
      return;
    }

    // If no external dataSource is provided, exit early
    if (!props.dataSource) return;

    // Handle data filtering, sorting, and pagination
    let filteredData = applyFilters(props.dataSource);
    filteredData = applySelectionFilter(filteredData);
    filteredData = applySorting(filteredData);

    // Update pagination information if necessary
    updatePagedInformation(filteredData);

    // Update the data source if the data has changed
    updateDataSource(filteredData);
  }, [
    applyFilters,
    applySelectionFilter,
    applySorting,
    data,
    handleQueryFilterData,
    props.dataSource,
    props.queryFilter,
    updateDataSource,
    updatePagedInformation
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
        {props.showSortSelected === true && sortButton({ field: 'isSelected' })}
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

    // Logger.debug('DataTable', { Id: props.id, isLazy: la, ProspisLazy: props.lazy, queryFilter: props.queryFilter });

    return la;
  }, [props.lazy, props.queryFilter]);

  const showPageination = useMemo(() => {
    return props.enablePaginator === true; // && state.dataSource && state.dataSource.length >= state.rows;
  }, [props.enablePaginator]);

  const getClass = useMemo(() => {
    return 'sm-datatable surface-overlay';
  }, []);

  // const getWrapperDiv = useMemo(() => {
  //   if (showPageination !== true) {
  //     return 'sm-standard-border-bottom';
  //   }
  //   return 'sm-standard-border-bottom-no-radius';
  // }, [showPageination]);

  const getExpanderHeader = useMemo(() => {
    if (props.expanderHeader !== undefined) {
      return props.expanderHeader;
    }

    return null;
  }, [props.expanderHeader]);

  const getRowExpanderHeader = useMemo(() => {
    return (
      <div className="flex align-items-center justify-content-center">
        <Tooltip target=".custom-target-icon" />
        <div className="custom-target-icon pi pi-sort" data-pr-position="right" data-pr-tooltip="Drag rows to reorder priority" />
      </div>
    );
  }, []);

  const getPageTemplate = useMemo(() => {
    if (state.smTableIsSimple === true) {
      return 'FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink';
    }
    return 'RowsPerPageDropdown  FirstPageLink PrevPageLink JumpToPageInput CurrentPageReport NextPageLink LastPageLink';
  }, [state.smTableIsSimple]);

  const getColClassName = useCallback((col: ColumnMeta) => {
    if (getAlign(col.align, col.fieldType) === 'right') {
      return col.className + ' pr-1';
    }
    return col.className;
  }, []);

  function findMissingKeys<T>(obj1: Record<string, T> | undefined, obj2: Record<string, T> | undefined): Record<string, T> {
    // if (!obj2 || obj2 === undefined) {
    //   return obj1 ?? {};
    // }

    if (obj1 === undefined || obj1 === null) {
      return obj2 ?? {};
    }

    const result: Record<string, T> = {};

    for (const key in obj2) {
      if (!(key in obj1)) {
        result[key] = obj2[key];
      }
    }

    return result;
  }

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
        <Suspense fallback={<div>Loading DataTable...</div>}>
          <LazyDataTable
            cellSelection={false}
            currentPageReportTemplate="{first} to {last} of {totalRecords}"
            dataKey={props.dataKey || 'Id'}
            editMode="cell"
            expandedRows={state.expandedRows}
            filterDisplay="row"
            filters={isEmptyObject(state.filters) ? getEmptyFilter(props.columns, state.showHidden) : state.filters}
            first={state.pagedInformation ? state.pagedInformation.First : state.first}
            loading={props.noIsLoading !== true ? props.isLoading === true || isLoading === true : false}
            lazy={isLazy}
            onRowCollapse={(e) => {
              setIsExpanded(false);
              props.onRowCollapse?.(e);
            }}
            onRowExpand={(e) => {
              setIsExpanded(true);
              props.onRowExpand?.(e);
            }}
            onRowReorder={(e) => {
              onRowReorder(e.value);
            }}
            onRowToggle={(e: any) => {
              if (props.singleExpand === true) {
                const expandedRows = findMissingKeys(state.expandedRows, e.data);
                setters.setExpandedRows(expandedRows);
              } else {
                setters.setExpandedRows(e.data as DataTableExpandedRows);
              }
            }}
            onSelectionChange={(e: DataTableSelectionMultipleChangeEvent<T[]>) => {
              onSelectionChange(e);
            }}
            onFilter={onFilter}
            onPage={onPage}
            onRowClick={props.selectRow === true ? props.onRowClick : undefined}
            paginator={showPageination}
            paginatorClassName="text-xs p-0 m-0"
            paginatorTemplate={getPageTemplate}
            // pt={{
            //   wrapper: { className: getWrapperDiv }
            // }}
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
            value={dataSource} //props.dataSource !== undefined ? filteredValues : getDataFromQ} //{state.dataSource}
          >
            <Column
              body={props.addOrRemoveTemplate}
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
              sortable
              hidden={!showSelection}
              style={getColumnStyles({
                maxWidth: props.showHiddenInSelection ? 42 : 20,
                minWidth: props.showHiddenInSelection ? 42 : 20,
                width: props.showHiddenInSelection ? 42 : 20
              } as ColumnMeta)}
            />
            <Column
              filter
              filterElement={getRowExpanderHeader}
              hidden={!props.reorderable}
              rowReorderIcon="font-bold pi pi-equals"
              rowReorder
              showClearButton={false}
              showFilterMatchModes={false}
              showFilterMenu={false}
              showFilterOperator={false}
              className={'sm-rowreorder'}
              // style={getColumnStyles({ width: '2rem' } as ColumnMeta)}
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
                    body={(e) =>
                      col.bodyTemplate ? col.bodyTemplate(e) : bodyTemplate(e, col.field, col.fieldType, settings.DefaultLogo, col.camelize, props.dataKey)
                    }
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
          </LazyDataTable>
        </Suspense>
      </div>
    </div>
  );
};

SMDataTable.displayName = 'SMDataTable';

export default memo(forwardRef(SMDataTable));
