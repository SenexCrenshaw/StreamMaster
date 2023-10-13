import {
  addOrUpdateValueForField,
  doSetsContainSameIds,
  type GetApiArg,
  type HasId,
  type SMDataTableFilterMetaData,
  type SimpleQueryApiArg,
} from '@lib/common/common';
import { skipToken } from '@reduxjs/toolkit/dist/query/react';
import { Dropdown, type DropdownChangeEvent, type DropdownFilterEvent } from 'primereact/dropdown';
import { classNames } from 'primereact/utils';
import { useCallback, useEffect, useState } from 'react';
import getRecord from '../dataSelector/getRecord';

export type PagedResponseDto<T> = {
  data: T[];
  first: number;
  pageNumber: number;
  pageSize: number;
  totalItemCount: number;
  totalPageCount: number;
};

export type StringArg = {
  value: string;
};
export type PagedResponseDtoData<T> = {
  data?: PagedResponseDto<T>;
};

export type SimpleQueryResponse<T> = {
  data?: T[];
};

export type BaseSelectorProps<T extends HasId> = {
  readonly className?: string | null;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly isLoading?: boolean;
  readonly itemSize: number[] | number | undefined;
  readonly itemTemplate: (option: T) => JSX.Element;
  readonly onChange: (value: string) => void;
  readonly optionLabel: string;
  readonly optionValue: string;
  readonly queryFilter: (option: GetApiArg | typeof skipToken) => PagedResponseDtoData<T>;
  readonly queryHook: (option: SimpleQueryApiArg) => SimpleQueryResponse<T>;
  readonly querySelectedItem: (arg: StringArg) => Promise<T | null>;
  readonly selectName: string;
  readonly selectedTemplate: (option: T) => JSX.Element;
  readonly value?: string;
};

