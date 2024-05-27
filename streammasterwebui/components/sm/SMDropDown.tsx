import { ReactNode, Suspense, lazy, useRef } from 'react';
import SMOverlay from './SMOverlay';

interface SMDropDownProps {
  readonly buttonDarkBackground?: boolean;
  readonly buttonLabel?: string;
  readonly buttonTemplate?: ReactNode;
  readonly data: any;
  readonly dataKey?: string;
  readonly isLoading?: boolean;
  readonly filter?: boolean;
  readonly filterBy?: string;
  readonly height?: string;
  readonly itemTemplate: (item: any) => React.ReactNode;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly onChange?: (value: any) => void;
  readonly select?: boolean;
  readonly simple?: boolean;
  readonly selectedItemsKey?: string;
  readonly title?: string;
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
  height = '40vh',
  select,
  selectedItemsKey,
  simple,
  title,
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
          buttonLabel={buttonLabel}
          buttonTemplate={buttonTemplate}
          icon="pi-chevron-down"
          isLoading={isLoading}
          simple={simple}
          title={title?.toUpperCase()}
          widthSize="2"
        >
          <div className="flex flex-row w-12 sm-card border-radius-left border-radius-right">
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
