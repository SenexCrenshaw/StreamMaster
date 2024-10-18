import { Checkbox } from 'primereact/checkbox';
import { TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { memo } from 'react';
import { v4 as uuidv4 } from 'uuid';

interface BooleanEditorProperties {
  readonly checked: boolean;
  readonly isValid?: boolean;
  readonly label?: string;
  readonly labelSmall?: boolean;
  readonly labelInline?: boolean;
  readonly onChange: (value: boolean) => void;
  readonly tooltip?: string | undefined;
  readonly tooltipOptions?: TooltipOptions | undefined;
}

const BooleanEditor = ({
  isValid = true,
  checked,
  label,
  labelSmall = false,
  labelInline = false,
  onChange,
  tooltip,
  tooltipOptions
}: BooleanEditorProperties) => {
  const uuid = uuidv4();

  return (
    <div className="flex flex-column flex-wrap justify-content-start align-content-start">
      {label && !labelInline && (
        <>
          <label className={`pl-15 sm-center-stuff text-container ${labelSmall ? ' sm-text-xs font-italic' : ''}`}>{label.toUpperCase()}</label>
          <div className="pt-small" />
        </>
      )}
      <div className={`flex ${labelInline ? 'align-items-center justify-content-between ' : 'flex-column align-items-center'}`}>
        {label && labelInline && <div className={`text-container pr-1 ${labelSmall ? 'sm-text-xs font-italic' : ''}`}>{label.toUpperCase()}</div>}
        <Checkbox
          invalid={!isValid}
          id={uuid}
          onChange={(event) => {
            if (event.checked !== null && event.checked !== undefined) {
              onChange(event.checked);
            }
          }}
          tooltip={tooltip}
          tooltipOptions={tooltipOptions}
          checked={checked}
        />
      </div>
    </div>
  );
};

export default memo(BooleanEditor);
