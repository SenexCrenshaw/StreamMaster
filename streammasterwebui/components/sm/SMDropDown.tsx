import { ReactNode, Suspense, lazy, useMemo, useRef } from 'react';
import SMOverlay from './SMOverlay';

interface SMDropDownProps {
  readonly buttonDarkBackground?: boolean;
  readonly buttonLabel?: string;
  readonly buttonTemplate?: ReactNode;
  readonly center?: React.ReactNode;
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
  readonly widthSize?: string;
}

const SMDropDown = ({
  buttonDarkBackground,
  buttonLabel,
  buttonTemplate,
  center,
  data,
  dataKey,
  filter,
  filterBy,
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
  widthSize
}: SMDropDownProps) => {
  const SMScroller = lazy(() => import('@components/sm/SMScroller'));
  const divReference = useRef<HTMLDivElement | null>(null);

  const getDiv = useMemo(() => {
    let div = 'sm-dropdown';
    if (label) {
      div += ' flex-column';
    }

    return div;
  }, [label]);

  return (
    <>
      {label && !labelInline && (
        <div className="w-6">
          <label className="pl-14">{label.toUpperCase()}</label>
          <div className="pt-small" />
        </div>
      )}
      <div ref={divReference} className={getDiv}>
        <SMOverlay
          buttonDarkBackground={buttonDarkBackground}
          buttonLabel={buttonLabel}
          buttonTemplate={buttonTemplate}
          center={center}
          icon="pi-chevron-down"
          isLoading={isLoading}
          simple={simple}
          title={title?.toUpperCase()}
          widthSize={widthSize}
        >
          <div className="flex flex-column w-12 sm-card border-radius-left border-radius-right">
            <Suspense fallback={<div>Loading...</div>}>
              <div className="flex w-12">
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
                  }}
                  optionValue={optionValue}
                  value={value}
                />
              </div>
            </Suspense>
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
    </>
  );
};
export default SMDropDown;
