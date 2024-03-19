import { useEffect, useState } from 'react';
import StringEditorBodyTemplate from './StringEditorBodyTemplate';

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
  }, [id, intValue, setIntValue, value]);

  return (
    <div>
      <StringEditorBodyTemplate
        debounceMs={500}
        onChange={async (e) => {
          console.log(e);
          setIntValue(e);
          onChange(e);
        }}
        placeholder={placeholder}
        showSave={false}
        value={intValue}
      />
    </div>
  );
};

export default StringTracker;
