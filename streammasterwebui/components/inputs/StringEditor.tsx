import { useClickOutside } from 'primereact/hooks';

import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { InputText } from 'primereact/inputtext';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useDebouncedCallback } from 'use-debounce';
import { v4 as uuidv4 } from 'uuid';

export interface StringEditorBodyTemplateProperties {
  readonly autoFocus?: boolean;
  readonly disableDebounce?: boolean;
  readonly disabled?: boolean;
  readonly debounceMs?: number;
  readonly isLoading?: boolean;
  readonly label?: string;
  readonly onChange?: (value: string | undefined) => void;
  readonly onSave: (value: string | undefined) => void;
  readonly onClick?: () => void;
  readonly placeholder?: string;
  readonly resetValue?: string | undefined;
  readonly showClear?: boolean;
  readonly tooltip?: string | undefined;
  readonly tooltipOptions?: TooltipOptions | undefined;
  readonly value: string | undefined;
  readonly darkBackGround?: boolean;
}

const StringEditor = ({
  autoFocus,
  disableDebounce = false,
  disabled = false,
  debounceMs = 1500,
  isLoading,
  label,
  onChange,
  onSave,
  onClick,
  placeholder,
  showClear = false,
  tooltip,
  tooltipOptions,
  value,
  darkBackGround
}: StringEditorBodyTemplateProperties) => {
  const uuid = uuidv4();
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const divReference = useRef<HTMLDivElement | null>(null);
  const [ignoreSave, setIgnoreSave] = useState<boolean>(false);
  const [originalValue, setOriginalValue] = useState<string | undefined>(undefined);
  const [inputValue, setInputValue] = useState<string | undefined>('');
  const { code } = useScrollAndKeyEvents();
  const inputRef = useRef<HTMLInputElement>(null);

  const save = useCallback(
    (forceValueSave?: string | undefined) => {
      setIgnoreSave(true);

      if (forceValueSave === undefined) {
        onSave(inputValue);
      } else {
        onSave(forceValueSave);
      }
    },
    [inputValue, onSave]
  );

  const debounced = useDebouncedCallback(
    useCallback(
      (newValue: string) => {
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
    return inputValue !== '' && originalValue !== inputValue;
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

  useEffect(() => {
    if (isLoading !== true && value !== undefined && originalValue !== value) {
      if (originalValue === undefined) {
        setOriginalValue(value);
        setInputValue(value);
        return;
      }
      if (value !== inputValue) {
        if (value === '') {
          setInputValue(inputValue);
          setOriginalValue(inputValue);
        } else {
          setInputValue(value);
          setOriginalValue(value);
        }
      }
    } else if (value !== undefined && originalValue !== undefined && originalValue !== '') {
      if (value === originalValue && value !== inputValue) {
        if (disableDebounce !== undefined && disableDebounce === true) {
          setInputValue(inputValue);
        }
      }
    }
    setIgnoreSave(false);
  }, [disableDebounce, inputValue, isLoading, originalValue, value]);

  const getDiv = useMemo(() => {
    let ret = 'stringeditorbody-inputtext';
    if (darkBackGround === true) {
      ret = 'stringeditorbody-inputtext-dark';
    }
    if (needsSave) {
      ret = 'stringeditorbody-inputtext-save';
    }

    return ret;
  }, [needsSave, darkBackGround]);

  const doShowClear = useMemo((): boolean => {
    return showClear === true && isFocused && inputValue !== originalValue;
  }, [inputValue, isFocused, originalValue, showClear]);

  useEffect(() => {
    if (autoFocus === true && inputRef.current) {
      inputRef.current.focus();
    }
  }, [autoFocus]);

  return (
    <div ref={divReference} className="stringeditor flex flex-column align-items-start">
      {label && (
        <>
          <label className="pl-15">{label.toUpperCase()}</label>
          <div className="pt-small" />
        </>
      )}
      <InputText
        ref={inputRef}
        className={getDiv}
        disabled={disabled}
        id={uuid}
        onChange={(e) => {
          setInputValue(e.target.value as string);
          if (disableDebounce !== true) {
            debounced(e.target.value as string);
          } else {
            onChange && onChange(e.target.value as string);
          }
        }}
        onClick={() => {
          onClick?.();
        }}
        onFocus={() => setIsFocused(true)}
        placeholder={placeholder}
        tooltip={tooltip}
        tooltipOptions={tooltipOptions}
        value={inputValue}
      />
      {doShowClear && (
        <i className="input-icon">
          <i
            className="pi pi-times-circle icon-yellow"
            onClick={() => {
              setInputValue('');
              setOriginalValue('');
              onChange && onChange('');
            }}
          />
        </i>
      )}
    </div>
  );
};

StringEditor.displayName = 'String Editor Body Template';

export default memo(StringEditor);
