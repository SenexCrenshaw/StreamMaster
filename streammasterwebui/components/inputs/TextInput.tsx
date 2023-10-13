import { useClickOutside } from 'primereact/hooks';
import { InputText } from 'primereact/inputtext';
import { memo, useEffect, useRef, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';
import CopyButton from '../buttons/CopyButton';

type TextInputProps = {
  readonly autoFocus?: boolean;
  readonly dontValidate?: boolean;
  readonly isUrl?: boolean;
  readonly isValid?: boolean;
  readonly label?: string;
  readonly onChange: (value: string) => void;
  readonly onEnter?: () => void;
  readonly onResetClick?: () => void;
  readonly placeHolder?: string;
  readonly showClear?: boolean;
  readonly showCopy?: boolean;
  readonly value: string;
};

const TextInput = ({
  autoFocus = true,
  dontValidate = false,
  isUrl = false,
  onEnter,
  isValid = true,
  label,
  onChange,
  onResetClick,
  placeHolder,
  showClear = true,
  showCopy = false,
  value,
}: TextInputProps) => {
  const [input, setInput] = useState<string>('');
  const [originalInput, setOriginalInput] = useState<string | undefined>(undefined);
  const uuid = uuidv4();
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayRef = useRef(null);

  useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if (!isFocused) {
        return;
      }

      if (event.code === 'Enter' || event.code === 'NumpadEnter') {
        if (originalInput !== input) {
          onEnter?.();
        }
      }
    };

    document.addEventListener('keydown', callback);

    return () => {
      document.removeEventListener('keydown', callback);
    };
  }, [input, isFocused, onChange, onEnter, originalInput]);

  useClickOutside(overlayRef, () => {
    if (!isFocused) {
      return;
    }

    setIsFocused(false);
  });

  const processValue = (val: string) => {
    if (dontValidate && !isUrl) return val;
    // If val is null, empty, or undefined, return it as is
    if (!val) return val;

    // If it's supposed to be a URL, process accordingly
    if (isUrl) {
      try {
        // Construct the URL and return it with the query parameters
        const constructedURL = new URL(val);
        return constructedURL.origin + constructedURL.pathname + constructedURL.search;
      } catch (error) {
        // If there's an error constructing the URL (meaning it's not a valid URL), return val as is
        return val;
      }
    }

    // If not a URL, remove file extension (previous behavior)
    return val.replace(/\.[^/.]+$/, '');
  };

  useEffect(() => {
    if (originalInput === undefined && value !== originalInput) {
      setOriginalInput(processValue(value));
    }

    setInput(processValue(value));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  const doShowClear = (): boolean => {
    return showClear === true && originalInput !== undefined && input !== originalInput;
  };

  const doShowCopy = (): boolean => {
    return showCopy === true && input !== undefined && input !== '';
  };

  return (
    <div className={placeHolder && !label ? 'flex grid w-full align-items-center' : 'flex grid w-full mt-3 align-items-center'} ref={overlayRef}>
      <span className={placeHolder && !label ? 'col-11 p-input-icon-right' : 'col-11 p-input-icon-right p-float-label'}>
        {doShowClear() && originalInput && (
          <i
            className="pi pi-times-circle"
            hidden={showClear !== true || input === originalInput}
            onClick={() => {
              setInput(originalInput);
              if (onResetClick) {
                onResetClick();
              }
              onChange(originalInput);
            }}
          />
        )}

        <InputText
          autoFocus={autoFocus}
          className={`text-large w-full ` + (isValid ? '' : 'p-invalid')}
          id={uuid}
          onChange={(event) => {
            setInput(processValue(event.target.value));
            onChange(processValue(event.target.value));
          }}
          onFocus={() => setIsFocused(true)}
          placeholder={placeHolder}
          value={input}
        />
        {label && <label htmlFor={uuid}>{label}</label>}
      </span>
      {doShowCopy() && (
        <div className="col-1 p-0 m-0">
          <CopyButton value={input} />
        </div>
      )}
    </div>
  );
};

export default memo(TextInput);
