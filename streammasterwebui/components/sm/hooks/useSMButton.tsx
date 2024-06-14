import React, { CSSProperties, useEffect, useMemo } from 'react';
import SMButton from '../SMButton';
import { useSMButtonContext } from '../context/SMButtonContext';
import { SMButtonProperties } from '../interfaces/SMButtonProperties';

export const useSMButton = ({
  buttonClassName = 'icon-red',
  buttonDarkBackground = false,
  buttonDisabled = false,
  buttonLabel,
  buttonLarge = false,
  buttonTemplate: buttonTemplate,
  getReferenceProps,
  icon,
  iconFilled = false,
  buttonIsLoading: isLoading = false,
  label,
  modal,
  modalCentered,
  onClick,
  refs,
  tooltip,
  ...props
}: SMButtonProperties & { getReferenceProps?: () => any; refs?: { setReference: React.Ref<any> } }) => {
  const { buttonDisabled: contextButtonDisabled, setDisabled, setEnabled } = useSMButtonContext();

  useEffect(() => {
    buttonDisabled ? setDisabled() : setEnabled();
  }, [buttonDisabled, setDisabled, setEnabled]);

  const getLabelColor = useMemo(() => {
    if (buttonClassName.includes('red')) return 'var(--icon-red)';
    if (buttonClassName.includes('orange')) return 'var(--icon-orange)';
    if (buttonClassName.includes('green')) return 'var(--icon-green)';
    if (buttonClassName.includes('blue')) return 'var(--icon-blue)';
    return 'var(--icon-blue)'; // Default color
  }, [buttonClassName]);

  const getStyle = useMemo((): CSSProperties => {
    return label ? { borderColor: getLabelColor } : {};
  }, [getLabelColor, label]);

  const getDiv = useMemo(() => {
    let divClass = 'flex align-items-center justify-content-center gap-1';
    if (label) {
      divClass += ' sm-menuitem';
    }
    return divClass;
  }, [label]);

  const isDisabled = useMemo(() => buttonDisabled || contextButtonDisabled, [buttonDisabled, contextButtonDisabled]);

  // if (icon === 'pi-bars') {
  //   Logger.debug('useSMButton', 'isLoading', isLoading, 'buttonDisabled', buttonDisabled, props);
  // }

  const buttonElement = useMemo(() => {
    if (buttonTemplate) {
      return (
        <div onClick={onClick}>
          <div
            ref={modal === true && modalCentered === true ? undefined : refs?.setReference}
            {...(!isDisabled && getReferenceProps ? getReferenceProps() : {})}
          >
            <SMButton
              buttonDarkBackground={buttonDarkBackground}
              buttonDisabled={isDisabled}
              buttonClassName={buttonClassName}
              iconFilled={iconFilled}
              icon={icon}
              buttonIsLoading={isLoading}
              large={buttonLarge}
              tooltip={tooltip}
              label={buttonLabel}
            >
              {buttonTemplate}
              {label && <div className={isDisabled === true ? 'p-disabled' : undefined}>{label}</div>}
            </SMButton>
          </div>
        </div>
      );
    }

    return (
      <div onClick={onClick}>
        <div
          ref={modal === true && modalCentered === true ? undefined : refs?.setReference}
          {...(!isDisabled && getReferenceProps ? getReferenceProps() : {})}
          className={getDiv}
          style={getStyle}
        >
          <SMButton
            buttonDarkBackground={buttonDarkBackground}
            // buttonDisabled={isDisabled}
            buttonClassName={buttonClassName}
            iconFilled={iconFilled}
            icon={icon}
            buttonIsLoading={isLoading}
            large={buttonLarge}
            tooltip={tooltip}
            label={buttonLabel}
          />
          {label && <div className={isDisabled === true ? 'p-disabled' : undefined}>{label}</div>}
        </div>
      </div>
    );
  }, [
    buttonTemplate,
    modal,
    modalCentered,
    refs?.setReference,
    isDisabled,
    getReferenceProps,
    getDiv,
    getStyle,
    buttonDarkBackground,
    buttonClassName,
    iconFilled,
    icon,
    isLoading,
    buttonLarge,
    tooltip,
    buttonLabel,
    onClick,
    label
  ]);

  return {
    buttonElement,
    setDisabled,
    setEnabled
  };
};
