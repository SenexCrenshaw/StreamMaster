import { useLocalStorage } from 'primereact/hooks';
import StringEditor from './StringEditor';

export interface StringTrackerProperties {
  readonly id: string;
  readonly isLoading?: boolean;
  readonly onChange: (value: string | undefined) => void;
  readonly placeholder: string;
  readonly value: string;
}

const StringTracker = ({ id, isLoading, onChange, placeholder, value }: StringTrackerProperties) => {
  const [intValue, setIntValue] = useLocalStorage<string | undefined>('', id);

  return (
    <StringEditor
      debounceMs={1000}
      isLoading={isLoading}
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