const BaseSelector = <T extends HasId>(props: BaseSelectorProps<T>) => {
  const [selectedItemName, setSelectedItemName] = useState<string>('');
  const [selectedItem, setSelectedItem] = useState<T>();
  const [index, setIndex] = useState<number>(0);
  const [totalItems, setTotalItems] = useState<number>(0);
  const [dataSource, setDataSource] = useState<T[]>([]);
  const [filteredDataSource, setFilteredDataSource] = useState<T[]>([]);

  const [simpleQuery, setSimpleQuery] = useState<SimpleQueryApiArg>({
    first: 0,
    last: 200,
  });
  const query = props.queryHook(simpleQuery);

  const [queryFilter, setQueryFilter] = useState<GetApiArg | undefined>(undefined);

  const filterQuery = props.queryFilter(queryFilter ?? skipToken);

  const dothing = useCallback(
    (data: T[]) => {
      if (query.data === undefined || query.data.length === 0 || props.value === null || props.value === undefined) return;

      if (selectedItemName === props.value) {
        return;
      }

      var existingIds = new Set(data.map((x) => x.id));
      // console.group('dothing');
      // console.log('props.value', props.value);
      // console.log('data', data);
      // console.log('existingIds', existingIds);
      // console.log('has', existingIds.has(props.value));
      // console.groupEnd();
      if (!existingIds.has(props.value)) {
        if (props.value !== '') {
          try {
            props
              .querySelectedItem({ value: props.value } as StringArg)
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
                    return;
                  }
                }
              })
              .catch((e) => {
                console.error(e);
              });
          } catch (e) {
            console.error(e);
          }
        }
      }
      const item = data.find((x) => x.id === props.value);
      // console.log('has', item);
      setSelectedItem(item);
      setSelectedItemName(props.value);
    },
    [props, query.data, selectedItemName],
  );

  useEffect(() => {
    var existingIds = new Set(dataSource.map((x) => x.id));
    var existingFiltered = new Set(filteredDataSource.map((x) => x.id));

    // var existingIds2 = dataSource.map((x) => x.id)

    // if (existingIds.size !== existingIds2.length) {
    //   console.log('mismatch existingIds', existingIds)
    //   console.log('mismatch existingIds2', existingIds2)
    //   console.log('mismatch', existingIds.size, existingIds2.length)
    // }

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
        if (selectedItem) {
          if (!filteredData.find((x) => x.id === selectedItem?.id)) {
            const updatedFilteredData = [selectedItem, ...filteredData];
            setFilteredDataSource(updatedFilteredData);
            return;
          }
        }
        setFilteredDataSource(filteredData);
      }
      // }
    } else {
      setFilteredDataSource(dataSource);
    }
  }, [dataSource, filterQuery, filteredDataSource, queryFilter, selectedItem, totalItems]);

  // useEffect(() => {
  //   if (query.data === undefined || query.data.length === 0 || props.value === null || props.value === undefined) return;

  //   if (selectedItemName === props.value) {
  //     return;
  //   }

  //   var existingIds = new Set(dataSource.map((x) => x.id));
  //   console.log('querySelectedItem', props.value, dataSource, existingIds);

  //   if (!existingIds.has(props.value)) {
  //     if (props.value !== '') {
  //       try {
  //         props
  //           .querySelectedItem({ value: props.value } as StringArg)
  //           .then((item) => {
  //             if (item) {
  //               existingIds = new Set(dataSource.map((x) => x.id));
  //               if (item && item.source !== selectedItemName && !existingIds.has(item.id)) {
  //                 const newDataSource = dataSource.concat(item);
  //                 setDataSource(newDataSource);
  //                 setFilteredDataSource(newDataSource);
  //                 setIndex(newDataSource.length);
  //                 setSelectedItemName(item.source);
  //                 setSelectedItem(item);
  //                 return;
  //               }
  //             }
  //           })
  //           .catch((e) => {
  //             console.error(e);
  //           });
  //       } catch (e) {
  //         console.error(e);
  //       }
  //     }
  //     const item = dataSource.find((x) => x.id === props.value);
  //     setSelectedItem(item);
  //     setSelectedItemName(props.value);
  //   }
  //   // eslint-disable-next-line react-hooks/exhaustive-deps
  // }, [props.value, query.data, dataSource]);

  useEffect(() => {
    if (!query?.data) return;

    var existingIds = new Set(dataSource.map((x) => x.id));
    const newItems = query.data.filter((cn: T) => cn?.id && !existingIds.has(cn.id));

    if (newItems.length > 0) {
      const d = dataSource.concat(newItems);
      // console.log('query.data', d);
      setDataSource(d);
      dothing(d);
      setIndex(d.length);
      setFilteredDataSource(d);
    }
  }, [dataSource, dothing, query]);

  const onChange = useCallback(
    (event: DropdownChangeEvent) => {
      if (event.value !== selectedItemName) {
        const name = getRecord(event.value, props.optionValue);
        setSelectedItemName(name);
        setSelectedItem(event.value);
        if (name && props.onChange) {
          props.onChange(name);
        }
      }
    },
    [selectedItemName, props],
  );

  const className = classNames('BaseSelector align-contents-center p-0 m-0 max-w-full w-full', props.className, {
    'p-button-loading': props.isLoading,
    'p-disabled': props.disabled,
  });

  const onFilter = (event: DropdownFilterEvent) => {
    if (event.filter === '') {
      // setQueryFilter(undefined);

      return;
    }

    const toSend = [] as SMDataTableFilterMetaData[];

    addOrUpdateValueForField(toSend, 'name', 'contains', event.filter);
    setQueryFilter({
      jsonFiltersString: JSON.stringify(toSend),
      pageSize: 40,
    } as GetApiArg);
    // setFilter(event.filter.toLowerCase());
  };

  return (
    <div className="BaseSelector flex align-contents-center w-full min-w-full">
      <Dropdown
        className={className}
        disabled={props.disabled}
        editable={props.editable}
        filter
        filterBy={props.optionLabel}
        // filterPlaceholder={`Filter ${props.selectName}`}
        itemTemplate={props.itemTemplate}
        onChange={onChange}
        onFilter={onFilter}
        optionLabel={props.optionLabel}
        // optionValue={props.optionValue}
        options={filteredDataSource}
        placeholder={`Select ${props.selectName}`}
        resetFilterOnHide
        scrollHeight="40vh"
        showFilterClear
        value={selectedItem}
        valueTemplate={props.selectedTemplate}
        virtualScrollerOptions={{
          delay: 200,
          itemSize: props.itemSize,
          lazy: true,
          loaderDisabled: true,
          // loadingTemplate: loadingTemplate,
          numToleratedItems: 40,

          onLazyLoad: (e: any) => {
            if (e.filter === '' && (e.last as number) >= index) {
              let firstRecord = (e.first as number) < index ? index : (e.first as number);
              setSimpleQuery({
                first: firstRecord,
                last: (e.last as number) + 100,
              } as SimpleQueryApiArg);
            }
          },
          showLoader: true,
          style: { width: '400px' },
        }}
      />
    </div>
  );
};

export default BaseSelector;
