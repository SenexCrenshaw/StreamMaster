import { useClickOutside } from 'primereact/hooks';
import { InputText } from 'primereact/inputtext';
import { memo, useEffect, useRef, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';
import CopyButton from '../buttons/CopyButton';

interface TextInputProperties {
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
  readonly value: string | undefined;
}

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
  value
}: TextInputProperties) => {
  const [input, setInput] = useState<string>('');
  const [originalInput, setOriginalInput] = useState<string | undefined>();
  const uuid = uuidv4();
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayReference = useRef(null);

  useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if (!isFocused) {
        return;
      }

      if ((event.code === 'Enter' || event.code === 'NumpadEnter') && originalInput !== input) {
        onEnter?.();
      }
    };

    document.addEventListener('keydown', callback);

    return () => {
      document.removeEventListener('keydown', callback);
    };
  }, [input, isFocused, onChange, onEnter, originalInput]);

  useClickOutside(overlayReference, () => {
    if (!isFocused) {
      return;
    }

    setIsFocused(false);
  });

  const processValue = (value_: string) => {
    if (dontValidate && !isUrl) return value_;
    // If val is null, empty, or undefined, return it as is
    if (!value_) return value_;

    // If it's supposed to be a URL, process accordingly
    if (isUrl) {
      try {
        // Construct the URL and return it with the query parameters
        const constructedURL = new URL(value_);
        return constructedURL.origin + constructedURL.pathname + constructedURL.search;
      } catch {
        // If there's an error constructing the URL (meaning it's not a valid URL), return val as is
        return value_;
      }
    }

    // If not a URL, remove file extension (previous behavior)
    return value_.replace(/\.[^./]+$/, '');
  };

  useEffect(() => {
    if (originalInput === undefined && value !== originalInput) {
      setOriginalInput(processValue(value));
    }

    if (value !== undefined) {
      setInput(processValue(value));
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  const doShowClear = (): boolean => showClear === true && originalInput !== undefined && input !== originalInput;

  const doShowCopy = (): boolean => showCopy === true && input !== undefined && input !== '';

  return (
    <div className={placeHolder && !label ? 'flex grid w-full align-items-center' : 'flex grid w-full mt-3 align-items-center'} ref={overlayReference}>
      <span className={placeHolder && !label ? 'col-11 p-input-icon-right w-full' : 'col-11 p-input-icon-right p-float-label w-full'}>
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
          className={`text-large w-full ${isValid ? '' : 'p-invalid'}`}
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
        {/* {!label && placeHolder && <span className="absolute top-25 left-50 text-xs">{placeHolder}</span>} */}
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
