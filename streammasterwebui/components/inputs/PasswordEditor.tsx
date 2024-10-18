import { Password } from 'primereact/password';
import React, { useMemo, useState } from 'react';

interface PasswordEditorProps {
  readonly autoFocus?: boolean;
  readonly labelInline?: boolean;
  readonly label?: string;
  readonly darkBackGround?: boolean;
  readonly value?: string;
}

export function PasswordEditor({ ...props }: PasswordEditorProps): React.ReactElement {
  const [inputValue, setInputValue] = useState<string | undefined>(props.value);

  const getDiv = useMemo(() => {
    let ret = 'flex justify-content-center';

    if (props.label && !props.labelInline) {
      ret += ' flex-column';
    }

    if (props.labelInline) {
      ret += ' align-items-start';
    } else {
      ret += ' align-items-center';
    }

    return ret;
  }, [props.label, props.labelInline]);

  const inputGetDiv = useMemo(() => {
    let ret = 'sm-input';
    if (props.darkBackGround) {
      ret += '-dark';
    }

    if (props.labelInline) {
      ret += ' w-12';
    }

    return ret;
  }, [props.darkBackGround, props.labelInline]);

  return (
    <div className={getDiv}>
      {props.label && props.labelInline && <div className={props.labelInline ? 'w-4' : 'w-6'}>{props.label.toUpperCase()}</div>}
      <Password
        className={inputGetDiv}
        feedback
        onChange={(e) => {
          e !== undefined && setInputValue(e.target.value);
        }}
        toggleMask
        value={inputValue}
      />
    </div>
  );
}
