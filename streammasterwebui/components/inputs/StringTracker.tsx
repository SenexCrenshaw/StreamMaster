import { useEffect, useState } from 'react';
import StringEditor from './StringEditor';

export interface StringTrackerProperties {
  readonly id: string;
  readonly onChange: (value: string | undefined) => void;
  readonly value: string;
  readonly placeholder: string;
}

const StringTracker = ({ id, onChange, placeholder, value }: StringTrackerProperties) => {
  const [intValue, setIntValue] = useState<string | undefined>('');

  useEffect(() => {
    if (value === intValue) {
      return;
    }

    setIntValue(value ?? undefined);
  }, [intValue, value]);

  return (
    <div>
      <StringEditor
        debounceMs={1000}
        onChange={async (e) => {
          setIntValue(e);
          onChange(e);
        }}
        onSave={async (e) => {
          // console.log(e);
          setIntValue(e);
          onChange(e);
        }}
        darkBackGround
        placeholder={placeholder}
        showSave={false}
        value={intValue}
      />
    </div>
  );
};

export default StringTracker;
