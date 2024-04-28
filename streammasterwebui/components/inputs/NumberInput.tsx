import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
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
  const { code } = useScrollAndKeyEvents();

  const [input, setInput] = useState<number>(1);
  const [originalInput, setOriginalInput] = useState<number | undefined>();
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const overlayReference = useRef(null);
  const uuid = uuidv4();

  if (code === 'Enter' || code === 'NumpadEnter') {
    onEnter?.();
  }

  useClickOutside(overlayReference, () => {
    if (!isFocused) {
      return;
    }

    setIsFocused(false);
  });

  useEffect(() => {
    if (value !== originalInput) {
      setOriginalInput(value ?? 0);
    }
    setInput(value ?? 0);
  }, [originalInput, value]);

  return (
    <div className="pt-4">
      <span className="stringeditorbody-inputtext-dark p-float-label">
        <InputNumber
          id={uuid}
          value={input}
          onChange={(e) => onChange(e.value ?? 0)}
          autoFocus={autoFocus === true}
          min={min}
          max={max}
          showButtons={showButtons}
          suffix={suffix}
        />
        <label htmlFor={uuid}>{label}</label>
      </span>
    </div>
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
