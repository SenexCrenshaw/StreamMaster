import { ReactNode, forwardRef, useCallback, useImperativeHandle, useMemo, useRef } from 'react';
import { SMDropDownProperties } from './Interfaces/SMDropDownProperties';
import SMOverlay, { SMOverlayRef } from './SMOverlay';
import SMScroller from './SMScroller';

interface ExtendedSMDropDownProperties extends SMDropDownProperties {
  readonly children?: ReactNode;
}

export interface SMDropDownRef {
  hide: () => void;
}
const SMDropDown = forwardRef<SMDropDownRef, ExtendedSMDropDownProperties>(
  (
    {
      closeOnSelection = true,
      autoPlacement = false,
      hasCloseButton = true,
      showClose = false,
      buttonIsLoading: isLoading = false,
      itemSize = 26,
      labelInline = false,
      scrollHeight = '40vh',
      info = '',
      zIndex = 10,
      ...props
    },
    ref
  ) => {
    useImperativeHandle(ref, () => ({
      hide: () => smOverlayRef.current?.hide(),
      props
    }));

    const smOverlayRef = useRef<SMOverlayRef | null>(null);

    const getDiv = useMemo(() => {
      let ret = 'sm-w-12 flex';

      if (labelInline) {
        ret += ' justify-content-between align-items-center flex-row';
      } else {
        ret += ' flex-column ';
      }

      return ret;
    }, [labelInline]);

    const getSMOverlay = useCallback(() => {
      const spreadProps = props as Required<SMDropDownProperties>;
      return (
        <SMOverlay
          hasCloseButton={hasCloseButton}
          showClose={showClose}
          info={info}
          autoPlacement={autoPlacement}
          icon="pi-chevron-down"
          buttonIsLoading={isLoading}
          ref={smOverlayRef}
          zIndex={zIndex}
          {...props}
        >
          <div className="sm-card">
            {props.children ? (
              props.children
            ) : (
              <SMScroller
                {...spreadProps}
                scrollHeight={scrollHeight}
                onChange={(e) => {
                  props.onChange?.(e);
                  if (closeOnSelection) smOverlayRef.current?.hide();
                }}
              />
            )}
            {props.footerTemplate && (
              <div className="w-12">
                <div className="layout-padding-bottom" />
                {props.footerTemplate}
                <div className="layout-padding-bottom" />
              </div>
            )}
          </div>
        </SMOverlay>
      );
    }, [autoPlacement, closeOnSelection, hasCloseButton, info, isLoading, props, scrollHeight, showClose, zIndex]);

    return (
      <>
        {props.label && !labelInline && (
          <>
            <label className="pl-15">{props.label.toUpperCase()}</label>
          </>
        )}
        <div className={getDiv}>
          {props.label && labelInline && <div className={labelInline ? 'w-5' : 'w-12'}>{props.label.toUpperCase()}</div>}
          <div className={labelInline ? 'w-7' : 'w-12'}>{getSMOverlay()}</div>
        </div>
      </>
    );
  }
);
export default SMDropDown;
