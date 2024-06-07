import BanButton from '@components/buttons/BanButton';
import StringEditor, { StringEditorRef } from '@components/inputs/StringEditor';
import SMButton from '@components/sm/SMButton';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { Checkbox } from 'primereact/checkbox';
import { useMountEffect } from 'primereact/hooks';
import { ObjectUtils } from 'primereact/utils';
import { VirtualScroller } from 'primereact/virtualscroller';
import React, { useCallback, useEffect, useMemo } from 'react';

interface SMScrollerProps {
  readonly className?: string;
  readonly data: any;
  readonly dataKey?: string;
  readonly filter?: boolean;
  readonly filterBy?: string;
  readonly itemSize?: number;
  readonly itemTemplate: (item: any) => React.ReactNode;
  readonly onChange?: (value: any) => void;
  readonly optionValue?: string;
  readonly scrollHeight?: string;
  readonly select?: boolean;
  readonly selectedItemsKey?: string;
  readonly simple?: boolean;
  readonly value?: any;
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
  simple,
  value,
  scrollHeight = '40vh'
}) => {
  const { selectedItems, setSelectedItems } = useSelectedItems(selectedItemsKey ?? 'NONE');
  const [filterString, setFilterString] = React.useState<string>('');
  const [scrolled, setScrolled] = React.useState<boolean>(true);
  const virtualScrollerRef = React.useRef<VirtualScroller>(null);
  const stringEditorRef = React.useRef<StringEditorRef>(null);

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
    return dataKey ? dataKey : 'Id';
  }, [dataKey]);

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
        const a = selectedItems.filter((x: any) => {
          if (x[key] !== undefined) {
            if (x[key] !== item[key]) {
              setScrolled(false);
              return true;
            }
          }
          return x !== item;
        });
        setSelectedItems(a);
        onChange && onChange(a);
        return true;
      }
      return false;
    },
    [equalityKey, onChange, selectedItems, setSelectedItems]
  );

  const isSelectedItem = useCallback(
    (toTest: any) => {
      const key = equalityKey();
      if (key) {
        const test = selectedItems.findIndex((item: any) => ObjectUtils.equals(toTest, getOptionValue(item), key));
        return test !== -1;
      }
      return false;
    },
    [equalityKey, getOptionValue, selectedItems]
  );

  const findOptionIndexInList = useCallback(
    (value: any, list: any) => {
      const key = equalityKey();
      if (key) {
        if (optionValue) {
          if (value[key] !== undefined) {
            const test = list.findIndex((item: any) => value[key] === item[optionValue]);
            return test;
          }
        }
        return list.findIndex((item: any) => ObjectUtils.equals(value, getOptionValue(item), key));
      }
    },
    [equalityKey, getOptionValue, optionValue]
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
        setTimeout(() => scrollTo(selectedIndex <= 1 ? 1 : selectedIndex - 1), 0);
      } else {
        scrollTo(1);
      }
    }
  }, [getSelectedOptionIndex, scrollTo]);

  const isSelected = useCallback(
    (item: any) => {
      const key = equalityKey();
      if (key) {
        if (optionValue) {
          const toMatch = getOptionValue(item);
          if (value[key] !== undefined) {
            return toMatch === value[key];
          }
        }
        const a = ObjectUtils.equals(value, getOptionValue(item), key);
        return a;
      }
      return false;
    },
    [equalityKey, getOptionValue, optionValue, value]
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
          <div className="flex align-items-center justify-content-start pl-1 sm-scroller-item">
            <div className="w-1">
              <Checkbox
                onChange={(e) => {
                  e.preventDefault();
                  if (selectedItemsKey !== undefined && selectedItemsKey !== 'NONE') {
                    if (e.checked) {
                      setSelectedItems([...selectedItems, item]);
                      onChange && onChange([...selectedItems, item]);
                    } else {
                      removeSelectedItem(item);
                    }
                  }
                }}
                checked={ss}
              />
            </div>
            <div className="w-11 pl-1">{itemTemplate(item)}</div>
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
    [isSelected, isSelectedItem, itemTemplate, onChange, removeSelectedItem, select, selectedItems, selectedItemsKey, setSelectedItems]
  );

  const getDiv = useMemo(() => {
    if (simple === true) {
      return `dark-background w-full ${className} `;
    }
    return `sm-scroller sm-sm-input-border w-full ${className} `;
  }, [className, simple]);

  return (
    <div className={getDiv}>
      <div className="layout-padding-bottom" />
      {filter && (
        <div className="flex align-items-center justify-content-between gap-1 pr-2 sm-w-12">
          <div className="">
            <BanButton disabled={selectedItems.length === 0} onClick={() => setSelectedItems([])} tooltip="Clear Selections" />
          </div>

          <div className="flex align-items-center justify-content-between sm-w-11">
            <div className="sm-w-11">
              <StringEditor
                autoFocus
                disableDebounce
                darkBackGround
                placeholder="Name"
                value={filterString}
                showClear
                onChange={(value) => {
                  if (value !== undefined) {
                    setFilterString(value);
                    scrollTo(1);
                  }
                }}
                onSave={(value) => {}}
                ref={stringEditorRef}
              />
            </div>
            <div>
              <SMButton
                iconFilled={false}
                icon="pi-filter-slash"
                onClick={() => {
                  setFilterString('');
                  stringEditorRef.current?.clear();
                  scrollTo(1);
                }}
              />
            </div>
          </div>
        </div>
      )}

      <div className="layout-padding-bottom" />
      <div className="sm-scroller-items block">
        <VirtualScroller ref={virtualScrollerRef} items={filteredValues} itemSize={itemSize} itemTemplate={getItemTemplate} scrollHeight={scrollHeight} />
      </div>
    </div>
  );
};

export default SMScroller;
