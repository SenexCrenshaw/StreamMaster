import { useClickOutside } from "primereact/hooks";
import { InputNumber } from "primereact/inputnumber";
import { memo, useEffect, useRef, useState } from "react";
import CopyButton from "../buttons/CopyButton";

type NumberInputProps = {
  readonly autoFocus?: boolean;
  readonly isValid?: boolean;
  readonly label?: string;
  readonly onChange: (value: number) => void;
  readonly onEnter?: () => void;
  readonly onResetClick?: () => void;
  readonly placeHolder?: string;
  readonly showClear?: boolean;
  readonly showCopy?: boolean;
  readonly value: number;
}

const NumberInput = ({ autoFocus = true, onEnter, isValid = true, label, onChange, onResetClick, placeHolder, showClear = true, showCopy = false, value }: NumberInputProps) => {
  const [input, setInput] = useState<number>(1);
  const [originalInput, setOriginalInput] = useState<number | undefined>(undefined);
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

  useEffect(() => {
    setInput(value);
    if (value !== originalInput) {
      setOriginalInput(value);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);


  const doShowClear = (): boolean => {
    return showClear === true && originalInput !== undefined && input !== originalInput;
  }

  const doShowCopy = (): boolean => {
    return showCopy === true && input !== undefined && input !== 0;
  }

  return (
    <div className={placeHolder && !label ? 'flex grid w-full align-items-center' : 'flex grid w-full mt-3 align-items-center'} ref={overlayRef}>
      <span className={placeHolder && !label ? 'col-11 p-input-icon-right' : 'col-11 p-input-icon-right p-float-label'}>
        {doShowClear() && originalInput &&
          <i
            className="pi pi-times-circle"
            hidden={showClear !== true || input === originalInput}
            onClick={() => {
              setInput(originalInput);
              if (onResetClick) { onResetClick(); }
              onChange(originalInput);
            }}
          />
        }

        <InputNumber
          autoFocus={autoFocus}
          className={`text-large w-full ` + (isValid ? '' : 'p-invalid')}
          id="name"
          onChange={(event) => {
            if (event.value !== null && event.value !== undefined) {
              setInput(event.value)
              onChange(event.value);
            }
          }
          }
          onFocus={() => setIsFocused(true)}
          placeholder={placeHolder}
          value={input}
        />
        {label &&
          <label htmlFor="name">{label}</label>
        }
      </span>
      {doShowCopy() &&
        <div className='col-1 p-0 m-0'>
          <CopyButton value={input.toString()} />
        </div>
      }
    </div>
  )
}

export default memo(NumberInput);
