import { Button } from "primereact/button";
import { getLeftToolOptions, getTopToolOptions } from "../../common/common";

export type ChildButtonProps = {
  disabled?: boolean | undefined;
  iconFilled?: boolean;
  label?: string | undefined;
  onClick: () => void;
  tooltip?: string;
}

export type BaseButtonProps = {
  disabled?: boolean | undefined;
  icon: string;
  iconFilled?: boolean;
  isLeft?: boolean | undefined;
  label?: string | undefined;
  onClick: () => void;
  rounded?: boolean;
  severity?: 'danger' | 'help' | 'info' | 'secondary' | 'success' | 'warning' | undefined;
  tooltip?: string; // Add other severities as needed
}

const BaseButton: React.FC<BaseButtonProps> = ({ disabled, icon, iconFilled = true, isLeft = false, label, onClick, rounded = true, severity, tooltip = '' }) => {

  return (
    <Button
      disabled={disabled}
      icon={"pi " + icon}
      label={label}
      onClick={onClick}
      rounded={rounded}
      severity={severity}
      size="small"
      text={iconFilled !== true}
      tooltip={tooltip}
      tooltipOptions={isLeft ? getLeftToolOptions : getTopToolOptions}
    />
  );
};

export default BaseButton;
