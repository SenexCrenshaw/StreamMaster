import BaseButton from '@components/buttons/BaseButton';
import StringEditor from '@components/inputs/StringEditor';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { Checkbox } from 'primereact/checkbox';
import { useMountEffect } from 'primereact/hooks';
import { ObjectUtils } from 'primereact/utils';
import { VirtualScroller } from 'primereact/virtualscroller';
import React, { useCallback, useEffect, useMemo } from 'react';

interface SMScrollerProps {
  readonly className?: string;
  readonly style?: React.CSSProperties;
  readonly data: any;
  readonly itemTemplate: (item: any) => React.ReactNode;
  readonly value?: any;
  readonly dataKey?: string;
  readonly filterBy?: string;
  readonly optionValue?: string;
  readonly itemSize?: number;
  readonly scrollHeight?: number;
  readonly filter?: boolean;
  readonly select?: boolean;
  readonly selectedItemsKey?: string;
  readonly onChange?: (value: any) => void;
}

const SMScroller: React.FC<SMScrollerProps> = ({
  data,
  dataKey,
  className,
  filter = false,
  filterBy,
  itemSize = 26,
  itemTemplate,
  onChange,
  optionValue,
  select,
  selectedItemsKey,
  style,
  value,
  scrollHeight = 100
}) => {
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems(selectedItemsKey ?? 'NONE');
  const [filterString, setFilterString] = React.useState<string>('');
  const [scrolled, setScrolled] = React.useState<boolean>(true);
  const virtualScrollerRef = React.useRef<VirtualScroller>(null);

  const filteredValues = useMemo(() => {
    setScrolled(false);
    if (filter === undefined || filter === false || filterBy === undefined) {
      return data;
    }
    if (filter && filterString !== '') {
      return data.filter((item: any) => item[filterBy].toLowerCase().includes(filterString.toLowerCase()));
    }
    return data;
  }, [data, filter, filterBy, filterString]);

  const equalityKey = useCallback(() => {
    return optionValue ? null : dataKey;
  }, [dataKey, optionValue]);

  const getOptionValue = useCallback(
    (option: any) => {
      return optionValue ? ObjectUtils.resolveFieldData(option, optionValue) : option && option['value'] !== undefined ? option['value'] : option;
    },
    [optionValue]
  );

  const removeSelectedItem = useCallback(
    (item: any) => {
      const key = equalityKey();

      if (key) {
        const a = selectSelectedItems.filter((x: any) => {
          if (x[key] !== undefined) {
            if (x[key] !== item[key]) {
              setScrolled(false);
              return true;
            }
          }
          return false;
        });
        setSelectSelectedItems(a);

        return true;
      }
      return false;
    },
    [equalityKey, selectSelectedItems, setSelectSelectedItems]
  );

  const isSelectedItem = useCallback(
    (toTest: any) => {
      const key = equalityKey();
      if (key) {
        const test = selectSelectedItems.findIndex((item: any) => ObjectUtils.equals(toTest, getOptionValue(item), key));
        return test !== -1;
      }
      return false;
    },
    [equalityKey, getOptionValue, selectSelectedItems]
  );

  const findOptionIndexInList = useCallback(
    (value: any, list: any) => {
      const key = equalityKey();
      if (key) {
        return list.findIndex((item: any) => ObjectUtils.equals(value, getOptionValue(item), key));
      }
    },
    [equalityKey, getOptionValue]
  );

  const getSelectedOptionIndex = useCallback(() => {
    if (value != null) {
      return findOptionIndexInList(value, filteredValues);
    }

    return -1;
  }, [filteredValues, findOptionIndexInList, value]);

  const scrollTo = useCallback((index: number) => {
    setTimeout(() => virtualScrollerRef.current?.scrollToIndex(index), 0);
  }, []);

  const scrollToSelectedIndex = useCallback(() => {
    if (virtualScrollerRef.current) {
      const selectedIndex = getSelectedOptionIndex();
      if (selectedIndex !== -1) {
        setTimeout(() => scrollTo(selectedIndex === 1 ? 1 : selectedIndex - 1), 0);
      } else {
        scrollTo(1);
      }
    }
  }, [getSelectedOptionIndex, scrollTo]);

  const isSelected = useCallback(
    (item: any) => {
      const key = equalityKey();
      if (key) {
        const a = ObjectUtils.equals(value, getOptionValue(item), key);
        return a;
      }
      return false;
    },
    [equalityKey, getOptionValue, value]
  );

  useMountEffect(() => {
    setScrolled(false);
  });

  useEffect(() => {
    if (filteredValues) {
      if (value && !scrolled) {
        setScrolled(true);
        scrollToSelectedIndex();
      }
    }
  }, [scrollToSelectedIndex, scrolled, value, filteredValues]);

  const getItemTemplate = useCallback(
    (item: any) => {
      const classes = isSelected(item)
        ? 'sm-scroller-item p-highlight p-focus w-full flex align-items-center'
        : 'sm-scroller-item w-full flex align-items-center';

      if (select === true) {
        const ss = isSelectedItem(item);
        return (
          <div
            className="flex align-items-center"
            onClick={() => {
              onChange && onChange(item);
            }}
          >
            <Checkbox
              onChange={(e) => {
                if (selectedItemsKey !== undefined && selectedItemsKey !== 'NONE') {
                  if (e.checked) {
                    setSelectSelectedItems([...selectSelectedItems, item]);
                  } else {
                    removeSelectedItem(item);
                  }
                }
              }}
              checked={ss}
            />
            <div className="pl-1">{itemTemplate(item)}</div>
          </div>
        );
      }
      return (
        <div
          onClick={() => {
            onChange && onChange(item);
          }}
          className={classes}
        >
          {itemTemplate(item)}
        </div>
      );
    },
    [isSelected, isSelectedItem, itemTemplate, onChange, removeSelectedItem, select, selectSelectedItems, selectedItemsKey, setSelectSelectedItems]
  );

  return (
    <div className={`sm-scroller sm-input-border w-full ${className} `}>
      {filter && (
        <div className="flex align-items-center justify-content-between px-2 pt-1 ">
          <div className="w-11">
            <StringEditor
              disableDebounce
              darkBackGround
              placeholder="Filter..."
              value={filterString}
              onChange={(value) => {
                if (value !== undefined) {
                  setFilterString(value);
                  scrollTo(1);
                }
              }}
              onSave={(value) => {}}
            />
          </div>
          <BaseButton iconFilled={false} icon="pi-filter-slash" onClick={() => setFilterString('')} />
        </div>
      )}

      <div className="layout-padding-bottom-lg" />
      <div className="sm-scroller-items block">
        <VirtualScroller
          ref={virtualScrollerRef}
          items={filteredValues}
          itemSize={itemSize}
          itemTemplate={getItemTemplate}
          scrollHeight={`${scrollHeight}px`}
        />
      </div>
    </div>
  );
};

export default SMScroller;
