import { forwardRef, useImperativeHandle, useMemo, useRef } from 'react';
import SMOverlay, { SMOverlayRef } from './SMOverlay';
import SMScroller from './SMScroller';
import { SMOverlayProperties } from './interfaces/SMOverlayProperties';

export interface SMDropDownRef {
  hide: () => void;
}
interface SMDropDownProps extends SMOverlayProperties {
  readonly closeOnSelection?: boolean;
  readonly data: any;
  readonly dataKey?: string;
  readonly filter?: boolean;
  readonly filterBy?: string;
  readonly footerTemplate?: React.ReactNode;
  readonly itemTemplate: (item: any) => React.ReactNode;
  readonly itemSize?: number;
  readonly labelInline?: boolean;
  readonly onChange?: (value: any) => void;
  readonly optionValue?: string;
  readonly height?: string;
  readonly selectedItemsKey?: string;
  readonly select?: boolean;
  readonly value?: any;
}

const SMDropDown = forwardRef<SMDropDownRef, SMDropDownProps>(
  (
    {
      className = 'w-full',
      closeOnSelection = true,
      autoPlacement = false,
      isLoading = false,
      itemSize = 26,
      itemTemplate,
      labelInline = false,
      onChange,
      height = '40vh',
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
      let div = 'w-full';

      if (props.label) {
        div += ' flex-column';
      }

      return div;
    }, [props.label]);

    return (
      <div className={className}>
        {props.label && !labelInline && (
          <div className="w-6">
            <label className="pl-14">{props.label.toUpperCase()}</label>
            <div className="pt-small" />
          </div>
        )}
        <div className={getDiv}>
          <SMOverlay autoPlacement={autoPlacement} icon="pi-chevron-down" isLoading={isLoading} ref={smOverlayRef} zIndex={zIndex} {...props}>
            <div className="sm-card border-radius-left border-radius-right">
              <SMScroller
                scrollHeight={height}
                itemSize={itemSize}
                itemTemplate={itemTemplate}
                onChange={(e) => {
                  onChange?.(e);
                  if (closeOnSelection) smOverlayRef.current?.hide();
                }}
                {...props}
              />

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
      </div>
    );
  }
);
export default SMDropDown;
