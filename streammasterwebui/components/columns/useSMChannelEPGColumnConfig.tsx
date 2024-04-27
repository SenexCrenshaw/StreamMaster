import EPGEditor from '@components/epg/EPGEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { isEmptyObject } from '@lib/common/common';
import useGetEPGColors from '@lib/smAPI/EPG/useGetEPGColors';
import useGetEPGFiles from '@lib/smAPI/EPGFiles/useGetEPGFiles';
import { EPGFileDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
import { ReactNode, useCallback, useMemo } from 'react';

export const useSMChannelEPGColumnConfig = () => {
  const { data } = useGetEPGFiles();
  const colorsQuery = useGetEPGColors();

  const epgFiles = useMemo(() => {
    const additionalOptions = [{ EPGNumber: -1, Name: 'SD' } as EPGFileDto, { EPGNumber: -99, Name: 'None' } as EPGFileDto];
    if (data) return [...additionalOptions, ...data];

    return additionalOptions;
  }, [data]);

  const itemTemplate = useCallback(
    (option: EPGFileDto) => {
      let color = '#FFFFFF';

      if (colorsQuery?.data !== undefined) {
        const entry = colorsQuery.data.find((x) => x.EPGNumber === option.EPGNumber);
        if (entry?.Color) {
          color = entry.Color;
        }
      }

      return (
        <div className="flex align-items-center gap-2">
          <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
          <span>{option.Name}</span>
        </div>
      );
    },
    [colorsQuery]
  );

  const selectedItemTemplate = useCallback(
    (option: EPGFileDto) => {
      if (option === undefined) {
        return null;
      }

      let color = '#FFFFFF';

      if (colorsQuery?.data !== undefined) {
        const entry = colorsQuery.data.find((x) => x.EPGNumber === option.EPGNumber);
        if (entry?.Color) {
          color = entry.Color;
        }
      }

      return (
        <div className="flex align-items-center gap-2">
          <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
          <span>{option.Name}</span>
        </div>
      );
    },
    [colorsQuery]
  );

  function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    return (
      <MultiSelect
        className="w-11"
        filter
        itemTemplate={itemTemplate}
        maxSelectedLabels={1}
        onChange={(e: MultiSelectChangeEvent) => {
          if (isEmptyObject(e.value)) {
            options.filterApplyCallback();
          } else {
            options.filterApplyCallback(e.value);
          }
        }}
        options={epgFiles}
        placeholder="Any"
        value={options.value}
        selectedItemTemplate={selectedItemTemplate}
      />
    );
  }
  const bodyTemplate = (bodyData: SMChannelDto) => {
    return <EPGEditor data={bodyData} />;
  };

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate: bodyTemplate,
    field: 'EPGId',
    filter: true,
    filterElement: filterTemplate,
    header: 'EPG',
    maxWidth: '14rem',
    minWidth: '14rem',
    sortable: true,
    width: '14rem'
  };

  return columnConfig;
};
