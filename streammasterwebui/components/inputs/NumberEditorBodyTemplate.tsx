import { getTopToolOptions } from '@lib/common/common';
import { ResetLogoIcon } from '@lib/common/icons';
import { Button } from 'primereact/button';
import { useClickOutside } from 'primereact/hooks';
import { InputNumber } from 'primereact/inputnumber';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useDebouncedCallback } from 'use-debounce';

export interface NumberEditorBodyTemplateProperties {
  readonly onChange: (value: number) => void;
  readonly onClick?: () => void;
  readonly prefix?: string | undefined;
  readonly resetValue?: number | undefined;
  readonly showSave?: boolean;
  readonly suffix?: string | undefined;
  readonly tooltip?: string | undefined;
  readonly tooltipOptions?: TooltipOptions | undefined;
  readonly value: number | undefined;
}

const NumberEditorBodyTemplate = (props: NumberEditorBodyTemplateProperties) => {
  const [inputValue, setInputValue] = useState<number>(0);
  const [originalValue, setOriginalValue] = useState<number>(0);
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayReference = useRef(null);

  const debounced = useDebouncedCallback(
    useCallback(
      (value) => {
        if (value !== originalValue) {
          setInputValue(value);
          setOriginalValue(value);
          props.onChange(value);
        }
      },
      [originalValue, props]
    ),
    1500,
    {}
  );

  const save = useCallback(
    (forceValueSave?: number | undefined) => {
      if (forceValueSave === undefined && (inputValue === undefined || inputValue === originalValue)) {
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
    if (props.value !== undefined) {
      setInputValue(props.value);
      setOriginalValue(props.value);
    }
  }, [props.value, setInputValue]);

  const needsSave = useMemo(() => {
    return originalValue !== inputValue;
  }, [inputValue, originalValue]);

  return (
    <div className="numbereditorbody relative h-full" ref={overlayReference}>
      {isFocused && props.resetValue !== undefined && props.resetValue !== 0 && props.resetValue !== inputValue && (
        <Button
          className="absolute mt-1 right-0"
          icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
          onClick={() => {
            setInputValue(props.resetValue === undefined ? 0 : props.resetValue);
            save(props.resetValue);
          }}
          rounded
          severity="warning"
          size="small"
          tooltip="Reset ChNo"
          tooltipOptions={getTopToolOptions}
        />
      )}
      <InputNumber
        className={needsSave ? 'save' : undefined}
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

export default memo(NumberEditorBodyTemplate);
