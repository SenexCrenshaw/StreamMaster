/* eslint-disable @typescript-eslint/no-unused-vars */
import { InputText } from "primereact/inputtext";
import { memo, useEffect, useState } from "react";

type TextInputProps = {
  readonly isValid?: boolean;
  readonly label: string;
  readonly onClick: (value: string) => void;
  readonly onResetClick?: () => void;
  readonly placeHolder?: string;
  readonly showClear?: boolean;
  readonly value: string;
}
// className={`${isValidUrl(source) ? '' : 'p-invalid'}`}
const TextInput = ({ isValid = true, label, onClick, onResetClick, placeHolder, showClear = true, value }: TextInputProps) => {
  const [input, setInput] = useState<string>('');
  const [originalInput, setOriginalInput] = useState<string>('');

  useEffect(() => {
    if (originalInput === '' && value != originalInput) {
      setOriginalInput(value.replace(/\.[^/.]+$/, ''))
    }

    setInput(value.replace(/\.[^/.]+$/, ''));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  return (
    <span className="p-input-icon-right p-float-label w-full mt-3">

      {showClear === true && input !== originalInput &&
        <i
          className="pi pi-times-circle z-1"
          hidden={showClear !== true || input === originalInput}
          onClick={() => {
            setInput(originalInput);
            if (onResetClick) onResetClick();
          }
          }
        />
      }

      <InputText
        autoFocus
        className={`text-large w-full ` + (isValid ? '' : 'p-invalid')}
        id="name"
        onChange={(event) => {
          setInput(event.target.value.replace(/\.[^/.]+$/, ''))
          onClick(event.target.value.replace(/\.[^/.]+$/, ''));
        }
        }
        placeholder={placeHolder}
        value={input}
      />
      <label htmlFor="name">{label}</label>
    </span>
  )
}

export default memo(TextInput);
