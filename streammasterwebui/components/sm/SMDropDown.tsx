import { ReactNode, Suspense, lazy, useRef } from 'react';
import SMOverlay from './SMOverlay';

interface SMDropDownProps {
  readonly buttonDarkBackground?: boolean;
  readonly buttonLabel?: string;
  readonly buttonTemplate: ReactNode;
  readonly data: any;
  readonly dataKey?: string;
  readonly isLoading?: boolean;
  readonly filter?: boolean;
  readonly filterBy?: string;
  readonly itemTemplate: (item: any) => React.ReactNode;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly onChange: (value: any) => void;
  readonly value?: any;
}

const SMDropDown = ({
  buttonDarkBackground,
  buttonLabel,
  buttonTemplate,
  data,
  dataKey,
  filter,
  filterBy,
  isLoading,
  itemTemplate,
  label,
  labelInline = false,
  onChange,
  value
}: SMDropDownProps) => {
  const SMScroller = lazy(() => import('@components/sm/SMScroller'));
  const divReference = useRef<HTMLDivElement | null>(null);

  return (
    <>
      {label && !labelInline && (
        <div className="w-6">
          <label className="pl-14">{label.toUpperCase()}</label>
          <div className="pt-small" />
        </div>
      )}
      <div ref={divReference} className={`sm-dropdown ${labelInline ? 'align-items-center' : 'flex-column align-items-start'}`}>
        <SMOverlay
          buttonDarkBackground={buttonDarkBackground}
          title={label?.toUpperCase()}
          widthSize="2"
          icon="pi-chevron-down"
          buttonTemplate={buttonTemplate}
          buttonLabel={buttonLabel}
          isLoading={isLoading}
        >
          <div className="flex flex-row w-12 sm-card border-radius-left border-radius-right">
            <Suspense fallback={<div>Loading...</div>}>
              <div className="flex w-12">
                <SMScroller
                  data={data}
                  dataKey={dataKey}
                  filter={filter}
                  filterBy={filterBy}
                  itemSize={26}
                  itemTemplate={itemTemplate}
                  onChange={(e) => {
                    onChange(e);
                  }}
                  value={value}
                />
              </div>
            </Suspense>
          </div>
        </SMOverlay>
      </div>
    </>
  );
};
export default SMDropDown;
