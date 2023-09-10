/* eslint-disable @typescript-eslint/no-unused-vars */
import { InputText } from "primereact/inputtext";
import { memo, useEffect, useState } from "react";

type TextInputProps = {
  readonly autoFocus?: boolean;
  readonly isValid?: boolean;
  readonly label?: string;
  readonly onChange: (value: string) => void;
  readonly onResetClick?: () => void;
  readonly placeHolder?: string;
  readonly showClear?: boolean;
  readonly value: string;
}
// className={`${isValidUrl(source) ? '' : 'p-invalid'}`}
const TextInput = ({ autoFocus = true, isValid = true, label, onChange, onResetClick, placeHolder, showClear = true, value }: TextInputProps) => {
  const [input, setInput] = useState<string>('');
  const [originalInput, setOriginalInput] = useState<string | undefined>(undefined);

  useEffect(() => {
    if (originalInput === undefined && value != originalInput) {
      setOriginalInput(value.replace(/\.[^/.]+$/, ''))
    }

    setInput(value.replace(/\.[^/.]+$/, ''));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  return (
    <div className={placeHolder && !label ? 'w-full' : 'w-full mt-3'}>
      <span className={placeHolder && !label ? 'w-full p-input-icon-right' : 'w-full p-input-icon-right p-float-label'}>
        {showClear === true && originalInput !== undefined && input !== originalInput &&
          <i
            className="pi pi-times-circle z-1"
            hidden={showClear !== true || input === originalInput}
            onClick={() => {
              setInput(originalInput);
              if (onResetClick) { onResetClick(); }
              onChange(originalInput);
            }}
          />
        }

        <InputText
          autoFocus={autoFocus}
          className={`text-large w-full ` + (isValid ? '' : 'p-invalid')}
          id="name"
          onChange={(event) => {
            setInput(event.target.value.replace(/\.[^/.]+$/, ''))
            onChange(event.target.value.replace(/\.[^/.]+$/, ''));
          }
          }
          placeholder={!label ? placeHolder : undefined}
          value={input}
        />
        {label &&
          <label htmlFor="name">{label}</label>
        }
      </span>
    </div>
  )
}

export default memo(TextInput);
