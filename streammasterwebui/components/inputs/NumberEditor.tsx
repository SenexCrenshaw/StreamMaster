import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { useClickOutside } from 'primereact/hooks';
import { InputNumber } from 'primereact/inputnumber';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useDebouncedCallback } from 'use-debounce';

interface NumberEditorTemplateProperties {
  readonly autoFocus?: boolean;
  readonly onChange?: (value: number) => void;
  readonly onSave?: (value: number | undefined) => void;
  readonly debounceMs?: number;
  readonly disableDebounce?: boolean;
  readonly isLoading?: boolean;
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
  disableDebounce = true,
  label,
  debounceMs = 1500,
  isLoading,
  onChange,
  onSave,
  onClick,
  prefix,
  suffix,
  tooltip,
  tooltipOptions,
  showButtons,
  value,
  darkBackGround
}: NumberEditorTemplateProperties) => {
  const divReference = useRef<HTMLDivElement | null>(null);
  const [inputValue, setInputValue] = useState<number>(0);
  const [originalValue, setOriginalValue] = useState<number>(0);
  const [ignoreSave, setIgnoreSave] = useState<boolean>(false);
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayReference = useRef(null);
  const { code } = useScrollAndKeyEvents();

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

  useEffect(() => {
    if (code === 'Enter' || code === 'NumpadEnter') {
      if (needsSave && !ignoreSave) {
        debounced.cancel();
        save();
      }
    }
  }, [code, debounced, ignoreSave, needsSave, save]);

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
    <div ref={divReference} className="flex flex-column align-items-start">
      {label && (
        <>
          <label className="pl-15" htmlFor="numbereditorbody-inputtext">
            {label.toUpperCase()}
          </label>
          <div className="pt-small" />
        </>
      )}
      <InputNumber
        className={getDiv}
        min={0}
        id="numbereditorbody-inputtext"
        locale="en-US"
        onChange={(e) => {
          setInputValue(e.value as number);
          if (disableDebounce !== true) {
            debounced(e.value as number);
          }
          onChange && onChange(e.value as number);
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
