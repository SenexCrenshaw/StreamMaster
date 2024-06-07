import { ToggleButton } from 'primereact/togglebutton';
import { memo } from 'react';

interface BooleanEditorProperties {
  readonly checked: boolean;
  readonly isValid?: boolean;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly onChange: (value: boolean) => void;
}

const BooleanEditor = ({ isValid = true, checked, label, labelInline = false, onChange }: BooleanEditorProperties) => {
  // const uuid = uuidv4();
  return (
    <>
      {label && !labelInline && (
        <>
          <label className="pl-15">{label.toUpperCase()}</label>
          <div className="pt-small" />
        </>
      )}
      <div className={`flex ${labelInline ? 'align-items-center' : 'flex-column align-items-start'}`}>
        {label && labelInline && <div className="w-11">{label.toUpperCase()}</div>}
        <ToggleButton
          className="w-full"
          checked={checked}
          onChange={(event) => {
            if (event.value !== null && event.value !== undefined) {
              onChange(event.value);
            }
          }}
        />
        {/* <Checkbox
          invalid={!isValid}
          id={uuid}
          onChange={(event) => {
            if (event.checked !== null && event.checked !== undefined) {
              onChange(event.checked);
            }
          }}
          checked={checked}
        /> */}
      </div>
    </>
  );
};

export default memo(BooleanEditor);
