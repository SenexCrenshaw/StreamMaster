import { getLeftToolOptions, getRightToolOptions } from '@lib/common/common';
import { Logger } from '@lib/common/logger';
import { Button } from 'primereact/button';
import { Tooltip } from 'primereact/tooltip';
import { default as React, forwardRef, default as react, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';
import { SMButtonProperties } from './interfaces/SMButtonProperties';

export type SeverityType = 'danger' | 'help' | 'info' | 'secondary' | 'success' | 'warning';

interface InternalSMButtonProperties extends SMButtonProperties {
  readonly children?: react.ReactNode;
}

const SMButton = forwardRef<Button, InternalSMButtonProperties>(
  (
    {
      color = 'val(--primary-color-text)',
      buttonClassName = '',
      buttonDarkBackground = false,
      buttonDisabled = false,
      iconFilled = false,
      isLeft = false,
      isLoading = false,
      label,
      large = false,
      outlined = false,
      rounded = true,
      tooltip = '',
      ...props
    },
    ref
  ) => {
    const tooltipClassName = React.useMemo(() => {
      const ret = `smbutton-${uuidv4()} width-100`;
      if (large) {
        return ret + ' sm-button-large';
      }
      return ret;
    }, [large]);

    const getClassName = React.useMemo(() => {
      let toRet = 'sm-button';
      let cClass = buttonClassName;

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
      //   // /toRet += ' sm-hover';
      // }
      return toRet + ' ' + cClass + ' ' + tooltipClassName;
    }, [buttonClassName, iconFilled, label, props.children, tooltipClassName]);

    const getStyle = useMemo(() => {
      return {
        // ...style,
        color: color
      };
    }, [color]);

    const iconClass = useMemo(() => {
      return isLoading ? 'pi-spin pi-spinner' : 'pi ' + props.icon;
    }, [props.icon, isLoading]);

    if (props.children) {
      if (buttonDarkBackground) {
        return (
          <div className="stringeditor">
            <div className={large ? 'sm-input-dark-large' : 'sm-input-dark'}>
              <Tooltip target={`.${tooltipClassName}`} />
              <div
                onClick={(e) => {
                  e.preventDefault();
                  props.onClick && props.onClick(e);
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
                <i className={`mr-1 ${iconClass}`} />
              </div>
            </div>
          </div>
        );
      }
      return (
        <div className="sm-input">
          <Tooltip target={`.${tooltipClassName}`} />
          <div
            onClick={(e) => {
              e.preventDefault();
              props.onClick && props.onClick(e);
            }}
            className={`${tooltipClassName} input-wrapper`}
            data-pr-tooltip={tooltip}
            data-pr-position={isLeft ? 'left' : 'right'}
            data-pr-showdelay={400}
            data-pr-hidedelay={100}
            data-pr-autohide={true}
          >
            {props.children}
            <i className={`input-icon ${iconClass}`} />
          </div>
        </div>
      );
    }

    if (props.icon?.includes('pi-window-maximize')) {
      Logger.debug('SMButton', 'icon', iconClass, 'buttonClassName', buttonClassName);
    }
    return (
      <>
        <Tooltip target={`.${tooltipClassName}`} />
        <Button
          ref={ref}
          className={getClassName}
          disabled={buttonDisabled || isLoading}
          icon={iconClass}
          label={label}
          loading={isLoading}
          onClick={(e) => {
            e.preventDefault();
            props.onClick && props.onClick(e);
          }}
          outlined={outlined}
          rounded={rounded}
          text={!iconFilled}
          tooltip={tooltip}
          tooltipOptions={isLeft ? getLeftToolOptions : getRightToolOptions}
          style={getStyle}
        />
      </>
    );
  }
);

export default SMButton;
