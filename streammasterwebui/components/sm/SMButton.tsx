import { getLeftToolOptions, getRightToolOptions } from '@lib/common/common';
import { Button } from 'primereact/button';
import { Tooltip } from 'primereact/tooltip';
import React, { CSSProperties, forwardRef, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';

export type SeverityType = 'danger' | 'help' | 'info' | 'secondary' | 'success' | 'warning';

export interface SMButtonProps {
  readonly children?: React.ReactNode;
  readonly className?: string;
  readonly disabled?: boolean;
  readonly color?: string;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly iconPos?: 'top' | 'bottom' | 'left' | 'right' | undefined;
  readonly isLeft?: boolean;
  readonly label?: string;
  onClick: (e: React.SyntheticEvent) => void;
  readonly rounded?: boolean;
  readonly severity?: SeverityType;
  tooltip?: string;
  readonly style?: CSSProperties | undefined;
  readonly outlined?: boolean | undefined;
}

const SMButton = forwardRef<Button, SMButtonProps>(
  (
    {
      className: configuredClassName,
      color = 'val(--primary-color-text)',
      disabled = false,
      icon,
      iconPos = 'right',
      iconFilled = true,
      isLeft = false,
      label,
      style,
      outlined = false,
      onClick,
      rounded = true,
      severity,
      tooltip = '',
      ...props
    },
    ref
  ) => {
    const tooltipClassName = React.useMemo(() => {
      // if (iconFilled) {
      if (label && label !== '' && !props.children) {
        return `sm-button-with-label basebutton-${uuidv4()} ${configuredClassName ?? ''}`;
      }
      return `sm-button basebutton-${uuidv4()} ${configuredClassName ?? ''}`;
      // }
      // return `sm-button basebutton-${uuidv4()} ${configuredClassName ?? ''}`;
    }, [configuredClassName, label, props.children]);

    const getStyle = useMemo(() => {
      return {
        ...style,
        color: color
      };
    }, [color, style]);

    if (props.children) {
      return (
        <>
          <Tooltip target={tooltipClassName} />
          <div
            className={`${tooltipClassName} flex flex-wrap justify-items-center justify-content-between align-content-center align-items-center sm-hover`}
            onClick={onClick}
          >
            {props.children}
            <div className={`pi ${icon} pr-1`} />
          </div>
        </>
      );
    }

    return (
      <>
        <Tooltip target={tooltipClassName} />
        <Button
          ref={ref}
          className={tooltipClassName}
          disabled={disabled}
          icon={`pi ${icon}`}
          iconPos={iconPos}
          label={label}
          onClick={onClick}
          outlined={outlined}
          rounded={rounded}
          severity={severity}
          text={!iconFilled}
          tooltip={tooltip}
          tooltipOptions={isLeft ? getLeftToolOptions : getRightToolOptions}
          style={getStyle}
          {...props}
        />
      </>
    );
  }
);

export default SMButton;
