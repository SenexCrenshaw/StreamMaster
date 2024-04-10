import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { useClickOutside } from 'primereact/hooks';
import { InputText } from 'primereact/inputtext';
import { memo, useCallback, useEffect, useRef, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';

interface TextInputProperties {
  readonly autoFocus?: boolean;
  readonly dontValidate?: boolean;
  readonly isUrl?: boolean;
  readonly isValid?: boolean;
  readonly label?: string;
  readonly openCopyWindow?: boolean;
  readonly placeHolder?: string;
  readonly showClear?: boolean;
  readonly showCopy?: boolean;
  readonly value: string | undefined;

  readonly onChange: (value: string) => void;
  readonly onEnter?: () => void;
  readonly onResetClick?: () => void;
}

const TextInput = ({
  autoFocus = true,
  dontValidate = false,
  isUrl = false,
  onEnter,
  isValid = true,
  label,
  onChange,
  openCopyWindow = false,
  onResetClick,
  placeHolder,
  showClear = true,
  showCopy = false,
  value
}: TextInputProperties) => {
  const [input, setInput] = useState<string>('');
  const [originalInput, setOriginalInput] = useState<string | undefined>(undefined);
  const uuid = uuidv4();
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayReference = useRef(null);
  const { code } = useScrollAndKeyEvents();

  if (code === 'Enter' || code === 'NumpadEnter') {
    onEnter?.();
  }

  useClickOutside(overlayReference, () => {
    if (!isFocused) {
      return;
    }

    setIsFocused(false);
  });

  const processValue = useCallback(
    (value_: string) => {
      if (dontValidate && !isUrl) return value_;
      // If val is null, empty, or undefined, return it as is
      if (!value_) return value_;

      // If it's supposed to be a URL, process accordingly
      if (isUrl) {
        return value_;
      }

      // If not a URL, remove file extension (previous behavior)
      return value_.replace(/\.[^./]+$/, '');
    },
    [dontValidate, isUrl]
  );

  useEffect(() => {
    if (value === undefined || value === '') {
      return;
    }
    if (originalInput === undefined || processValue(value) !== originalInput) {
      setOriginalInput(processValue(value));
      setInput(value);
    }
  }, [originalInput, processValue, value]);

  // const doShowClear = (): boolean => showClear === true && originalInput !== undefined && input !== originalInput;

  // const doShowCopy = (): boolean => showCopy === true && input !== undefined && input !== '';

  return (
    // <div className={placeHolder && !label ? 'flex grid w-full align-items-center' : 'flex grid w-full mt-3 align-items-center'} ref={overlayReference}>
    //   <span className={placeHolder && !label ? 'col-11 p-input-icon-right' : 'col-11 p-input-icon-right p-float-label'}>
    //     {doShowClear() && originalInput && (
    //       <i
    //         className="pi pi-times-circle"
    //         hidden={showClear !== true || input === originalInput}
    //         onClick={() => {
    //           setInput(originalInput);
    //           if (onResetClick) {
    //             onResetClick();
    //           }
    //           onChange(originalInput);
    //         }}
    //       />
    //     )}
    <span className="p-float-label text-xs">
      <InputText
        autoFocus={autoFocus}
        className={`w-full ${isValid ? '' : 'p-invalid'}`}
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
    //   {/* {doShowCopy() && (
    //     <div className="col-1 p-0 m-0">
    //       <CopyButton openCopyWindow={openCopyWindow} value={input} />
    //     </div>
    //   )} */}
    // </div>
  );
};

export default memo(TextInput);
