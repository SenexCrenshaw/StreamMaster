import { ReactNode, Suspense, lazy, useRef } from 'react';
import SMOverlay from './SMOverlay';
import { Logger } from '@lib/common/logger';

interface SMDropDownProps {
  readonly buttonDarkBackground?: boolean;
  readonly buttonLabel?: string;
  readonly buttonTemplate: ReactNode;
  readonly data: any;
  readonly dataKey?: string;
  readonly filter?: boolean;
  readonly filterBy?: string;
  readonly itemTemplate: (item: any) => React.ReactNode;
  readonly label: string;
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
  itemTemplate,
  label,
  labelInline = false,
  onChange,
  value
}: SMDropDownProps) => {
  const SMScroller = lazy(() => import('@components/sm/SMScroller'));
  const divReference = useRef<HTMLDivElement | null>(null);
  Logger.debug('SMDropDown', { label, labelInline, buttonDarkBackground, buttonLabel, buttonTemplate, data, dataKey, filter, filterBy, itemTemplate, value });
  return (
    <div className="flex justify-content-start align-items-center ">
      {label && !labelInline && (
        <div className="w-6">
          <label className="pl-14">{label.toUpperCase()}</label>
          <div className="pt-small" />
        </div>
      )}
      <div ref={divReference} className={`flex w-6  sm-dropdown ${labelInline ? 'align-items-center' : 'flex-column align-items-start'}`}>
        <SMOverlay
          buttonDarkBackground={buttonDarkBackground}
          title={label.toUpperCase()}
          widthSize="2"
          icon="pi-chevron-down"
          buttonTemplate={buttonTemplate}
          buttonLabel={buttonLabel}
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
                  scrollHeight={150}
                  value={value}
                />
              </div>
            </Suspense>
          </div>
        </SMOverlay>
      </div>
    </div>
  );
};
export default SMDropDown;
