import { getLeftToolOptions, getRightToolOptions } from '@lib/common/common';
import { Button } from 'primereact/button';
import { Tooltip } from 'primereact/tooltip';
import { CSSProperties, default as React, forwardRef, default as react, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';
import { SMButtonProperties } from './interfaces/SMButtonProperties';

export type SeverityType = 'danger' | 'help' | 'info' | 'secondary' | 'success' | 'warning';

interface InternalSMButtonProperties extends SMButtonProperties {
  readonly children?: react.ReactNode;
}

const SMButton = forwardRef<Button, InternalSMButtonProperties>(
  (
    {
      buttonLargeImage = false,
      buttonClassName = 'icon-blue',
      buttonDarkBackground = false,
      buttonDisabled = false,
      hollow = false,
      iconFilled = false,
      isLeft = false,
      buttonIsLoading: isLoading = false,
      buttonLarge = false,
      outlined = false,
      rounded = true,
      tooltip = '',
      ...props
    },
    ref
  ) => {
    const tooltipClassName = React.useMemo(() => {
      const ret = `smbutton-${uuidv4()} width-100`;
      if (buttonLarge) {
        return ret + ' sm-button-large';
      }
      return ret;
    }, [buttonLarge]);

    const getClassName = React.useMemo(() => {
      let toRet = 'sm-button';
      let cClass = buttonClassName;

      if (props.label && props.label !== '' && !props.children) {
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
    }, [buttonClassName, iconFilled, props.label, props.children, tooltipClassName]);

    const getStyle = useMemo(() => {
      return {
        // ...style,
        color: props.color
      };
    }, [props.color]);

    const iconClass = useMemo(() => {
      return isLoading ? 'pi pi-spin pi-spinner' : 'pi ' + props.icon;
    }, [props.icon, isLoading]);

    const getLabelColor = useMemo(() => {
      if (buttonClassName.includes('red')) return 'var(--icon-red)';
      if (buttonClassName.includes('orange')) return 'var(--icon-orange)';
      if (buttonClassName.includes('green')) return 'var(--icon-green)';
      if (buttonClassName.includes('blue')) return 'var(--icon-blue)';
      if (buttonClassName.includes('yellow')) return 'var(--icon-yellow)';
      return 'var(--icon-blue)'; // Default color
    }, [buttonClassName]);

    const getHollowStyle = useMemo((): CSSProperties => {
      return props.label ? { borderColor: getLabelColor } : {};
    }, [getLabelColor, props.label]);

    const getDiv = useMemo(() => {
      let divClass = 'flex align-items-center justify-content-center gap-1';
      if (props.label) {
        divClass += ' sm-menuitem';
      }
      return divClass;
    }, [props.label]);

    const getDarkDiv = useMemo(() => {
      if (buttonLargeImage === true) {
        return 'sm-input-dark-large-image';
      }
      let divClass = buttonLarge ? 'sm-input-dark-large' : 'sm-input-dark';

      return divClass;
    }, [buttonLarge, buttonLargeImage]);

    if (props.children) {
      if (buttonDarkBackground) {
        return (
          <div className="stringeditor">
            <div className={getDarkDiv}>
              <Tooltip target={`.${tooltipClassName}`} />
              <div
                onClick={(e) => {
                  e.preventDefault();
                  if (buttonDisabled === true) {
                    return;
                  }
                  props.onClick && props.onClick(e);
                }}
                className={`${tooltipClassName} input-wrapper`}
                data-pr-tooltip={tooltip}
                data-pr-position={isLeft ? 'left' : 'right'}
                data-pr-showdelay={400}
                data-pr-hidedelay={100}
                data-pr-autohide={true}
                ref={props.modal === true && props.modalCentered === true ? undefined : props.refs?.setReference}
                {...(!buttonDisabled && props.getReferenceProps ? props.getReferenceProps() : {})}
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
        <div className={buttonLarge ? 'sm-input-large' : 'sm-border-transparent sm-hover'}>
          <>
            <Tooltip target={`.${tooltipClassName}`} />
            <div
              onClick={(e) => {
                e.preventDefault();
                if (buttonDisabled === true) {
                  return;
                }
                props.onClick && props.onClick(e);
              }}
              className={`${tooltipClassName} input-wrapper`}
              data-pr-tooltip={tooltip}
              data-pr-position={isLeft ? 'left' : 'right'}
              data-pr-showdelay={400}
              data-pr-hidedelay={100}
              data-pr-autohide={true}
              ref={props.modal === true && props.modalCentered === true ? undefined : props.refs?.setReference}
              {...(!buttonDisabled && props.getReferenceProps ? props.getReferenceProps() : {})}
            >
              {props.children}
              <i className={`input-icon ${iconClass}`} />
            </div>
          </>
        </div>
      );
    }

    if (hollow) {
      return (
        <div
          onClick={(e) => {
            e.preventDefault();
            if (buttonDisabled === true) {
              return;
            }
            props.onClick && props.onClick(e);
          }}
        >
          <div
            ref={props.modal === true && props.modalCentered === true ? undefined : props.refs?.setReference}
            {...(!buttonDisabled && props.getReferenceProps ? props.getReferenceProps() : {})}
            className={getDiv}
            style={getHollowStyle}
          >
            <Tooltip target={`.${tooltipClassName}`} />
            <Button
              ref={ref}
              className={getClassName}
              disabled={buttonDisabled || isLoading}
              icon={iconClass}
              loading={isLoading}
              outlined={outlined}
              rounded={rounded}
              text={!iconFilled}
              tooltip={tooltip}
              tooltipOptions={isLeft ? getLeftToolOptions : getRightToolOptions}
              style={getStyle}
            />
            {props.label && <div className={buttonDisabled === true ? 'p-disabled' : undefined}>{props.label}</div>}
          </div>
        </div>
      );
    }

    return (
      <div
        onClick={(e) => {
          e.preventDefault();
          if (buttonDisabled === true) {
            return;
          }
          props.onClick && props.onClick(e);
        }}
      >
        <div
          ref={props.modal === true && props.modalCentered === true ? undefined : props.refs?.setReference}
          {...(!buttonDisabled && props.getReferenceProps ? props.getReferenceProps() : {})}
        >
          <Tooltip target={`.${tooltipClassName}`} />
          <Button
            ref={ref}
            className={getClassName}
            disabled={buttonDisabled || isLoading}
            icon={iconClass}
            label={props.label}
            loading={isLoading}
            outlined={outlined}
            rounded={rounded}
            text={!iconFilled}
            tooltip={tooltip}
            tooltipOptions={isLeft ? getLeftToolOptions : getRightToolOptions}
            style={getStyle}
          />
        </div>
      </div>
    );
  }
);

export default SMButton;
