import { getLeftToolOptions, getRightToolOptions } from '@lib/common/common';
import { Button } from 'primereact/button';
import { Tooltip } from 'primereact/tooltip';
import React, { CSSProperties, forwardRef, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';

export type SeverityType = 'danger' | 'help' | 'info' | 'secondary' | 'success' | 'warning';

export interface SMButtonProps {
  readonly children?: React.ReactNode;
  readonly className?: string;
  readonly darkBackGround?: boolean;
  readonly disabled?: boolean;
  readonly color?: string;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly iconPos?: 'top' | 'bottom' | 'left' | 'right' | undefined;
  readonly isLeft?: boolean;
  readonly label?: string;
  onClick?: (e: React.SyntheticEvent) => void;
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
      darkBackGround = false,
      disabled = false,
      icon,
      iconPos = 'right',
      iconFilled = false,
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
      const ret = `basebutton-${uuidv4()}`;

      return ret;
    }, []);

    const getClassName = React.useMemo(() => {
      let toRet = 'sm-button';
      let cClass = configuredClassName;

      if (label && label !== '' && !props.children) {
        toRet += ' sm-button-with-label';
      } else {
        if (iconFilled === true) {
          toRet += ' sm-button-icon-filled';
        } else {
          toRet += ' sm-button-icon-only';
        }
      }

      if (iconFilled && !cClass?.endsWith('filled')) {
        cClass += '-filled';
      }

      return toRet + ' ' + cClass + ' ' + tooltipClassName;
    }, [configuredClassName, iconFilled, label, props.children, tooltipClassName]);

    const getStyle = useMemo(() => {
      return {
        ...style,
        color: color
      };
    }, [color, style]);

    if (props.children) {
      if (darkBackGround) {
        return (
          <div className="dark-background w-full">
            <Tooltip target={`.${tooltipClassName}`} />
            <div
              onClick={onClick}
              className={`${tooltipClassName} input-wrapper`}
              data-pr-tooltip={tooltip}
              data-pr-position={isLeft ? 'left' : 'right'}
              data-pr-showdelay={400}
              data-pr-hidedelay={100}
              data-pr-autohide={true}
            >
              {props.children}
              <i className={`input-icon pi ${icon} pr-1`} />
            </div>
          </div>
        );
      }
      return (
        <>
          <Tooltip target={`.${tooltipClassName}`} />
          <div
            onClick={onClick}
            className={`${tooltipClassName} input-wrapper`}
            data-pr-tooltip={tooltip}
            data-pr-position={isLeft ? 'left' : 'right'}
            data-pr-showdelay={400}
            data-pr-hidedelay={100}
            data-pr-autohide={true}
          >
            {props.children}
            <i className={`input-icon pi ${icon} pr-1`} />
          </div>
        </>
      );
    }

    return (
      <>
        <Tooltip target={`.${tooltipClassName}`} />
        <Button
          ref={ref}
          className={getClassName}
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
