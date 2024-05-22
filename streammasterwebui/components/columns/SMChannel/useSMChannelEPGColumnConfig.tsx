import EPGEditor from '@components/epg/EPGEditor';
import { SMOverlay } from '@components/sm/SMOverlay';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { isEmptyObject } from '@lib/common/common';
import { useSMContext } from '@lib/signalr/SMProvider';
import useGetEPGColors from '@lib/smAPI/EPG/useGetEPGColors';
import useGetEPGFiles from '@lib/smAPI/EPGFiles/useGetEPGFiles';
import { EPGFileDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';

import { ReactNode, Suspense, lazy, useCallback, useMemo } from 'react';

const SMScroller = lazy(() => import('@components/sm/SMScroller'));

interface SMChannelEPGColumnConfigProperties {
  readonly width?: string;
}

export const useSMChannelEPGColumnConfig = ({ width = '8rem' }: SMChannelEPGColumnConfigProperties) => {
  const dataKey = 'epgColumn-selections';
  const { data } = useGetEPGFiles();
  const colorsQuery = useGetEPGColors();
  const { isSystemReady, settings } = useSMContext();

  const epgFiles = useMemo(() => {
    let additionalOptions = [] as EPGFileDto[];
    if (settings.SDSettings?.SDEnabled === undefined) {
      console.log('sa', isSystemReady);
      return;
    }
    if (settings.SDSettings.SDEnabled === true) {
      additionalOptions = [{ EPGNumber: -1, Id: -1, Name: 'SD' } as EPGFileDto];
    }
    additionalOptions.push({ EPGNumber: -99, Id: -99, Name: 'None' } as EPGFileDto);

    if (data) return [...additionalOptions, ...data];

    return additionalOptions;
  }, [data, isSystemReady, settings.SDSettings.SDEnabled]);

  const getColor = useCallback(
    (epgNumber: number) => {
      let color = '#FFFFFF';
      if (epgNumber < 0) {
        if (epgNumber === -99) {
          color = '#000000';
        }
      }

      if (epgNumber > 0 && colorsQuery?.data !== undefined) {
        const entry = colorsQuery.data.find((x) => x.EPGNumber === epgNumber);
        if (entry?.Color) {
          color = entry.Color;
        }
      }
      return color;
    },
    [colorsQuery]
  );

  const itemTemplate = useCallback(
    (option: EPGFileDto) => {
      if (option === undefined) {
        return null;
      }
      const color = getColor(option.EPGNumber);

      return (
        <>
          <i className="pi pi-circle-fill pr-2 " style={{ color: color }} />
          <span className="text-container">{option.Name}</span>
        </>
      );
    },
    [getColor]
  );

  const buttonTemplate = useCallback((options: any): ReactNode => {
    if (Array.isArray(options.value)) {
      if (options.value.length > 0) {
        const names = options.value.map((x: EPGFileDto) => x.Name);
        const sortedInput = [...names].sort();
        // const suffix = names.length > 2 ? ',...' : '';
        return <div className="text-container">{sortedInput.join(', ')}</div>;
      }
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">EPG</div>
      </div>
    );
  }, []);

  function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    console.log('options', options);
    return (
      <div className="sm-input-border-dark w-full">
        <SMOverlay title="EPG FILE" widthSize="2" icon="pi-chevron-down" buttonTemplate={buttonTemplate(options)} buttonLabel="EPG">
          <div className="flex flex-row w-12 sm-card border-radius-left border-radius-right">
            <Suspense fallback={<div>Loading...</div>}>
              <div className="flex w-full">
                <SMScroller
                  data={epgFiles}
                  dataKey="Id"
                  filter
                  filterBy="Name"
                  itemSize={26}
                  itemTemplate={itemTemplate}
                  onChange={async (e: any) => {
                    console.log('e', e);
                    if (isEmptyObject(e) || !Array.isArray(e)) {
                      options.filterApplyCallback();
                    } else {
                      options.filterApplyCallback(e);
                    }
                  }}
                  scrollHeight={150}
                  select
                  selectedItemsKey={dataKey}
                  value={options.value}
                />
              </div>
            </Suspense>
          </div>
        </SMOverlay>
      </div>
    );
  }
  const bodyTemplate = useCallback((smChannel: SMChannelDto) => {
    return <EPGEditor data={{ ...smChannel }} />;
  }, []);

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate: bodyTemplate,
    field: 'EPGId',
    filter: true,
    filterElement: filterTemplate,
    header: 'EPG',
    maxWidth: width,
    minWidth: width,
    sortable: true,
    width: width
  };

  return columnConfig;
};
