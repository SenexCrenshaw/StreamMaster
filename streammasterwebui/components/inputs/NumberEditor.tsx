import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { useClickOutside } from 'primereact/hooks';
import { InputNumber } from 'primereact/inputnumber';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useDebouncedCallback } from 'use-debounce';

interface NumberEditorTemplateProperties {
  readonly autoFocus?: boolean;
  readonly darkBackGround?: boolean;
  readonly debounceMs?: number;
  readonly disableDebounce?: boolean;
  readonly isLoading?: boolean;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly max?: number;
  readonly min?: number;
  readonly onChange?: (value: number) => void;
  readonly onClick?: () => void;
  readonly onSave?: (value: number) => void;
  readonly prefix?: string | undefined;
  readonly resetValue?: number | undefined;
  readonly showButtons?: boolean;
  readonly showSave?: boolean;
  readonly suffix?: string | undefined;
  readonly tooltip?: string | undefined;
  readonly tooltipOptions?: TooltipOptions | undefined;
  readonly value: number | undefined;
}

const NumberEditor = ({
  autoFocus,
  darkBackGround,
  debounceMs = 1500,
  disableDebounce = false,
  isLoading,
  label,
  labelInline = false,
  max = 99999,
  min = 0,
  onChange,
  onClick,
  onSave,
  prefix,
  showButtons,
  suffix,
  tooltip,
  tooltipOptions,
  value
}: NumberEditorTemplateProperties) => {
  const divReference = useRef<HTMLDivElement | null>(null);
  const [inputValue, setInputValue] = useState<number>(0);
  const [originalValue, setOriginalValue] = useState<number>(0);
  const [ignoreSave, setIgnoreSave] = useState<boolean>(false);
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayReference = useRef(null);
  const { code } = useScrollAndKeyEvents();
  const inputRef = useRef<InputNumber>(null);

  useEffect(() => {
    if (autoFocus && inputRef.current) {
      inputRef.current.focus();
    }
  }, [autoFocus]);

  const save = useCallback(
    (forceValueSave?: number | undefined) => {
      setIgnoreSave(true);

      if (forceValueSave === undefined) {
        onSave && onSave(inputValue);
      } else {
        onSave && onSave(forceValueSave);
      }
    },
    [inputValue, onSave]
  );

  const debounced = useDebouncedCallback(
    useCallback(
      (newValue: number) => {
        if (newValue !== originalValue && isLoading !== true) {
          save(newValue);
        }
      },
      [isLoading, originalValue, save]
    ),
    debounceMs,
    {}
  );

  const needsSave = useMemo(() => {
    return originalValue !== inputValue;
  }, [inputValue, originalValue]);

  useEffect(() => {
    if (code === 'Enter' || code === 'NumpadEnter') {
      if (needsSave && !ignoreSave) {
        debounced.cancel();
        save();
      }
    }
  }, [code, debounced, ignoreSave, needsSave, save]);

  useClickOutside(divReference, () => {
    if (!isFocused) {
      return;
    }
    setIsFocused(false);
    if (disableDebounce !== undefined && disableDebounce !== true && originalValue !== inputValue && !ignoreSave) {
      save();
    }
  });

  const getDiv = useMemo(() => {
    let ret = 'flex stringeditor justify-content-center';

    if (label && !labelInline) {
      ret += ' flex-column';
    }

    if (labelInline) {
      ret += ' align-items-start';
    } else {
      ret += ' align-items-center';
    }

    return ret;
  }, [label, labelInline]);

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
    setIgnoreSave(false);
  }, [value, setInputValue]);
  const inputGetDiv = useMemo(() => {
    let ret = 'sm-input';
    if (darkBackGround) {
      ret += '-dark';
    }
    if (labelInline) {
      ret += ' w-12';
    }

    return ret;
  }, [darkBackGround, labelInline]);

  return (
    <>
      {label && !labelInline && (
        <>
          <label className="pl-15">{label.toUpperCase()}</label>
          {/* <div className="pt-small" /> */}
        </>
      )}
      <div ref={divReference} className={getDiv}>
        {label && labelInline && <div className="w-6">{label.toUpperCase()}</div>}
        <InputNumber
          className={inputGetDiv}
          disabled={isLoading}
          min={min}
          max={max}
          locale="en-US"
          onChange={(e) => {
            setInputValue(e.value as number);
            if (disableDebounce !== true) {
              debounced(e.value as number);
            }
            onChange && onChange(e.value as number);
          }}
          ref={inputRef}
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
    </>
  );
};

export default memo(NumberEditor);
