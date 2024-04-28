import { useClickOutside } from 'primereact/hooks';

import { FloatLabel } from 'primereact/floatlabel';

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
  readonly showSave?: boolean;
  readonly tooltip?: string | undefined;
  readonly tooltipOptions?: TooltipOptions | undefined;
  readonly value: string | undefined;
  readonly darkBackGround?: boolean;
  readonly border?: boolean;
  readonly onFilterClear?: () => void;
}

const StringEditor = ({
  autoFocus,
  disableDebounce,
  debounceMs,
  isLoading,
  label,
  onChange,
  onSave,
  onClick,
  placeholder,
  resetValue,
  showClear = false,
  showSave,
  tooltip,
  tooltipOptions,
  value,
  darkBackGround,
  border,
  onFilterClear
}: StringEditorBodyTemplateProperties) => {
  const uuid = uuidv4();
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayReference = useRef<HTMLDivElement | null>(null);

  const [originalValue, setOriginalValue] = useState<string | undefined>(undefined);
  const [inputValue, setInputValue] = useState<string | undefined>('');

  const debounced = useDebouncedCallback(
    useCallback(
      (value: string) => {
        if (value !== originalValue && isLoading !== true) {
          setInputValue(value);
          // setOriginalValue(value);
          onSave && onSave(value);
          setOriginalValue(undefined);
        }
      },
      [isLoading, onSave, originalValue]
    ),
    debounceMs ? debounceMs : 1500,
    {}
  );

  const save = useCallback(
    (forceValueSave?: string | undefined) => {
      if (isLoading === true || (forceValueSave === undefined && (inputValue === undefined || inputValue === originalValue))) {
        return;
      }
      //setOriginalValue(undefined);
      debounced.cancel();
      if (forceValueSave === undefined) {
        // setOriginalValue(inputValue);
        onSave(inputValue);
      } else {
        // setOriginalValue(forceValueSave);
        onSave(forceValueSave);
      }
    },
    [debounced, inputValue, isLoading, onSave, originalValue]
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

  if (value === 'PIA') {
    console.log('y');
  }
  useEffect(() => {
    if (isLoading !== true && value !== undefined && value !== '' && (originalValue === undefined || value !== inputValue)) {
      if (inputValue !== '') {
        setInputValue(inputValue);
        if (originalValue === undefined) {
          setOriginalValue(inputValue);
        }
      } else {
        setInputValue(value);
        if (originalValue === undefined) {
          setOriginalValue(value);
        }
      }
    }
    // else {
    //   setInputValue('');
    //   setOriginalValue('');
    // }
  }, [isLoading, value, originalValue, inputValue]);

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
    if (border) {
      ret += ' default-border';
    }
    return ret;
  }, [needsSave, border, darkBackGround]);

  const doShowClear = (): boolean => showClear === true && disableDebounce === true && originalValue !== undefined && inputValue !== originalValue;
  // if (showClear)
  //   console.log('StringEditorBodyTemplate', 'showClear', showClear, 'originalValue', originalValue, 'inputValue', inputValue, 'value', value, doShowClear());
  return (
    <div className={label ? 'pt-4' : ''} ref={overlayReference}>
      {/* {isFocused && resetValue !== undefined && resetValue !== inputValue && (
        <Button
          className="absolute right-0"
          disabled={isLoading}
          icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
          onClick={() => {
            setInputValue(resetValue === undefined ? '' : resetValue);
            save(resetValue);
          }}
          rounded
          severity="warning"
          size="small"
          text
          tooltip="Reset Name"
          tooltipOptions={getTopToolOptions}
        />
      )} */}

      {/* {showSave && needsSave && <i className="absolute right-0 pt-1 pi pi-save pr-2 text-500" />} */}

      <FloatLabel>
        <>
          <span className="flex align-items-center">
            {doShowClear() && (
              <i
                className="pi pi-times-circle icon-yellow absolute right-0 pr-1"
                hidden={showClear !== true || value === originalValue}
                onClick={() => {
                  setInputValue(originalValue);
                  // if (onResetClick) {
                  //   onResetClick();
                  // }
                  // onChange(originalValue);
                  onChange && onChange(originalValue);
                }}
              />
            )}
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
          </span>
        </>
        {label && <label htmlFor={uuid}>{label}</label>}
      </FloatLabel>
    </div>
  );
};

StringEditor.displayName = 'String Editor Body Template';

export default memo(StringEditor);
