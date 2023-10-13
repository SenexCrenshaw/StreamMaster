import { getLeftToolOptions, getTopToolOptions } from '@/lib/common/common';
import { Button } from 'primereact/button';
import { type SyntheticEvent } from 'react';

export type ChildButtonProps = {
  className?: string;
  disabled?: boolean | undefined;
  iconFilled?: boolean;
  label?: string | undefined;
  onClick: (e: SyntheticEvent) => void;
  tooltip?: string;
};

export type BaseButtonProps = {
  className?: string;
  disabled?: boolean | undefined;
  icon: string;
  iconFilled?: boolean;
  isLeft?: boolean | undefined;
  label?: string | undefined;
  onClick: (e: SyntheticEvent) => void;
  rounded?: boolean;
  severity?: 'danger' | 'help' | 'info' | 'secondary' | 'success' | 'warning' | undefined;
  tooltip?: string; // Add other severities as needed
};

const BaseButton: React.FC<BaseButtonProps> = ({
  className,
  disabled,
  icon,
  iconFilled = true,
  isLeft = false,
  label,
  onClick,
  rounded = true,
  severity,
  tooltip = '',
}) => {
  return (
    <Button
      className={className}
      disabled={disabled}
      icon={'pi ' + icon}
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
