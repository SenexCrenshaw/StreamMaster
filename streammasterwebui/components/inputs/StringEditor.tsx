import { Logger } from '@lib/common/logger';
import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { useClickOutside } from 'primereact/hooks';
import { InputText } from 'primereact/inputtext';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { forwardRef, memo, useCallback, useEffect, useImperativeHandle, useMemo, useRef, useState } from 'react';
import { useDebouncedCallback } from 'use-debounce';
import { v4 as uuidv4 } from 'uuid';

export interface StringEditorBodyTemplateProperties {
  readonly autoFocus?: boolean;
  readonly darkBackGround?: boolean;
  readonly debounceMs?: number;
  readonly disabled?: boolean;
  readonly disableDebounce?: boolean;
  readonly isLoading?: boolean;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly labelInlineSmall?: boolean;
  readonly onChange?: (value: string | undefined) => void;
  readonly onClick?: () => void;
  readonly onSave?: (value: string | undefined) => void;
  readonly placeholder?: string;
  readonly resetValue?: string | undefined;
  readonly showClear?: boolean;
  readonly tooltip?: string | undefined;
  readonly tooltipOptions?: TooltipOptions | undefined;
  readonly value: string | undefined;
}

export interface StringEditorRef {
  clear: () => void;
}

const StringEditor = forwardRef<StringEditorRef, StringEditorBodyTemplateProperties>(
  (
    {
      autoFocus,
      darkBackGround,
      debounceMs = 1500,
      disabled = false,
      disableDebounce = false,
      isLoading = false,
      label,
      labelInline = false,
      labelInlineSmall = false,
      onChange,
      onClick,
      onSave,
      placeholder,
      showClear = false,
      tooltip,
      tooltipOptions,
      value
    },
    ref
  ) => {
    const uuid = uuidv4();
    const [isFocused, setIsFocused] = useState<boolean>(false);
    const divReference = useRef<HTMLDivElement | null>(null);
    const [ignoreSave, setIgnoreSave] = useState<boolean>(false);
    const [originalValue, setOriginalValue] = useState<string | undefined | null>(undefined);
    const [inputValue, setInputValue] = useState<string | undefined | null>(value);
    const { code } = useScrollAndKeyEvents();
    const inputRef = useRef<HTMLInputElement>(null);

    useImperativeHandle(
      ref,
      () => ({
        clear: () => {
          setInputValue('');
          setOriginalValue('');
        }
      }),
      []
    );

    const save = useCallback(
      (forceValueSave?: string | undefined) => {
        setIgnoreSave(true);
        Logger.debug('Saving value', { forceValueSave, inputValue });
        onSave && onSave(forceValueSave ?? inputValue ?? '');
      },
      [inputValue, onSave]
    );

    const debounced = useDebouncedCallback(
      useCallback(
        (newValue: string) => {
          if (!isLoading) {
            save(newValue);
          }
        },
        [isLoading, save]
      ),
      debounceMs
    );

    const needsSave = useMemo(() => inputValue !== '' && originalValue !== inputValue, [inputValue, originalValue]);

    useEffect(() => {
      if (code === 'Enter' || code === 'NumpadEnter') {
        if (needsSave && !ignoreSave) {
          debounced.cancel();
          save();
        }
      }
    }, [code, debounced, ignoreSave, needsSave, save]);

    useClickOutside(divReference, () => {
      if (!isFocused) return;
      setIsFocused(false);
      if (!disableDebounce && originalValue !== inputValue && !ignoreSave) {
        save();
      }
    });

    useEffect(() => {
      if (value?.includes('PIA')) {
        Logger.debug('StringEditor', { inputValue, originalValue, value });
      }

      if (!isLoading) {
        if (originalValue === undefined && value !== undefined) {
          setOriginalValue(value);
          setInputValue(value);
          // } else if (value !== undefined) {
          //   setInputValue(value);
        } else if (disableDebounce && value !== undefined && value === originalValue && value !== inputValue) {
          setInputValue(value);
        }
        if (onSave && value !== undefined) {
          setOriginalValue(value);
          setInputValue(value);
        }
      }

      setIgnoreSave(false);
    }, [disableDebounce, inputValue, isLoading, onSave, originalValue, value]);

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

    const doShowClear = useMemo(
      () => darkBackGround && showClear && inputValue !== '' && originalValue !== inputValue,
      [darkBackGround, inputValue, originalValue, showClear]
    );

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

    useEffect(() => {
      if (autoFocus && inputRef.current) {
        inputRef.current.focus();
      }
    }, [autoFocus]);

    return (
      <>
        {label && !labelInline && (
          <>
            <label className="pl-15">{label.toUpperCase()}</label>
          </>
        )}
        <div ref={divReference} className={getDiv}>
          {label && labelInline && <div className={labelInline ? 'w-4' : 'w-6'}>{label.toUpperCase()}</div>}
          <InputText
            ref={inputRef}
            className={inputGetDiv}
            disabled={disabled || isLoading}
            id={uuid}
            onChange={(e) => {
              const newValue = e.target.value as string;
              setInputValue(newValue);
              if (!disableDebounce) {
                debounced(newValue);
              } else {
                onChange?.(newValue);
              }
            }}
            onClick={onClick}
            onFocus={() => setIsFocused(true)}
            placeholder={placeholder}
            tooltip={tooltip}
            tooltipOptions={tooltipOptions}
            value={inputValue ?? ''}
          />
          {doShowClear && (
            <i className="input-icon">
              <i
                className="pi pi-times-circle icon-yellow"
                onClick={() => {
                  setInputValue(originalValue);
                  if (originalValue !== null) onChange?.(originalValue);
                }}
              />
            </i>
          )}
        </div>
      </>
    );
  }
);

StringEditor.displayName = 'String Editor Body Template';

export default memo(StringEditor);
