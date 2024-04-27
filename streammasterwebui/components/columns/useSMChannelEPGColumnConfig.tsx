import EPGEditor from '@components/epg/EPGEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { isEmptyObject } from '@lib/common/common';
import useGetEPGColors from '@lib/smAPI/EPG/useGetEPGColors';
import useGetEPGFiles from '@lib/smAPI/EPGFiles/useGetEPGFiles';
import { EPGFileDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
import { ReactNode, useCallback, useMemo, useRef } from 'react';

export const useSMChannelEPGColumnConfig = () => {
  const { data } = useGetEPGFiles();
  const colorsQuery = useGetEPGColors();

  const multiSelectRef = useRef<MultiSelect>(null);
  const epgFiles = useMemo(() => {
    const additionalOptions = [{ EPGNumber: -1, Id: -1, Name: 'SD' } as EPGFileDto, { EPGNumber: -99, Id: -99, Name: 'None' } as EPGFileDto];
    if (data) return [...additionalOptions, ...data];

    return additionalOptions;
  }, [data]);

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
      const color = getColor(option.EPGNumber);

      return (
        <div className="flex align-items-center gap-2">
          <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
          <span>{option.Name}</span>
        </div>
      );
    },
    [getColor]
  );

  const selectedItemTemplate = useCallback(
    (option: EPGFileDto) => {
      if (option === undefined) {
        return null;
      }
      const color = getColor(option.EPGNumber);

      return (
        <div className="flex align-items-center gap-2">
          <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
          <span>{option.Name}</span>
        </div>
      );
    },
    [getColor]
  );

  function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    return (
      <MultiSelect
        className="w-11"
        filter
        ref={multiSelectRef}
        itemTemplate={itemTemplate}
        maxSelectedLabels={1}
        showClear
        filterBy="Name"
        onChange={(e: MultiSelectChangeEvent) => {
          if (isEmptyObject(e.value)) {
            options.filterApplyCallback();
          } else {
            options.filterApplyCallback(e.value);
          }
        }}
        onShow={() => {}}
        options={epgFiles}
        placeholder="All"
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
