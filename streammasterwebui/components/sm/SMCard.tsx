import CloseButton from '@components/buttons/CloseButton';
import OKButton from '@components/buttons/OKButton';
import { useMemo } from 'react';
import { SMCardProperties } from './Interfaces/SMCardProperties';

interface InternalSMCardProperties extends SMCardProperties {
  readonly children: React.ReactNode;
}

export const SMCard = ({
  children,
  closeButtonDisabled = false,
  darkBackGround = false,
  header,
  info = 'General',
  noBorderChildren = false,
  noCloseButton = false,
  okButtonDisabled = false,
  onCloseClick,
  closeToolTip = 'Close',
  okToolTip = 'Ok',
  onOkClick,
  simple = false,
  simpleChildren = true,
  title,
  ...props
}: InternalSMCardProperties) => {
  const getDiv = useMemo(() => {
    let ret = darkBackGround === true ? 'sm-card-dark' : 'sm-card';
    ret += simpleChildren === true ? '-simple' : '';
    return ret;
  }, [darkBackGround, simpleChildren]);

  const borderClass = info !== '' ? 'sm-border-bottom' : 'info-header-text';

  // useEffect(() => {
  //   if (props.answer !== undefined) {
  //     props.onAnswered?.();
  //     props.answer = undefined;
  //     return;
  //   }
  // }, [props, props.answer]);

  if (simple === true || title === undefined || title === '') {
    return <div>{children}</div>;
  }

  if (props.center && props.center !== '') {
    return (
      <div className={getDiv}>
        <div className="sm-card-header flex flex-row justify-content-between align-items-center w-full">
          <div className="header-text-sub w-4 flex justify-content-start">{title}</div>
          <div className="w-4 flex justify-content-center">{props.center}</div>
          <div className="flex justify-content-end w-4 gap-1 pr-1">
            {header}
            {!noCloseButton && (
              <CloseButton
                onClick={(e) => {
                  onCloseClick?.();
                }}
                tooltip={closeToolTip}
              />
            )}
            {onOkClick && (
              <OKButton
                buttonDisabled={okButtonDisabled}
                onClick={(e) => {
                  onOkClick?.();
                }}
                tooltip={okToolTip}
              />
            )}
          </div>
        </div>
        <div className="layout-padding-bottom" />
        <div className={noBorderChildren ? 'sm-card-children-noborder' : 'sm-card-children'}>
          <div className="sm-card-children-content">
            {info && info !== '' && <div className={`${borderClass} sm-card-children-info`}>{info}</div>}
            {children}
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className={getDiv}>
      <div className="sm-card-header flex flex-row justify-content-between align-items-center w-full">
        <div className="header-text-sub flex justify-content-start">{title}</div>
        <div className="flex justify-content-end gap-1 pr-1">
          {header}
          {onOkClick && (
            <OKButton
              buttonDisabled={okButtonDisabled}
              onClick={(e) => {
                onOkClick?.();
              }}
              tooltip={okToolTip}
            />
          )}
          {(!noCloseButton || onCloseClick) && <CloseButton buttonDisabled={closeButtonDisabled} onClick={(e) => onCloseClick?.()} tooltip={closeToolTip} />}
        </div>
      </div>

      {children && children !== '' && (
        <>
          <div className="layout-padding-bottom" />
          <div className={noBorderChildren ? 'sm-card-children-noborder' : 'sm-card-children'}>
            <div className="sm-card-children-content">
              {info && info !== '' && <div className={`${borderClass} sm-card-children-info`}>{info}</div>}
              {children}
            </div>
          </div>
        </>
      )}
    </div>
  );
};
