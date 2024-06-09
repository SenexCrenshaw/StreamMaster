import EPGEditor from '@components/epg/EPGEditor';
import SMDropDown from '@components/sm/SMDropDown';

import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { isEmptyObject } from '@lib/common/common';
import { useSMContext } from '@lib/signalr/SMProvider';
import useGetEPGColors from '@lib/smAPI/EPG/useGetEPGColors';
import useGetEPGFiles from '@lib/smAPI/EPGFiles/useGetEPGFiles';
import { EPGFileDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { ReactNode, useCallback, useMemo } from 'react';

export const useSMChannelEPGColumnConfig = () => {
  const dataKey = 'epgColumn-selections';
  const { data, isLoading } = useGetEPGFiles();
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
  }, [data, isSystemReady, settings]);

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
          <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
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

        return <div className="text-container pl-1">{sortedInput.join(', ')}</div>;
      }
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">EPG</div>
      </div>
    );
  }, []);

  function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    return (
      <div className="w-full">
        <SMDropDown
          buttonDarkBackground
          buttonTemplate={buttonTemplate(options)}
          data={epgFiles}
          dataKey="Id"
          isLoading={isLoading}
          itemTemplate={itemTemplate}
          filter
          filterBy="Name"
          fixed
          onChange={async (e: any) => {
            if (isEmptyObject(e) || !Array.isArray(e)) {
              options.filterApplyCallback();
            } else {
              options.filterApplyCallback(e);
            }
          }}
          select
          selectedItemsKey={dataKey}
          title={'EPG'}
          contentWidthSize="2"
        />
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
    minWidth: '6',
    sortable: true,
    width: '10'
  };

  return columnConfig;
};
