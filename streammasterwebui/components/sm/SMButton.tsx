import { getLeftToolOptions, getRightToolOptions } from '@lib/common/common';
import { Button } from 'primereact/button';
import { Tooltip } from 'primereact/tooltip';
import React, { CSSProperties, forwardRef, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';

export type SeverityType = 'danger' | 'help' | 'info' | 'secondary' | 'success' | 'warning';

export interface SMButtonProps {
  onClick?: (e: React.SyntheticEvent) => void;
  readonly children?: React.ReactNode;
  readonly className?: string;
  readonly color?: string;
  readonly darkBackGround?: boolean;
  readonly disabled?: boolean;
  readonly hover?: boolean;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly iconPos?: 'top' | 'bottom' | 'left' | 'right' | undefined;
  readonly isLeft?: boolean;
  readonly isLoading?: boolean;
  readonly label?: string;
  readonly outlined?: boolean | undefined;
  readonly rounded?: boolean;
  readonly severity?: SeverityType;
  readonly style?: CSSProperties | undefined;
  readonly tooltip?: string;
}

const SMButton = forwardRef<Button, SMButtonProps>(
  (
    {
      className: configuredClassName,
      color = 'val(--primary-color-text)',
      darkBackGround = false,
      disabled = false,
      hover = false,
      icon,
      iconPos = 'right',
      iconFilled = false,
      isLeft = false,
      isLoading = false,
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
      const ret = `smbutton-${uuidv4()} w-full `;

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

      // if (hover) {
      //   toRet += ' sm-hover';
      // }
      return toRet + ' ' + cClass + ' ' + tooltipClassName;
    }, [configuredClassName, hover, iconFilled, label, props.children, tooltipClassName]);

    const getStyle = useMemo(() => {
      return {
        ...style,
        color: color
      };
    }, [color, style]);

    const iconClass = useMemo(() => {
      return isLoading ? 'pi-spin pi-spinner' : icon;
    }, [icon, isLoading]);

    if (props.children) {
      if (darkBackGround) {
        return (
          <div className="sm-input-dark w-full">
            <Tooltip target={`.${tooltipClassName}`} />
            <div
              onClick={(e) => {
                e.preventDefault();
                onClick && onClick(e);
              }}
              className={`${tooltipClassName} input-wrapper`}
              data-pr-tooltip={tooltip}
              data-pr-position={isLeft ? 'left' : 'right'}
              data-pr-showdelay={400}
              data-pr-hidedelay={100}
              data-pr-autohide={true}
            >
              {props.children}
              <div className="pl-1" />
              <i className={`mr-1 pi ${iconClass}`} />
            </div>
          </div>
        );
      }
      return (
        <div className="sm-input w-full">
          <Tooltip target={`.${tooltipClassName}`} />
          <div
            onClick={(e) => {
              e.preventDefault();
              onClick && onClick(e);
            }}
            className={`${tooltipClassName} input-wrapper`}
            data-pr-tooltip={tooltip}
            data-pr-position={isLeft ? 'left' : 'right'}
            data-pr-showdelay={400}
            data-pr-hidedelay={100}
            data-pr-autohide={true}
          >
            {props.children}
            <i className={`input-icon pi ${iconClass}`} />
          </div>
        </div>
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
          loading={isLoading}
          onClick={(e) => {
            e.preventDefault();
            onClick && onClick(e);
          }}
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
