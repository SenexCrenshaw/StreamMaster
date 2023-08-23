
import { useState, useEffect, useCallback, useMemo } from 'react';
import { type DropdownFilterEvent } from 'primereact/dropdown';
import { type DropdownChangeEvent } from 'primereact/dropdown';
import { Dropdown } from 'primereact/dropdown';
import { Skeleton } from 'primereact/skeleton';
import { classNames } from 'primereact/utils';
import { type HasId, type SimpleQueryApiArg } from "../../common/common";
import { type VirtualScrollerTemplateOptions } from 'primereact/virtualscroller';

export type BaseSelectorProps<T extends HasId> = {
  className?: string | null;
  disabled?: boolean;
  editable?: boolean | undefined;
  isLoading?: boolean;
  itemSize: number[] | number | undefined;
  itemTemplate: (option: T) => JSX.Element;
  onChange: (value: string) => void;
  optionLabel: string;
  optionValue: string;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  queryHook: (option: SimpleQueryApiArg) => any;
  querySelectedItem: (arg: string) => Promise<T>;
  selectName: string;
  selectedTemplate: (option: T) => JSX.Element;
  value?: string;
}

const BaseSelector = <T extends HasId>(props: BaseSelectorProps<T>) => {
  const [selectedItem, setSelectedItem] = useState<string>('');
  const [filter, setFilter] = useState<string>('');
  const [index, setIndex] = useState<number>(0);
  const [dataSource, setDataSource] = useState<T[]>([]);
  const [filteredDataSource, setFilteredDataSource] = useState<T[]>([]);

  const [simpleQuery, setSimpleQuery] = useState<SimpleQueryApiArg>({ first: 0, last: 40 });
  const query = props.queryHook(simpleQuery);

  const existingDataSourceIds = useMemo(() => new Set(dataSource.map(x => x.id)), [dataSource]);

  useEffect(() => {
    if (!query?.data) return;

    const newItems = query.data.filter((cn: T) => cn?.id && (!existingDataSourceIds.has(cn.id)));
    if (newItems.length > 0) {
      console.log('Adding new items', newItems.length)
      setDataSource(dataSource.concat(newItems));
      setIndex(dataSource.length + newItems.length);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [query]);

  useEffect(() => {
    if (filter) {
      if (dataSource.length > 0) {
        const filteredData = dataSource.filter(cn => cn?.[props.optionLabel]?.toLowerCase().includes(filter));
        setFilteredDataSource(filteredData);
        console.log('filtered', filteredData.length);
      }
    } else {
      setFilteredDataSource(dataSource);
      console.log('filter clear', dataSource.length);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dataSource, filter]);

  useEffect(() => {
    if (!props.value) return;
    if (selectedItem === props.value) {
      return;
    }

    setSelectedItem(props.value);
    try {
      props.querySelectedItem(props.value).then((item) => {
        if (item) {
          if (!existingDataSourceIds.has(item.id)) {
            console.log('Adding new item', item.name);
            const newDataSource = dataSource.concat(item);
            setDataSource(newDataSource);
            setIndex(newDataSource.length);
          }
        }
      }).catch((e) => { console.error(e) });

    } catch (e) {
      console.error(e);
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


  const loadingTemplate = (options: VirtualScrollerTemplateOptions) => {
    const className2 = classNames('flex align-items-center p-2', {
      odd: options.odd
    });

    return (
      <div className={className2} style={{ height: '50px' }}>
        <Skeleton height="1.3rem" width={options.even ? '60%' : '50%'} />
      </div>
    );
  };

  const onFilter = (event: DropdownFilterEvent) => {
    setFilter(event.filter.toLowerCase());
  }

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
        style={{
          ...{
            backgroundColor: 'var(--mask-bg)',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
          },
        }}
        value={selectedItem}
        valueTemplate={props.selectedTemplate}
        virtualScrollerOptions={{
          delay: 200,
          itemSize: props.itemSize,
          lazy: true,
          loadingTemplate: loadingTemplate,
          // eslint-disable-next-line @typescript-eslint/no-explicit-any
          onLazyLoad: (e: any) => {
            if (e.filter === '' && e.last as number >= index) {
              let firstRecord = e.first as number < index ? index : e.first as number;
              setSimpleQuery({ first: firstRecord, last: e.last as number + 100 } as SimpleQueryApiArg)
            }
          },
          showLoader: false,
        }}

      />
    </div>
  );
};

BaseSelector.displayName = 'BaseSelector';
BaseSelector.defaultProps = {
  className: null,
  disabled: false,
  editable: false,
  isLoading: true,
};

export default BaseSelector;
