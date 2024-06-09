import { ReactNode, forwardRef, useImperativeHandle, useMemo, useRef } from 'react';
import SMOverlay, { SMOverlayRef } from './SMOverlay';
import SMScroller from './SMScroller';
interface SMDropDownProps {
  readonly buttonDarkBackground?: boolean;
  readonly buttonLabel?: string;
  readonly buttonTemplate?: ReactNode;
  readonly buttonLarge?: boolean;
  readonly center?: React.ReactNode;
  readonly className?: string;
  readonly closeOnSelection?: boolean;
  readonly data: any;
  readonly dataKey?: string;
  readonly footerTemplate?: ReactNode;
  readonly isLoading?: boolean;
  readonly filter?: boolean;
  readonly filterBy?: string;
  readonly height?: string;
  readonly itemTemplate: (item: any) => React.ReactNode;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly onChange?: (value: any) => void;
  readonly optionValue?: string;
  readonly select?: boolean;
  readonly simple?: boolean;
  readonly selectedItemsKey?: string;
  readonly title?: string;
  readonly value?: any;
  readonly contentWidthSize?: string;
  readonly fixed?: boolean;
}

export interface SMDropDownRef {
  hide: () => void;
}

const SMDropDown = forwardRef<SMDropDownRef, SMDropDownProps>((props: SMDropDownProps, ref) => {
  const {
    buttonDarkBackground,
    buttonLabel,
    buttonLarge = false,
    buttonTemplate,
    center,
    className = 'w-full',
    closeOnSelection = true,
    data,
    dataKey,
    filter,
    filterBy,
    fixed = false,
    footerTemplate,
    isLoading,
    itemTemplate,
    label,
    labelInline = false,
    onChange,
    optionValue,
    height = '40vh',
    select,
    selectedItemsKey,
    simple,
    title,
    value,
    contentWidthSize
  } = props;

  useImperativeHandle(ref, () => ({
    hide: () => smOverlayRef.current?.hide(),
    props
  }));

  const smOverlayRef = useRef<SMOverlayRef | null>(null);

  const getDiv = useMemo(() => {
    let div = 'w-full';

    if (label) {
      div += ' flex-column';
    }

    return div;
  }, [label]);

  return (
    <div className={className}>
      {label && !labelInline && (
        <div className="w-6">
          <label className="pl-14">{label.toUpperCase()}</label>
          <div className="pt-small" />
        </div>
      )}
      <div className={getDiv}>
        <SMOverlay
          buttonDarkBackground={buttonDarkBackground}
          buttonLabel={buttonLabel}
          buttonLarge={buttonLarge}
          buttonTemplate={buttonTemplate}
          center={center}
          fixed={fixed}
          icon="pi-chevron-down"
          isLoading={isLoading}
          ref={smOverlayRef}
          simple={simple}
          title={title?.toUpperCase()}
          contentWidthSize={contentWidthSize}
        >
          <div className="sm-card border-radius-left border-radius-right">
            <SMScroller
              scrollHeight={height}
              select={select}
              selectedItemsKey={selectedItemsKey}
              data={data}
              dataKey={dataKey}
              filter={filter}
              filterBy={filterBy}
              simple={simple}
              itemSize={22}
              itemTemplate={itemTemplate}
              onChange={(e) => {
                onChange?.(e);
                if (closeOnSelection) smOverlayRef.current?.hide();
              }}
              optionValue={optionValue}
              value={value}
            />

            {footerTemplate && (
              <div className="w-12">
                <div className="layout-padding-bottom" />
                {footerTemplate}
                <div className="layout-padding-bottom" />
              </div>
            )}
          </div>
        </SMOverlay>
      </div>
    </div>
  );
});
export default SMDropDown;
