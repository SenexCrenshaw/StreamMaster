import { useState, useEffect, useCallback } from 'react';
import { type DropdownFilterEvent } from 'primereact/dropdown';
import { type DropdownChangeEvent } from 'primereact/dropdown';
import { Dropdown } from 'primereact/dropdown';
import { Skeleton } from 'primereact/skeleton';
import { classNames } from 'primereact/utils';
import { type HasId, type GetApiArg, type SimpleQueryApiArg, type SMDataTableFilterMetaData, isEmptyObject } from "../../common/common";
import { addOrUpdateValueForField } from "../../common/common";
import { type VirtualScrollerTemplateOptions } from 'primereact/virtualscroller';

export type BaseSelectorProps<T extends HasId> = {
  className?: string | null;
  data: T[];
  disabled?: boolean;
  editable?: boolean | undefined;
  fetch: (arg: string) => Promise<T>;
  filteredData: T[];
  isLoading?: boolean;
  itemSize: number[] | number | undefined;
  itemTemplate: (option: T) => JSX.Element;
  onChange: (value: string) => void;
  onFilter: (value: GetApiArg) => void;
  onPaging: (value: SimpleQueryApiArg) => void;
  optionLabel: string;
  optionValue: string;
  selectName: string;
  selectedTemplate: (option: T) => JSX.Element;
  value?: string;
}

const BaseSelector = <T extends HasId>(props: BaseSelectorProps<T>) => {
  const [selectedItem, setSelectedItem] = useState<string>('');
  const [selectedValue, setSelectedValue] = useState<T>({} as T);
  const [filter, setFilter] = useState<string>('');
  const [index, setIndex] = useState<number>(0);
  const [dataSource, setDataSource] = useState<T[]>([]);
  const [oldDataSource, setOldDataSource] = useState<T[]>([]);

  useEffect(() => {
    if (filter) {

      if (props.filteredData && props.filteredData.length > 0) {
        setOldDataSource([...dataSource]);

        // Create a set of ids/sources for faster lookups
        const existingIds = new Set(dataSource.map(x => x.id));

        const newItems = props.filteredData.filter(cn =>
          cn?.id && (!existingIds.has(cn.id))
        );

        setDataSource([...newItems]);
        setIndex(newItems.length);
      }
    } else {
      if (oldDataSource.length > 0) {
        setDataSource([...oldDataSource]);
        setIndex(oldDataSource.length);
        setOldDataSource([]);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.filteredData, filter]);


  useEffect(() => {
    if (props.data !== undefined && props.data.length > 0) {

      const existingIds = new Set(dataSource.map(x => x.id));

      const newItems = props.data.filter(cn =>
        cn?.id && (!existingIds.has(cn.id))
      );
      if (newItems.length > 0) {
        // Use spread operator to combine arrays
        const newDataSource = [...dataSource, ...newItems];

        setIndex(newDataSource.length);
        setDataSource(newDataSource);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.data]);

  useEffect(() => {
    if (!props.value) return;
    // const fetchAndSetIcon = async () => {
    if (selectedItem !== props.value) {
      // Check for undefined, null, or empty string
      setSelectedItem(props.value);
      try {
        props.fetch(props.value).then((item) => {
          if (item) {
            console.log(' item', dataSource.map(existingItem => existingItem.id));
            setSelectedValue(item);
          }
        }).catch((e) => { console.error(e) });

      } catch (e) {
        console.error(e);
      }

    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.value]);

  useEffect(() => {
    if (!isEmptyObject(selectedValue)) {

      const existingIds = new Set(dataSource.map(x => x.id));
      if (!existingIds.has(selectedValue.id)) {
        const newDataSource = [...dataSource, selectedValue];
        setDataSource(newDataSource);
        setIndex(newDataSource.length);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedValue]);

  const onChange = useCallback((event: DropdownChangeEvent) => {
    setSelectedItem(event.value);

    if (!event.value || !props.onChange) return;

    props.onChange(event.value);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

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
    const toSend = [] as SMDataTableFilterMetaData[];
    addOrUpdateValueForField(toSend, 'name', 'contains', event.filter);
    setFilter(JSON.stringify(toSend));
    props.onFilter?.({ jsonFiltersString: JSON.stringify(toSend), pageSize: 40 } as GetApiArg);
  }

  return (
    <div className="BaseSelector flex align-contents-center w-full min-w-full min-w-10rem" >
      <Dropdown
        className={className}
        disabled={props.disabled}
        editable={props.editable}
        filter
        filterBy={props.optionLabel}
        filterInputAutoFocus
        itemTemplate={props.itemTemplate}
        onChange={onChange}
        onFilter={onFilter}
        optionLabel={props.optionLabel}
        optionValue={props.optionValue}
        options={dataSource}
        placeholder={`Select an ${props.selectName}`}
        scrollHeight="40vh"
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
              props.onPaging?.({ first: firstRecord, last: e.last as number + 100 } as SimpleQueryApiArg);
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
  isLoading: false,
};

export default BaseSelector;
