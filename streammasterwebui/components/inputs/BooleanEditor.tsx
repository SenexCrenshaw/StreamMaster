import { Checkbox } from 'primereact/checkbox';
import { memo } from 'react';
import { v4 as uuidv4 } from 'uuid';

interface BooleanEditorProperties {
  readonly checked: boolean;
  readonly isValid?: boolean;
  readonly label?: string;
  readonly labelSmall?: boolean;
  readonly labelInline?: boolean;
  readonly onChange: (value: boolean) => void;
}

const BooleanEditor = ({ isValid = true, checked, label, labelSmall = false, labelInline = false, onChange }: BooleanEditorProperties) => {
  const uuid = uuidv4();

  return (
    <>
      {label && !labelInline && (
        <>
          <label className={`pl-15 text-container ${labelSmall ? ' sm-text-xs font-italic' : ''}`}>{label.toUpperCase()}</label>
          <div className="pt-small" />
        </>
      )}
      <div className={`flex ${labelInline ? 'align-items-center ' : 'flex-column align-items-start'}`}>
        {label && labelInline && <div className={`text-container pr-1 ${labelSmall ? 'sm-text-xs font-italic' : ''}`}>{label.toUpperCase()}</div>}
        <Checkbox
          invalid={!isValid}
          id={uuid}
          onChange={(event) => {
            if (event.checked !== null && event.checked !== undefined) {
              onChange(event.checked);
            }
          }}
          checked={checked}
        />
      </div>
    </>
  );
};

export default memo(BooleanEditor);
