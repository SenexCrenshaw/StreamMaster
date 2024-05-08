import { useClickOutside } from 'primereact/hooks';

import { FloatLabel } from 'primereact/floatlabel';

import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { InputText } from 'primereact/inputtext';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useDebouncedCallback } from 'use-debounce';
import { v4 as uuidv4 } from 'uuid';

export interface StringEditorBodyTemplateProperties {
  readonly autoFocus?: boolean;
  readonly disableDebounce?: boolean;
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
  debounceMs,
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

  const [originalValue, setOriginalValue] = useState<string | undefined>(undefined);
  const [inputValue, setInputValue] = useState<string | undefined>('');
  const { code } = useScrollAndKeyEvents();

  const [ignoreSave, setIgnoreSave] = useState<boolean>(false);

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
    debounceMs ? debounceMs : 1500,
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

  // const doReset = (): boolean => showClear === true && disableDebounce === true && originalValue !== undefined && inputValue !== originalValue;

  const doShowClear = useMemo((): boolean => {
    return showClear === true && isFocused && inputValue !== originalValue;
  }, [inputValue, isFocused, originalValue, showClear]);

  const getButton = useMemo(() => {
    return (
      <div ref={divReference} className="stringeditor">
        <InputText
          className={getDiv}
          id={uuid}
          autoFocus={autoFocus}
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
  }, [autoFocus, debounced, disableDebounce, doShowClear, getDiv, inputValue, onChange, onClick, placeholder, tooltip, tooltipOptions, uuid]);

  if (!label) {
    return getButton;
  }

  return (
    <div className={label ? 'stringeditor pt-4' : 'stringeditor'} ref={divReference}>
      <FloatLabel>
        {getButton}
        <label className="" htmlFor={uuid}>
          {label}
        </label>
      </FloatLabel>
    </div>
  );
};

StringEditor.displayName = 'String Editor Body Template';

export default memo(StringEditor);
