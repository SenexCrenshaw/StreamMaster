import StringEditor from './StringEditor';
import { useLocalStorage } from 'primereact/hooks';

export interface StringTrackerProperties {
  readonly id: string;
  readonly onChange: (value: string | undefined) => void;
  readonly value: string;
  readonly placeholder: string;
}

const StringTracker = ({ id, onChange, placeholder, value }: StringTrackerProperties) => {
  const [intValue, setIntValue] = useLocalStorage<string | undefined>('', id);

  return (
    <StringEditor
      debounceMs={1000}
      onChange={async (e) => {
        setIntValue(e);
        onChange(e);
      }}
      onSave={async (e) => {
        setIntValue(e);
        onChange(e);
      }}
      showClear
      darkBackGround
      placeholder={placeholder}
      value={intValue}
    />
  );
};

export default StringTracker;
