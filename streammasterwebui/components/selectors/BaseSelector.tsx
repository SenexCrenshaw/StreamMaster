import {
  addOrUpdateValueForField,
  doSetsContainSameIds,
  type GetApiArgument,
  type HasId,
  type SMDataTableFilterMetaData,
  type SimpleQueryApiArgument
} from '@lib/common/common';
import { skipToken } from '@reduxjs/toolkit/dist/query/react';
import { Dropdown, type DropdownChangeEvent, type DropdownFilterEvent } from 'primereact/dropdown';
import { classNames } from 'primereact/utils';
import { useCallback, useEffect, useState } from 'react';
import getRecord from '../dataSelector/getRecord';

export interface PagedResponseDto<T> {
  data: T[];
  first: number;
  pageNumber: number;
  pageSize: number;
  totalItemCount: number;
  totalPageCount: number;
}

export interface StringArgument {
  value: string;
}
export interface PagedResponseDtoData<T> {
  data?: PagedResponseDto<T>;
}

export interface SimpleQueryResponse<T> {
  data?: T[];
}

export interface BaseSelectorProperties<T extends HasId> {
  readonly className?: string | null;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly filterValue?: string;
  readonly isLoading?: boolean;
  readonly itemSize: number;
  readonly itemTemplate: (option: T) => JSX.Element;
  readonly onChange: (value: string) => void;
  readonly optionLabel: string;
  readonly optionValue: string;
  readonly queryFilter: (option: GetApiArgument | typeof skipToken) => PagedResponseDtoData<T>;
  readonly queryHook: (option: SimpleQueryApiArgument) => SimpleQueryResponse<T>;
  readonly querySelectedItem: (argument: StringArgument) => Promise<T | null>;
  readonly selectName: string;
  readonly selectedTemplate: (option: T) => JSX.Element;
  readonly value?: string;
}

