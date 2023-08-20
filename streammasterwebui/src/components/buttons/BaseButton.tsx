import { Button } from "primereact/button";
import { getTopToolOptions } from "../../common/common";

type BaseButtonProps = {
  icon: string;
  iconFilled?: boolean;
  label: string;
  onClick: () => void;
  rounded?: boolean;
  severity: 'danger' | 'info' | 'success' | 'warning';
  tooltip?: string; // Add other severities as needed
}

const BaseButton: React.FC<BaseButtonProps> = ({ icon, iconFilled = true, label, onClick, rounded = true, severity, tooltip = '' }) => {
  return (
    <Button
      icon={icon}
      label={label}
      onClick={onClick}
      rounded={rounded}
      severity={severity}
      size="small"
      text={iconFilled !== true}
      tooltip={tooltip}
      tooltipOptions={getTopToolOptions}
    />
  );
};

export default BaseButton;
