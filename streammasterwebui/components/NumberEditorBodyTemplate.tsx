import { getTopToolOptions } from '@lib/common/common';
import { ResetLogoIcon } from '@lib/common/icons';
import { Button } from 'primereact/button';
import { useClickOutside } from 'primereact/hooks';
import { InputNumber } from 'primereact/inputnumber';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';

import { memo, useCallback, useEffect, useRef, useState, type CSSProperties } from 'react';
import { useDebouncedCallback } from 'use-debounce';

const NumberEditorBodyTemplate = (props: NumberEditorBodyTemplateProps) => {
  const [inputValue, setInputValue] = useState<number>(0);
  const [originalValue, setOriginalValue] = useState<number>(0);
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayRef = useRef(null);

  const debounced = useDebouncedCallback(
    useCallback(
      (value) => {
        if (value !== originalValue) {
          setInputValue(value);
          props.onChange(value);
        }
      },
      [originalValue, props],
    ),
    1500,
    {},
  );

  const save = useCallback(
    (forceValueSave?: number | undefined) => {
      if (forceValueSave === undefined && (inputValue === undefined || inputValue === originalValue)) {
        return;
      }

      debounced.cancel();

      if (forceValueSave !== undefined) {
        props.onChange(forceValueSave);
      } else {
        props.onChange(inputValue);
      }
    },
    [debounced, inputValue, originalValue, props],
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

  useClickOutside(overlayRef, () => {
    if (!isFocused) {
      return;
    }

    setIsFocused(false);

    if (originalValue !== inputValue) {
      save();
    }
  });

  useEffect(() => {
    if (props.value !== undefined) {
      setInputValue(props.value);
      setOriginalValue(props.value);
    }
  }, [props.value, setInputValue]);

  return (
    <div className="relative h-full" ref={overlayRef} style={props.style}>
      {isFocused && props.resetValue !== undefined && props.resetValue !== 0 && props.resetValue !== inputValue && (
        <Button
          className="absolute mt-1 right-0"
          icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
          onClick={() => {
            setInputValue(props.resetValue !== undefined ? props.resetValue : 0);
            save(props.resetValue);
          }}
          rounded
          severity="warning"
          size="small"
          tooltip="Reset ChNo"
          tooltipOptions={getTopToolOptions}
        />
      )}
      {originalValue !== inputValue && <i className="absolute right-0 pt-1 pi pi-save pr-2 text-500" />}
      <InputNumber
        className="w-full h-full"
        locale="en-US"
        onChange={(e) => {
          debounced(e.value as number);
          setInputValue(e.value as number);
        }}
        onClick={() => {
          props.onClick?.();
        }}
        onFocus={() => setIsFocused(true)}
        prefix={props.prefix}
        suffix={props.suffix}
        tooltip={props.tooltip}
        tooltipOptions={props.tooltipOptions}
        value={inputValue}
      />
    </div>
  );
};

export type NumberEditorBodyTemplateProps = {
  readonly onChange: (value: number) => void;
  readonly onClick?: () => void;
  // onReset?: ((value: string) => void);
  readonly prefix?: string | undefined;
  readonly resetValue?: number | undefined;
  readonly style?: CSSProperties;
  readonly suffix?: string | undefined;
  readonly tooltip?: string | undefined;
  readonly tooltipOptions?: TooltipOptions | undefined;
  readonly value: number | undefined;
};

export default memo(NumberEditorBodyTemplate);
