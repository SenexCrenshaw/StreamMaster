import { useClickOutside } from 'primereact/hooks';
import { InputNumber } from 'primereact/inputnumber';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useDebouncedCallback } from 'use-debounce';

interface NumberEditorTemplateProperties {
  readonly autoFocus?: boolean;
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
  readonly showButtons?: boolean;
}

const NumberEditor = ({
  autoFocus,
  disableDebounce = false,
  label,
  onChange,
  onClick,
  prefix,
  suffix,
  tooltip,
  tooltipOptions,
  showButtons,
  value,
  darkBackGround
}: NumberEditorTemplateProperties) => {
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
          onChange(value);
        }
      },
      [onChange, originalValue]
    ),
    1500,
    {}
  );

  const needsSave = useMemo(() => {
    return originalValue !== inputValue;
  }, [inputValue, originalValue]);

  const getDiv = useMemo(() => {
    let ret = 'stringeditorbody-inputtext';
    if (darkBackGround === true) {
      ret = 'stringeditorbody-inputtext-dark';
    }
    if (needsSave) {
      ret = 'stringeditorbody-inputtext-save';
    }

    if (showButtons === true) {
      ret += ' stringeditorbody-inputtext-buttons';
    }
    return ret;
  }, [darkBackGround, needsSave, showButtons]);

  const save = useCallback(
    (forceValueSave?: number | undefined) => {
      if (forceValueSave === undefined && (inputValue === undefined || inputValue === originalValue)) {
        return;
      }

      debounced.cancel();

      if (forceValueSave === undefined) {
        setOriginalValue(inputValue);
        onChange(inputValue);
      } else {
        setOriginalValue(forceValueSave);
        onChange(forceValueSave);
      }
    },
    [debounced, inputValue, onChange, originalValue]
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
    if (value !== undefined) {
      setInputValue(value);
      setOriginalValue(value);
    }
  }, [value, setInputValue]);

  return (
    <div className="flex flex-column align-items-start">
      {label && (
        <>
          <label htmlFor="numbereditorbody-inputtext">{label}</label>
          <div className="pt-small" />
        </>
      )}
      <InputNumber
        className={getDiv}
        min={0}
        id="numbereditorbody-inputtext"
        locale="en-US"
        onChange={(e) => {
          debounced(e.value as number);
          setInputValue(e.value as number);
        }}
        onClick={() => {
          onClick?.();
        }}
        onFocus={() => setIsFocused(true)}
        prefix={prefix}
        showButtons={showButtons}
        suffix={suffix}
        tooltip={tooltip}
        tooltipOptions={tooltipOptions}
        value={inputValue}
      />
    </div>
  );
};

export default memo(NumberEditor);
