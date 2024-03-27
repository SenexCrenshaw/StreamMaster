import { useClickOutside } from 'primereact/hooks';
import { InputNumber } from 'primereact/inputnumber';
import { memo, useEffect, useRef, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';
import CopyButton from '../buttons/CopyButton';

interface NumberInputProperties {
  readonly autoFocus?: boolean;
  readonly isValid?: boolean;
  readonly label?: string;
  readonly min?: number;
  readonly max?: number;
  readonly onChange: (value: number) => void;
  readonly onEnter?: () => void;
  readonly onResetClick?: () => void;
  readonly placeHolder?: string;
  readonly showClear?: boolean;
  readonly showCopy?: boolean;
  readonly showButtons?: boolean;
  readonly value: number;
}

const NumberInput = ({
  autoFocus = true,
  onEnter,
  isValid = true,
  label,
  min,
  max,
  onChange,
  onResetClick,
  placeHolder,
  showClear = true,
  showCopy = false,
  showButtons = false,
  value
}: NumberInputProperties) => {
  const [input, setInput] = useState<number>(1);
  const [originalInput, setOriginalInput] = useState<number | undefined>();
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayReference = useRef(null);
  const uuid = uuidv4();

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

  useEffect(() => {
    setInput(value);
    if (value !== originalInput) {
      setOriginalInput(value);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  const doShowClear = (): boolean => showClear === true && originalInput !== undefined && input !== originalInput;

  const doShowCopy = (): boolean => showCopy === true && input !== undefined && input !== 0;

  return (
    <div className={placeHolder && !label ? 'align-items-center' : 'align-items-center  m-0 p-0 py-2'} ref={overlayReference}>
      <span className={placeHolder && !label ? 'p-input-icon-right' : 'grid grid-nogutter p-input-icon-right p-float-label'}>
        {/* {doShowClear() && originalInput && (
          <i
            className="pi pi-times-circle border-1 border-blue-500"
            hidden={showClear !== true || input === originalInput}
            onClick={() => {
              setInput(originalInput);
              if (onResetClick) {
                onResetClick();
              }
              onChange(originalInput);
            }}
          />
        )} */}
        <InputNumber
          // autoFocus={autoFocus}
          // className={`border-1 w-4rem ${isValid ? '' : 'p-invalid'}`}
          // id={uuid}
          // min={min}
          // max={max}
          // showButtons={showButtons}
          // onChange={(event) => {
          //   if (event.value !== null && event.value !== undefined) {
          //     setInput(event.value);
          //     onChange(event.value);
          //   }
          // }}
          // onFocus={() => setIsFocused(true)}
          // placeholder={placeHolder}
          value={input}
        />
        {label && <label htmlFor={uuid}>{label}</label>}
      </span>
      {/* {doShowCopy() && (
        <div className="col-1 p-0 m-0">
          <CopyButton openCopyWindow={false} value={input.toString()} />
        </div>
      )} */}
    </div>
  );
};

export default memo(NumberInput);
