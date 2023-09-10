/* eslint-disable @typescript-eslint/no-unused-vars */
import { InputText } from "primereact/inputtext";
import { memo, useEffect, useState } from "react";
import CopyButton from "../buttons/CopyButton";

type TextInputProps = {
  readonly autoFocus?: boolean;
  readonly isValid?: boolean;
  readonly label?: string;
  readonly onChange: (value: string) => void;
  readonly onResetClick?: () => void;
  readonly placeHolder?: string;
  readonly showClear?: boolean;
  readonly showCopy?: boolean;
  readonly value: string;
}
// className={`${isValidUrl(source) ? '' : 'p-invalid'}`}
const TextInput = ({ autoFocus = true, isValid = true, label, onChange, onResetClick, placeHolder, showClear = true, showCopy = false, value }: TextInputProps) => {
  const [input, setInput] = useState<string>('');
  const [originalInput, setOriginalInput] = useState<string | undefined>(undefined);


  useEffect(() => {
    if (originalInput === undefined && value != originalInput) {
      setOriginalInput(value.replace(/\.[^/.]+$/, ''))
    }

    setInput(value.replace(/\.[^/.]+$/, ''));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  const doShowClear = (): boolean => {
    return showClear === true && originalInput !== undefined && input !== originalInput;
  }

  const doShowCopy = (): boolean => {
    return showCopy === true && input !== undefined && input !== '';
  }

  return (
    <div className={placeHolder && !label ? 'flex grid w-full align-items-center' : 'flex grid w-full mt-3 align-items-center'}>
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
      {doShowCopy() &&
        <div className='col-1'>
          <CopyButton value={input} />
        </div>
      }
    </div>
  )
}

export default memo(TextInput);
