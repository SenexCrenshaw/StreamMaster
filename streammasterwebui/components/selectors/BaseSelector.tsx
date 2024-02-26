import { addOrUpdateValueForField, type GetApiArgument, type HasId, type SMDataTableFilterMetaData, type SimpleQueryApiArgument } from '@lib/common/common';
import { PagedResponseDtoData, SimpleQueryResponse, StringArgument } from '@lib/common/dataTypes';

import { useCache } from '@lib/redux/CacheProvider';
import { skipToken } from '@reduxjs/toolkit/dist/query/react';
import { Dropdown, type DropdownChangeEvent, type DropdownFilterEvent } from 'primereact/dropdown';
import { classNames } from 'primereact/utils';
import { useCallback, useEffect, useState } from 'react';
import getRecord from '../dataSelector/getRecord';

export interface BaseSelectorProperties<T extends HasId> {
  readonly className?: string | null;
  readonly dataKey: string;
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
  const [filteredDataSource, setFilteredDataSource] = useState<T[]>([]);

  const { cacheData, updateCache, fetchAndAddItem } = useCache<T>(props.dataKey, props.querySelectedItem);

  const isIconSelector = props.dataKey === 'iconSelector';

  const [simpleQuery, setSimpleQuery] = useState<SimpleQueryApiArgument>({
    first: 0,
    last: 200000
  });

  const query = props.queryHook(simpleQuery);

  const [queryFilter, setQueryFilter] = useState<GetApiArgument | undefined>();

  const filterQuery = props.queryFilter(queryFilter ?? skipToken);

  const logSmallData = useCallback((data: T[]) => {
    if (data.length > 1 && data.length < 100) {
      // console.log(data);
    }
  }, []);

  useEffect(() => {
    if (filteredDataSource.length === 0 && cacheData.length > 0) {
      //   logSmallData(cacheData);
      setFilteredDataSource(cacheData);
      // }
    }
  }, [cacheData, filteredDataSource.length]);

  const setFiltered = useCallback(
    (data: T[]) => {
      logSmallData(data);

      if (selectedItem) {
        const updatedFilteredData = [selectedItem, ...data.filter((x) => x.id !== selectedItem?.id)];
        setFilteredDataSource(updatedFilteredData);
        return;
      }
      setFilteredDataSource(data);
    },
    [logSmallData, selectedItem]
  );

  const getItemByValue = useCallback(
    (toMatch: string) => {
      if (isIconSelector) {
        return cacheData.find((item) => item.source.toString() === toMatch);
      }

      return cacheData.find((item) => item.id.toString() === toMatch);
    },
    [cacheData, isIconSelector]
  );

  //Add props.value to the cache if it is not already there
  useEffect(() => {
    if (props.value === null || props.value === null) return;
    if (selectedItem !== null && selectedItem !== undefined && selectedItem.id === props.value) return;

    fetchAndAddItem({ value: props.value } as StringArgument).catch((error) => {
      console.error(error);
    });
  }, [cacheData, fetchAndAddItem, props.value, selectedItem]);

  //Set the selected item if the value is set
  useEffect(() => {
    // if (selectedItem === undefined && props.value !== undefined && filteredDataSource && filteredDataSource.length > 0) {
    if (props.value !== undefined && filteredDataSource && filteredDataSource.length > 0) {
      const item = getItemByValue(props.value);

      if (!item) {
        if (!isIconSelector) {
          setSelectedItemName(props.value);
        }
        return;
      }
      if (isIconSelector) {
        setSelectedItemName(item.source);
      } else {
        setSelectedItemName(item.displayName);
      }
      setSelectedItem(item);
    }
  }, [filteredDataSource, getItemByValue, isIconSelector, props.value, selectedItem]);

  //Data was Filtered
  useEffect(() => {
    if (queryFilter === undefined || !queryFilter.jsonFiltersString) {
      // if (cacheData.length > 0 && filteredDataSource.length === 0) {
      //   logSmallData(cacheData);
      //   setFilteredDataSource(cacheData);
      // }
      return;
    }

    if (!filterQuery.data?.data) {
      return;
    }

    if (filterQuery.data.totalItemCount !== totalItems) {
      setTotalItems(filterQuery.data.totalItemCount);
    }

    if (filterQuery.data) {
      const filteredData = filterQuery.data.data;
      const existingIds = cacheData !== undefined ? new Set(cacheData.map((x: HasId) => (x as HasId).id)) : new Set();
      const newItems = filteredData.filter((cn: T) => cn?.id && !existingIds.has(cn.id));
      let ds = cacheData;

      if (newItems.length > 0) {
        ds = ds.concat(newItems);
        updateCache(ds);
        setIndex(ds.length);
      }

      if (filteredData.length === 0) {
        if (ds.length > 1 && ds.length < 100) {
          console.log(ds);
        }
        setFiltered(ds);
      } else {
        if (selectedItem && !filteredData.find((x) => x.id === selectedItem?.id)) {
          const updatedFilteredData = [selectedItem, ...filteredData.filter((x) => x.id !== selectedItem?.id)];
          logSmallData(updatedFilteredData);

          setFiltered(filteredData);
          return;
        }

        logSmallData(filteredData);
        setFiltered(filteredData);
      }
      // }
    } else {
      logSmallData(cacheData);

      setFiltered(cacheData);
    }
  }, [cacheData, filterQuery, logSmallData, queryFilter, selectedItem, setFiltered, totalItems, updateCache]);

  //New Data, new page
  useEffect(() => {
    if (query.data == null) return;

    if (query.data.length === 0 && props.value !== undefined) {
      setSelectedItemName(props.value);
      return;
    }

    const existingIds = cacheData !== undefined ? new Set(cacheData.map((x: HasId) => x.id)) : new Set();
    const newItems = query.data.filter((cn: T) => cn?.id && !existingIds.has(cn.id));

    let d: T[] = [];

    if (newItems.length > 0) {
      if (cacheData === undefined) {
        d = newItems;
        updateCache(newItems);
      } else {
        d = cacheData.concat(newItems);
        updateCache(d);
      }

      setIndex(d?.length ?? 0);
      if (d.length > 1 && d.length < 100) {
        // console.log(d);
      }
      setFiltered(d);
    }
  }, [cacheData, props.value, query, setFiltered, updateCache]);

  const onChange = useCallback(
    (event: DropdownChangeEvent) => {
      let newValue = event.value;

      if (event.value.id !== undefined) {
        newValue = getRecord(event.value, props.optionValue);
      }

      if (newValue !== selectedItemName) {
        if (newValue && props.onChange) {
          setSelectedItemName(newValue);
          setSelectedItem(event.value);
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
      setFilteredDataSource(cacheData);
      return;
    }
    const toSend = [] as SMDataTableFilterMetaData[];
    addOrUpdateValueForField(toSend, 'name', 'contains', event.filter);
    setQueryFilter({
      jsonFiltersString: JSON.stringify(toSend),
      pageSize: 40
    } as GetApiArgument);
  };

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
        onHide={() => {
          setQueryFilter(undefined);
          setFiltered(cacheData);
        }}
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
          style: {
            minWidth: '400px',
            width: '400px',
            maxWidth: '50vw'
          }
        }}
      />
    </div>
  );
};

export default BaseSelector;
