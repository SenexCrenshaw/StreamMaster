import { getLeftToolOptions, getTopToolOptions } from '@lib/common/common';
import { Button } from 'primereact/button';
import React, { CSSProperties, forwardRef } from 'react';

export interface BaseButtonProps {
  className?: string;
  disabled?: boolean;
  icon: string;
  iconFilled?: boolean;
  isLeft?: boolean;
  label?: string;
  onClick: (e: React.SyntheticEvent) => void;
  rounded?: boolean;
  severity?: 'danger' | 'help' | 'info' | 'secondary' | 'success' | 'warning';
  tooltip?: string;
  style?: CSSProperties | undefined;
}

const BaseButton = forwardRef<Button, BaseButtonProps>(
  ({ className, disabled = false, icon, iconFilled = true, isLeft = false, label, style, onClick, rounded = true, severity, tooltip = '', ...props }, ref) => (
    <Button
      className={className}
      disabled={disabled}
      icon={`pi ${icon}`}
      label={label}
      onClick={onClick}
      rounded={rounded}
      severity={severity}
      size="small"
      text={!iconFilled}
      tooltip={tooltip}
      tooltipOptions={isLeft ? getLeftToolOptions : getTopToolOptions}
      ref={ref}
      style={style && style}
      {...props}
    />
  )
);

export default BaseButton;
