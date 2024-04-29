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
  disableDebounce = false,
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
  const { code } = useScrollAndKeyEvents();

  const [ignoreSave, setIgnoreSave] = useState<boolean>(false);

  const save = useCallback(
    (forceValueSave?: string | undefined) => {
      setIgnoreSave(true);
      // if (isLoading === true || (forceValueSave === undefined && (inputValue === undefined || inputValue === originalValue))) {
      //   return;
      // }
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

  useClickOutside(overlayReference, () => {
    if (!isFocused) {
      return;
    }
    setIsFocused(false);
    if (disableDebounce !== undefined && disableDebounce !== true && originalValue !== inputValue && !ignoreSave) {
      save();
    }
  });

  useEffect(() => {
    if (isLoading !== true && value !== undefined && value !== '' && originalValue !== value) {
      if (value !== inputValue) {
        setInputValue(value);
        setOriginalValue(value);
      }
    } else if (value !== undefined && value !== '' && originalValue !== undefined && originalValue !== '') {
      if (value === originalValue && value !== inputValue) {
        if (disableDebounce !== undefined && disableDebounce === true) {
          setInputValue(value);
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
    if (border) {
      ret += ' default-border';
    }
    return ret;
  }, [needsSave, border, darkBackGround]);

  const doShowClear = (): boolean => showClear === true && disableDebounce === true && originalValue !== undefined && inputValue !== originalValue;
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

        <label htmlFor={uuid}>{label}</label>
      </FloatLabel>
    </div>
  );
};

StringEditor.displayName = 'String Editor Body Template';

export default memo(StringEditor);
