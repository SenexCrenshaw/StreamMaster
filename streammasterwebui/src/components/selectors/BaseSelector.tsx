/* eslint-disable @typescript-eslint/no-unused-vars */
import { type skipToken } from '@reduxjs/toolkit/dist/query/react';
import { Dropdown, type DropdownChangeEvent, type DropdownFilterEvent } from 'primereact/dropdown';
import { classNames } from 'primereact/utils';
import { useCallback, useEffect, useState } from 'react';
import { addOrUpdateValueForField, doSetsContainSameIds, type GetApiArg, type HasId, type SMDataTableFilterMetaData, type SimpleQueryApiArg } from "../../common/common";

export type PagedResponseDto<T> = {
  data: T[];
  first: number;
  pageNumber: number;
  pageSize: number;
  totalItemCount: number;
  totalPageCount: number;
  totalRecords: number;
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
  readonly querySelectedItem: (arg: string) => Promise<T>;
  readonly selectName: string;
  // eslint-disable-next-line react/no-unused-prop-types
  readonly selectedTemplate: (option: T) => JSX.Element;
  readonly value?: string;
}

const BaseSelector = <T extends HasId>(props: BaseSelectorProps<T>) => {
  const [selectedItem, setSelectedItem] = useState<string>('');
  const [index, setIndex] = useState<number>(0);
  const [totalItems, setTotalItems] = useState<number>(0);
  const [dataSource, setDataSource] = useState<T[]>([]);
  const [filteredDataSource, setFilteredDataSource] = useState<T[]>([]);

  const [simpleQuery, setSimpleQuery] = useState<SimpleQueryApiArg>({ first: 0, last: 200 });
  const query = props.queryHook(simpleQuery);

  const [queryFilter, setQueryFilter] = useState<GetApiArg | undefined>(undefined);

  const filterQuery = props.queryFilter(queryFilter ?? { pageSize: 40 });


  useEffect(() => {
    if (!query?.data) return;
    var existingIds = new Set(dataSource.map(x => x.id));
    const newItems = query.data.filter((cn: T) => cn?.id && (!existingIds.has(cn.id)));

    if (newItems.length > 0) {
      const d = dataSource.concat(newItems);
      setDataSource(d);
      setIndex(d.length);
      setFilteredDataSource(d);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [query]);

  useEffect(() => {
    var existingIds = new Set(dataSource.map(x => x.id));
    var existingFiltered = new Set(filteredDataSource.map(x => x.id));

    var existingIds2 = dataSource.map(x => x.id);

    if (existingIds.size !== existingIds2.length) {
      console.log('mismatch existingIds', existingIds)
      console.log('mismatch existingIds2', existingIds2)
      console.log('mismatch', existingIds.size, existingIds2.length)
    }

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
      if (dataSource.length > 0) {
        const filteredData = filterQuery.data.data;

        const newItems = filteredData.filter((cn: T) => cn?.id && (!existingIds.has(cn.id)));

        if (newItems.length > 0) {
          setDataSource(dataSource.concat(newItems));
          setIndex(dataSource.length + newItems.length);
        }

        setFilteredDataSource(filteredData);

      }
    } else {
      setFilteredDataSource(dataSource);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filterQuery]);

  useEffect(() => {
    if (props.value === null || props.value === undefined) return;

    if (selectedItem === props.value) {
      return;
    }

    var existingIds = new Set(dataSource.map(x => x.id));
    var existingIds2 = dataSource.map(x => x.id);

    if (existingIds.size !== existingIds2.length) {
      console.log('mismatch')
    }

    if (props.value !== '') {
      try {
        props.querySelectedItem(props.value).then((item) => {
          if (item) {

            if (item && item.source != selectedItem && !existingIds.has(item.id)) {
              const newDataSource = dataSource.concat(item);

              setDataSource(newDataSource);
              setFilteredDataSource(newDataSource);
              setIndex(newDataSource.length);
              setSelectedItem(item.source);
              return;
            }
          }
        }).catch((e) => { console.error(e) });

      } catch (e) {
        console.error(e);
      }

      setSelectedItem(props.value);

    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.value]);

  const onChange = useCallback((event: DropdownChangeEvent) => {
    if (event.value !== selectedItem) {
      setSelectedItem(event.value);

      if (event.value && props.onChange) {
        props.onChange(event.value);
      }
    }
  }, [selectedItem, props]);

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
    setQueryFilter({ jsonFiltersString: JSON.stringify(toSend), pageSize: 40 } as GetApiArg);
    // setFilter(event.filter.toLowerCase());
  }

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const selectedTemplate = (option: any) => {
    return (
      <div>
        {option}
      </div>
    );
  };

  return (
    <div className="BaseSelector flex align-contents-center w-full min-w-full" >
      <Dropdown
        className={className}
        disabled={props.disabled}
        editable={props.editable}
        filter
        filterBy={props.optionLabel}
        filterPlaceholder={`Filter ${props.selectName}`}
        itemTemplate={props.itemTemplate}
        onChange={onChange}
        onFilter={onFilter}
        optionLabel={props.optionLabel}
        optionValue={props.optionValue}
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
          // eslint-disable-next-line @typescript-eslint/no-explicit-any
          onLazyLoad: (e: any) => {
            if (e.filter === '' && e.last as number >= index) {
              let firstRecord = e.first as number < index ? index : e.first as number;
              setSimpleQuery({ first: firstRecord, last: e.last as number + 100 } as SimpleQueryApiArg)
            }
          },
          showLoader: true,
          style: { width: '400px' },
        }}

      />
    </div>
  );
};

BaseSelector.defaultProps = {
  className: null,
  disabled: false,
  editable: false,
  isLoading: true,
};

export default BaseSelector;
