import { FloatLabel } from 'primereact/floatlabel';
import { useClickOutside } from 'primereact/hooks';
import { InputNumber } from 'primereact/inputnumber';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useDebouncedCallback } from 'use-debounce';

export interface NumberEditorTemplateProperties {
  readonly onChange: (value: number) => void;
  readonly disableDebounce?: boolean;
  readonly onClick?: () => void;
  readonly label?: string;
  readonly prefix?: string | undefined;
  readonly resetValue?: number | undefined;
  readonly showSave?: boolean;
  readonly suffix?: string | undefined;
  readonly tooltip?: string | undefined;
  readonly tooltipOptions?: TooltipOptions | undefined;
  readonly value: number | undefined;
  readonly darkBackGround?: boolean;
}

const NumberEditor = (props: NumberEditorTemplateProperties) => {
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

  const needsSave = useMemo(() => {
    return originalValue !== inputValue;
  }, [inputValue, originalValue]);

  const getDiv = useMemo(() => {
    let ret = 'stringeditorbody-inputtext';
    if (props.darkBackGround === true) {
      ret = 'stringeditorbody-inputtext-dark';
    }
    if (needsSave) {
      ret = 'stringeditorbody-inputtext-save';
    }

    return ret;
  }, [needsSave, props.darkBackGround]);

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

  if (!props.label) {
    return (
      <div className={getDiv} ref={overlayReference}>
        <InputNumber
          id="numbereditorbody-inputtext"
          locale="en-US"
          onChange={(e) => {
            if (props.disableDebounce !== undefined && props.disableDebounce !== true) {
              debounced(e.value as number);
            }

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
  }

  return (
    <div className={props.label ? 'pt-4' : ''}>
      <FloatLabel>
        <InputNumber
          className={getDiv}
          id="numbereditorbody-inputtext"
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
        <label htmlFor="numbereditorbody-inputtext">{props.label}</label>
      </FloatLabel>
    </div>
  );
};

export default memo(NumberEditor);
