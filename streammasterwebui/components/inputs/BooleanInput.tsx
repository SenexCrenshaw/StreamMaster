import { Checkbox } from 'primereact/checkbox';
import { memo } from 'react';
import { v4 as uuidv4 } from 'uuid';

interface BooleanInputProperties {
  readonly isValid?: boolean;
  readonly label?: string;
  readonly onChange: (value: boolean) => void;
  checked: boolean;
}

const BooleanInput = ({ isValid = true, checked, label, onChange }: BooleanInputProperties) => {
  const uuid = uuidv4();

  return (
    <span className="flex col-11 p-input-icon-right">
      <div className="p-0 mr-1 col-2">
        <Checkbox
          className={`text-large w-full ${isValid ? '' : 'p-invalid'}`}
          id={uuid}
          onChange={(event) => {
            if (event.checked !== null && event.checked !== undefined) {
              onChange(event.checked);
            }
          }}
          checked={checked}
        />
      </div>
      {label && (
        <label htmlFor={uuid} className="ml-1">
          {label}
        </label>
      )}
    </span>
  );
};

export default memo(BooleanInput);
