import { getTopToolOptions } from '@lib/common/common';
import { ResetLogoIcon } from '@lib/common/icons';
import { Button } from 'primereact/button';
import { useClickOutside } from 'primereact/hooks';

import { InputText } from 'primereact/inputtext';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useDebouncedCallback } from 'use-debounce';

export interface StringEditorBodyTemplateProperties {
  readonly autofocus?: boolean;
  readonly disableDebounce?: boolean;
  readonly debounceMs?: number;
  readonly isLoading?: boolean;
  readonly onChange: (value: string | undefined) => void;
  readonly onClick?: () => void;
  readonly placeholder?: string;
  readonly resetValue?: string | undefined;
  readonly showSave?: boolean;
  readonly tooltip?: string | undefined;
  readonly tooltipOptions?: TooltipOptions | undefined;
  readonly value: string | undefined;
  readonly isSearch?: boolean;
  readonly border?: boolean;
  readonly onFilterClear?: () => void;
}

const StringEditorBodyTemplate = (props: StringEditorBodyTemplateProperties) => {
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayReference = useRef<HTMLDivElement | null>(null);

  const [originalValue, setOriginalValue] = useState<string | undefined>('');
  const [inputValue, setInputValue] = useState<string | undefined>('');

  const debounced = useDebouncedCallback(
    useCallback(
      (value: string) => {
        if (value !== originalValue && !props.isLoading) {
          setInputValue(value);
          // setOriginalValue(value);
          props.onChange(value);
        }
      },
      [originalValue, props]
    ),
    props.debounceMs ? props.debounceMs : 1500,
    {}
  );

  const save = useCallback(
    (forceValueSave?: string | undefined) => {
      if (props.isLoading || (forceValueSave === undefined && (inputValue === undefined || inputValue === originalValue))) {
        return;
      }

      debounced.cancel();
      if (forceValueSave === undefined) {
        setOriginalValue(inputValue);
        props.onChange(inputValue);
      } else {
        setOriginalValue(forceValueSave);
        props.onChange(forceValueSave);
      }
    },
    [debounced, inputValue, originalValue, props]
  );

  // Keyboard Enter
  useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if (!isFocused) {
        return;
      }

      if (event.code === 'Enter' || event.code === 'NumpadEnter') {
        save();
      }
    };

    document.addEventListener('keydown', callback);

    return () => {
      document.removeEventListener('keydown', callback);
    };
  }, [isFocused, save]);

  useClickOutside(overlayReference, () => {
    if (!isFocused) {
      return;
    }

    setIsFocused(false);

    if (originalValue !== inputValue) {
      save();
    }
  });

  useEffect(() => {
    if (!props.isLoading && props.value !== null && props.value !== undefined) {
      setInputValue(props.value);
      setOriginalValue(props.value);
    } else {
      setInputValue('');
      setOriginalValue('');
    }
  }, [props.value, props.isLoading, setInputValue]);

  const needsSave = useMemo(() => {
    return originalValue !== inputValue;
  }, [inputValue, originalValue]);

  const getDiv = useMemo(() => {
    let ret = 'stringeditorbody-inputtext';
    if (props.isSearch === true) {
      ret = 'filter';
    }
    if (needsSave) {
      ret += ' save';
    }
    if (props.border) {
      ret += ' default-border';
    }
    return ret;
  }, [needsSave, props.border, props.isSearch]);

  return (
    <div className={'stringeditorbody relative'} ref={overlayReference}>
      {isFocused && props.resetValue !== undefined && props.resetValue !== inputValue && (
        <Button
          className="absolute right-0"
          disabled={props.isLoading}
          icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
          onClick={() => {
            setInputValue(props.resetValue === undefined ? '' : props.resetValue);
            save(props.resetValue);
          }}
          rounded
          severity="warning"
          size="small"
          text
          tooltip="Reset Name"
          tooltipOptions={getTopToolOptions}
        />
      )}

      {props.showSave && needsSave && <i className="absolute right-0 pt-1 pi pi-save pr-2 text-500" />}
      {props.isSearch && (
        <Button rounded text className="absolute right-0 pt-1 pi pi-filter-slash pr-2 text-500 w-3rem" onClick={() => props.onFilterClear?.()} />
      )}

      <InputText
        className={getDiv}
        autoFocus={props.autofocus}
        onChange={(e) => {
          setInputValue(e.target.value as string);
          if (!props.disableDebounce) {
            debounced(e.target.value as string);
          }
        }}
        onClick={() => {
          props.onClick?.();
        }}
        onFocus={() => setIsFocused(true)}
        placeholder={props.placeholder}
        tooltip={props.tooltip}
        tooltipOptions={props.tooltipOptions}
        value={inputValue}
      />
    </div>
  );
};

StringEditorBodyTemplate.displayName = 'String Editor Body Template';

export default memo(StringEditorBodyTemplate);
