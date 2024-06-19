import BanButton from '@components/buttons/BanButton';
import StringEditor, { StringEditorRef } from '@components/inputs/StringEditor';
import SMButton from '@components/sm/SMButton';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { Checkbox } from 'primereact/checkbox';
import { useMountEffect } from 'primereact/hooks';
import { ObjectUtils } from 'primereact/utils';
import { VirtualScroller } from 'primereact/virtualscroller';
import React, { useCallback, useEffect, useMemo } from 'react';
import { SMScrollerProperties } from './interfaces/SMScrollerProperties';

const SMScroller: React.FC<SMScrollerProperties> = ({ filter = false, itemSize = 26, scrollHeight = '40vh', ...props }) => {
  const { selectedItems, setSelectedItems } = useSelectedItems(props.selectedItemsKey ?? 'NONE');
  const [filterString, setFilterString] = React.useState<string>('');
  const [scrolled, setScrolled] = React.useState<boolean>(false);
  const virtualScrollerRef = React.useRef<VirtualScroller>(null);
  const stringEditorRef = React.useRef<StringEditorRef>(null);

  const filteredValues = useMemo(() => {
    // Early return if filter is undefined/false or filterBy is undefined
    if (!filter || !props.filterBy || filterString === '') {
      return props.data;
    }

    // Ensure props.filterBy is a string and exists in the item
    return props.data.filter((item: any) => {
      const filterKey = props.filterBy as keyof typeof item;
      const itemValue = item[filterKey];
      // Ensure itemValue is a string before calling .toLowerCase()
      return typeof itemValue === 'string' && itemValue.toLowerCase().includes(filterString.toLowerCase());
    });
  }, [filter, filterString, props.data, props.filterBy]);

  const equalityKey = useCallback(() => {
    return props.dataKey ? props.dataKey : 'Id';
  }, [props.dataKey]);

  const getOptionValue = useCallback(
    (option: any) => {
      return props.optionValue ? ObjectUtils.resolveFieldData(option, props.optionValue) : option && option['value'] !== undefined ? option['value'] : option;
    },
    [props.optionValue]
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
            return false;
          }
          return x !== item;
        });
        setSelectedItems(a);
        props.onChange && props.onChange(a);
        return true;
      }
      return false;
    },
    [equalityKey, props, selectedItems, setSelectedItems]
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
    (value: Record<string, any>, list: Record<string, any>[]) => {
      const key = equalityKey();

      if (key) {
        // Check if props.optionValue is defined and is a string
        if (props.optionValue && typeof props.optionValue === 'string') {
          // Check if the value has the key defined by equalityKey
          if (value[key] !== undefined) {
            // Find the index in the list where the optionValue matches
            const index = list.findIndex((item: Record<string, any>) => {
              // Ensure the item has the property defined by props.optionValue
              const itemValue = item[props.optionValue as keyof typeof item];
              return itemValue === value[key];
            });
            return index;
          }
        }

        // Fallback to using ObjectUtils.equals if no specific key is provided
        return list.findIndex((item: any) => ObjectUtils.equals(value, getOptionValue(item), key));
      }

      return -1; // Return -1 if no match is found or if key is undefined
    },
    [equalityKey, getOptionValue, props.optionValue]
  );

  const getSelectedOptionIndex = useCallback(() => {
    if (props.value != null) {
      return findOptionIndexInList(props.value, filteredValues);
    }

    return -1;
  }, [filteredValues, findOptionIndexInList, props.value]);

  const scrollTo = useCallback((index: number) => {
    // const virtualScrollerRef = listBoxRef.current?.getVirtualScroller();
    if (virtualScrollerRef) {
      setTimeout(() => virtualScrollerRef.current?.scrollToIndex(index), 0);
    }
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
        if (props.optionValue) {
          const toMatch = getOptionValue(item);
          if (props.value?.[key] !== undefined) {
            return toMatch === props.value[key];
          }
        }
        const a = ObjectUtils.equals(props.value, getOptionValue(item), key);
        return a;
      }
      return false;
    },
    [equalityKey, getOptionValue, props.optionValue, props.value]
  );

  useMountEffect(() => {
    setScrolled(false);
  });

  useEffect(() => {
    if (filteredValues) {
      if (props.value && !scrolled) {
        setScrolled(true);
        scrollToSelectedIndex();
      }
    }
  }, [filteredValues, props.value, scrolled, scrollToSelectedIndex]);

  const getItemTemplate = useCallback(
    (item: any) => {
      const classes = isSelected(item)
        ? 'sm-scroller-item p-highlight p-focus w-full flex align-items-center'
        : 'sm-scroller-item w-full flex align-items-center';

      if (props.select === true) {
        const ss = isSelectedItem(item);
        return (
          <div className="sm-scroller-item sm-border-top">
            <div className="flex align-items-center justify-content-start w-full">
              <div className="pr-1 sm-border-right">
                <Checkbox
                  onChange={(e) => {
                    e.preventDefault();
                    if (props.selectedItemsKey !== undefined && props.selectedItemsKey !== 'NONE') {
                      if (e.checked) {
                        setSelectedItems([...selectedItems, item]);
                        props.onChange && props.onChange([...selectedItems, item]);
                      } else {
                        removeSelectedItem(item);
                      }
                    }
                  }}
                  checked={ss}
                />
              </div>
              <div className="pl-1 flex align-items-center justify-content-start w-full">{props.itemTemplate?.(item) ?? ''}</div>
            </div>
          </div>
        );
      }
      return (
        <div
          onClick={() => {
            props.onChange && props.onChange(item);
          }}
          className={classes}
        >
          {props.itemTemplate?.(item) ?? ''}
        </div>
      );
    },
    [isSelected, isSelectedItem, props, removeSelectedItem, selectedItems, setSelectedItems]
  );

  const getDiv = useMemo(() => {
    let ret = props.className ? props.className : '';

    if (props.simple === true) {
      ret += '  w-full';
    } else {
      ret += ' sm-scroller sm-sm-input-border w-full ';
    }
    return ret;
  }, [props.className, props.simple]);

  return (
    <div className={getDiv}>
      <div className="sm-scroller-header">
        <div className="layout-padding-bottom" />
        {filter && (
          <div className="flex align-items-center justify-content-between gap-1 pr-2 sm-w-12">
            <div className="">
              <BanButton
                buttonDisabled={selectedItems.length === 0}
                onClick={() => {
                  setSelectedItems([]);
                  props.onChange && props.onChange([]);
                }}
                tooltip="Clear Selections"
              />
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
      </div>
      <div className="sm-scroller-items">
        <VirtualScroller ref={virtualScrollerRef} items={filteredValues} itemSize={itemSize} itemTemplate={getItemTemplate} scrollHeight={scrollHeight} />
      </div>
    </div>
  );
};

export default SMScroller;
