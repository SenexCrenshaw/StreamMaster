import { memo, useCallback, useEffect, useState } from 'react';

import StringEditor from './StringEditor';

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

  const processValue = useCallback(
    (value_: string | undefined): string => {
      if (value_ === undefined || value_ === '') {
        return '';
      }
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
    <div className="pt-4">
      <StringEditor
        disableDebounce={true}
        darkBackGround
        autoFocus={autoFocus}
        label={label}
        // className={`w-full ${isValid ? '' : 'p-invalid'}`}
        // id={uuid}
        onSave={(value) => {
          setInput(processValue(value));
          onChange(processValue(value));
        }}
        showClear
        placeholder={placeHolder}
        value={input}
      />
    </div>
  );
};

export default memo(TextInput);
