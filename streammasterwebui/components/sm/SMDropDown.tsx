import { ReactNode, forwardRef, useCallback, useImperativeHandle, useMemo, useRef } from 'react';
import SMOverlay, { SMOverlayRef } from './SMOverlay';
import SMScroller from './SMScroller';
import { SMDropDownProperties } from './interfaces/SMDropDownProperties';

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
      buttonIsLoading: isLoading = false,
      itemSize = 26,
      labelInline = false,
      scrollHeight = '40vh',
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
      if (props.label && !labelInline) {
        return 'flex-column';
      }

      return '';
    }, [labelInline, props.label]);

    const getSMOverlay = useCallback(
      (children: ReactNode) => {
        return (
          <div className={getDiv}>
            <SMOverlay autoPlacement={autoPlacement} icon="pi-chevron-down" buttonIsLoading={isLoading} ref={smOverlayRef} zIndex={zIndex} {...props}>
              <div className="sm-card border-radius-left border-radius-right">
                {children}
                {props.footerTemplate && (
                  <div className="w-12">
                    <div className="layout-padding-bottom" />
                    {props.footerTemplate}
                    <div className="layout-padding-bottom" />
                  </div>
                )}
              </div>
            </SMOverlay>
          </div>
        );
      },
      [autoPlacement, getDiv, isLoading, props, zIndex]
    );

    // Logger.debug('SMDropDown', { value: props.value, label: props.label });
    const spreadProps = props as Required<SMDropDownProperties>;
    return (
      <>
        {props.label && !labelInline && (
          <>
            <label className="pl-15">{props.label.toUpperCase()}</label>
          </>
        )}
        <div className={getDiv}>
          {props.label && labelInline && <div className={labelInline ? 'w-4' : 'w-6'}>{props.label.toUpperCase()}</div>}
          {props.children
            ? getSMOverlay(props.children)
            : getSMOverlay(
                <SMScroller
                  {...spreadProps}
                  scrollHeight={scrollHeight}
                  onChange={(e) => {
                    props.onChange?.(e);
                    if (closeOnSelection) smOverlayRef.current?.hide();
                  }}
                />
              )}
        </div>
      </>
    );
  }
);
export default SMDropDown;