const BaseSelector = <T extends HasId>(props: BaseSelectorProperties<T>) => {
  const [selectedItemName, setSelectedItemName] = useState<string>('');
  const [selectedItem, setSelectedItem] = useState<T>();
  const [index, setIndex] = useState<number>(0);
  const [totalItems, setTotalItems] = useState<number>(0);
  const [dataSource, setDataSource] = useState<T[]>([]);
  const [filteredDataSource, setFilteredDataSource] = useState<T[]>([]);

  const [simpleQuery, setSimpleQuery] = useState<SimpleQueryApiArgument>({
    first: 0,
    last: 200
  });
  const query = props.queryHook(simpleQuery);

  const [queryFilter, setQueryFilter] = useState<GetApiArgument | undefined>();

  const filterQuery = props.queryFilter(queryFilter ?? skipToken);

  const dothing = useCallback(
    (data: T[]) => {
      if (query.data === undefined || query.data.length === 0 || props.value === null || props.value === undefined) return;

      if (selectedItemName === props.value) {
        return;
      }

      let existingIds = new Set(data.map((x) => x.id));

      if (!existingIds.has(props.value) && props.value !== '') {
        try {
          props
            .querySelectedItem({ value: props.value } as StringArgument)
            .then((item) => {
              if (item) {
                existingIds = new Set(data.map((x) => x.id));
                if (item && item.source !== selectedItemName && !existingIds.has(item.id)) {
                  const newDataSource = data.concat(item);
                  setDataSource(newDataSource);
                  setFilteredDataSource(newDataSource);
                  setIndex(newDataSource.length);
                  setSelectedItemName(item.source);
                  setSelectedItem(item);
                }
              }
            })
            .catch((error) => {
              console.error(error);
            });
        } catch (error) {
          console.error(error);
        }
      }
      const item = data.find((x) => x.id === props.value || x.source === props.value);

      if (item) {
        setSelectedItem(item);
      }
      setSelectedItemName(props.value);
    },
    [props, query.data, selectedItemName]
  );

  useEffect(() => {
    if (filteredDataSource && filteredDataSource.length > 0) {
      const item = filteredDataSource.find((x) => x.id === props.value);
      if (!item || selectedItem?.id === props.value) {
        return;
      }

      setSelectedItemName(item.source);
      setSelectedItem(item);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.value]);

  useEffect(() => {
    const existingIds = new Set(dataSource.map((x) => x.id));
    const existingFiltered = new Set(filteredDataSource.map((x) => x.id));

    if (queryFilter?.jsonFiltersString && !queryFilter.jsonFiltersString && dataSource.length > 0) {
      if (!doSetsContainSameIds(existingIds, existingFiltered)) {
        setFilteredDataSource(dataSource);
      }

      if (filterQuery.data && filterQuery.data.totalItemCount !== totalItems) {
        setTotalItems(filterQuery.data.totalItemCount);
      }

      return;
    }

    if (!filterQuery.data?.data) {
      return;
    }

    if (filterQuery.data) {
      // if (dataSource.length > 0) {
      const filteredData = filterQuery.data.data;
      const newItems = filteredData.filter((cn: T) => cn?.id && !existingIds.has(cn.id));
      let ds = dataSource;

      if (newItems.length > 0) {
        ds = ds.concat(newItems);
        setDataSource(ds);
        setIndex(ds.length);
      }

      if (filteredData.length === 0) {
        setFilteredDataSource(ds);
      } else {
        if (selectedItem && !filteredData.find((x) => x.id === selectedItem?.id)) {
          const updatedFilteredData = [selectedItem, ...filteredData];
          setFilteredDataSource(updatedFilteredData);
          return;
        }
        setFilteredDataSource(filteredData);
      }
      // }
    } else {
      setFilteredDataSource(dataSource);
    }
  }, [dataSource, filterQuery, filteredDataSource, queryFilter, selectedItem, totalItems]);

  useEffect(() => {
    if (query.data == null) return;

    if (query.data.length === 0 && props.value !== undefined) {
      setSelectedItemName(props.value);
      return;
    }

    const existingIds = new Set(dataSource.map((x) => x.id));
    const newItems = query.data.filter((cn: T) => cn?.id && !existingIds.has(cn.id));

    if (newItems.length > 0) {
      const d = dataSource.concat(newItems);
      setDataSource(d);
      dothing(d);
      setIndex(d.length);
      setFilteredDataSource(d);
    }
  }, [dataSource, dothing, props.value, query]);

  const onChange = useCallback(
    (event: DropdownChangeEvent) => {
      let newValue = event.value;

      if (event.value.id !== undefined) {
        newValue = getRecord(event.value, props.optionValue);
      }

      if (newValue !== selectedItemName) {
        setSelectedItemName(newValue);
        setSelectedItem(event.value);
        if (newValue && props.onChange) {
          props.onChange(newValue);
        }
      }
    },
    [selectedItemName, props]
  );

  const className = classNames('BaseSelector align-contents-center p-0 m-0 max-w-full w-full', props.className, {
    'p-button-loading': props.isLoading,
    'p-disabled': props.disabled
  });

  const onFilter = (event: DropdownFilterEvent) => {
    if (event.filter === '') {
      return;
    }

    const toSend = [] as SMDataTableFilterMetaData[];

    addOrUpdateValueForField(toSend, 'name', 'contains', event.filter);
    setQueryFilter({
      jsonFiltersString: JSON.stringify(toSend),
      pageSize: 40
    } as GetApiArgument);
  };

  console.log(props.editable);
  return (
    <div className="BaseSelector flex align-contents-center w-full min-w-full">
      <Dropdown
        className={className}
        disabled={props.disabled}
        editable={props.editable}
        filter
        filterBy={props.optionLabel}
        // filterPlaceholder={`Filter ${selectedItemName}`}
        itemTemplate={props.itemTemplate}
        onChange={onChange}
        onFilter={onFilter}
        optionLabel={props.optionLabel}
        // optionValue={props.optionValue}
        options={filteredDataSource}
        placeholder={selectedItemName}
        resetFilterOnHide
        scrollHeight="40vh"
        showFilterClear
        value={selectedItem}
        valueTemplate={props.selectedTemplate}
        virtualScrollerOptions={{
          itemSize: props.itemSize,
          lazy: true,
          loaderDisabled: true,
          // loadingTemplate: loadingTemplate,
          numToleratedItems: 100,

          onLazyLoad: (e: any) => {
            if (e.filter === '' && (e.last as number) >= index) {
              const firstRecord = (e.first as number) < index ? index : (e.first as number);
              setSimpleQuery({
                first: firstRecord,
                last: (e.last as number) + 200
              } as SimpleQueryApiArgument);
            }
          },
          showLoader: true,
          style: { width: '400px' }
        }}
      />
    </div>
  );
};

export default BaseSelector;
