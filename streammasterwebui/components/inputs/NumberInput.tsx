import { useClickOutside } from 'primereact/hooks';
import { InputNumber } from 'primereact/inputnumber';
import { memo, useEffect, useRef, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';
// import CopyButton from '../buttons/CopyButton';

interface NumberInputProperties {
  readonly autoFocus?: boolean;
  readonly isValid?: boolean;
  readonly label?: string;
  readonly max?: number;
  readonly min?: number;
  readonly onChange: (value: number) => void;
  readonly onEnter?: () => void;
  readonly onResetClick?: () => void;
  readonly placeHolder?: string;
  readonly showButtons?: boolean;
  readonly showClear?: boolean;
  readonly showCopy?: boolean;
  readonly suffix?: string | undefined;
  readonly value: number | undefined;
}

const NumberInput = ({
  autoFocus = false,
  onEnter,
  isValid = true,
  label,
  min,
  max,
  onChange,
  placeHolder,
  showButtons = false,
  suffix,
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
    setInput(value ?? 0);
    if (value !== originalInput) {
      setOriginalInput(value);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  return (
    <span className="p-float-label">
      <InputNumber
        id="number-input"
        value={input}
        onChange={(e) => onChange(e.value ?? 0)}
        autoFocus={autoFocus === true}
        min={min}
        max={max}
        showButtons={showButtons}
        suffix={suffix}
      />
      <label htmlFor="number-input">{label}</label>
    </span>
  );
  // return (
  //   <div className={placeHolder && !label ? 'align-items-center' : 'align-items-center  m-0 p-0 py-2'} ref={overlayReference}>
  //     <span className={placeHolder && !label ? 'p-input-icon-right' : 'p-input-icon-right p-float-label'}>
  //       <InputNumber className="text-right" value={input} autoFocus={autoFocus} min={min} max={max} showButtons={showButtons} suffix={suffix} />
  //       {label && <label htmlFor={uuid}>{label}</label>}
  //     </span>
  //   </div>
  // );
};

export default memo(NumberInput);
