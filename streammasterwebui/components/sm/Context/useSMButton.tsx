import React, { CSSProperties, useEffect, useMemo } from 'react';
import { SMButtonProperties } from '../Interfaces/SMButtonProperties';
import SMButton from '../SMButton';
import { useSMButtonContext } from './SMButtonContext';

export const useSMButton = ({
  buttonClassName = 'icon-red',
  buttonDarkBackground = false,
  buttonDisabled = false,
  buttonLabel,
  buttonLarge = false,
  buttonTemplate,
  getReferenceProps,
  icon,
  iconFilled = false,
  isLoading = false,
  label,
  modal,
  modalCentered,
  refs,
  tooltip,
  ...props
}: SMButtonProperties & { getReferenceProps?: () => any; refs: { setReference: React.Ref<any> } }) => {
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

  // if (modalCentered) {
  //   Logger.debug('SMButton', { modal: modal, modalCentered: modalCentered });
  // }

  const buttonElement = useMemo(() => {
    if (buttonTemplate) {
      return (
        <div ref={modal === true && modalCentered === true ? undefined : refs.setReference} {...(!isDisabled && getReferenceProps ? getReferenceProps() : {})}>
          <SMButton
            darkBackGround={buttonDarkBackground}
            disabled={isDisabled}
            className={buttonClassName}
            iconFilled={iconFilled}
            icon={icon}
            isLoading={isLoading}
            large={buttonLarge}
            tooltip={tooltip}
            label={buttonLabel}
          >
            {buttonTemplate}
            {label}
          </SMButton>
        </div>
      );
    }

    return (
      <div
        ref={modal === true && modalCentered === true ? undefined : refs.setReference}
        {...(!isDisabled && getReferenceProps ? getReferenceProps() : {})}
        className={getDiv}
        style={getStyle}
      >
        <SMButton
          darkBackGround={buttonDarkBackground}
          disabled={isDisabled}
          className={buttonClassName}
          iconFilled={iconFilled}
          icon={icon}
          isLoading={isLoading}
          large={buttonLarge}
          tooltip={tooltip}
          label={buttonLabel}
        />
        {label && <div className="sm-menuitsem2">{label}</div>}
      </div>
    );
  }, [
    buttonTemplate,
    modal,
    modalCentered,
    refs.setReference,
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
    label
  ]);

  return {
    buttonElement,
    setDisabled,
    setEnabled
  };
};
