import EPGEditor from '@components/epg/EPGEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { isEmptyObject } from '@lib/common/common';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { EPGFileDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
import { ReactNode, useCallback, useRef } from 'react';

export const useSMChannelGroupColumnConfig = () => {
  const { data } = useGetChannelGroups();

  const multiSelectRef = useRef<MultiSelect>(null);
  // const epgFiles = useMemo(() => {
  //   const additionalOptions = [{ EPGNumber: -1, Name: 'SD' } as EPGFileDto, { EPGNumber: -99, Name: 'None' } as EPGFileDto];
  //   if (data) return [...additionalOptions, ...data];

  //   return additionalOptions;
  // }, [data]);

  const itemTemplate = useCallback((option: EPGFileDto) => {
    return (
      <div className="flex align-items-center gap-2">
        <i className="pi pi-circle-fill pr-2" />
        <span>{option.Name}</span>
      </div>
    );
  }, []);

  const selectedItemTemplate = useCallback((option: EPGFileDto) => {
    if (option === undefined) {
      return null;
    }

    return (
      <div className="flex align-items-center gap-2">
        <i className="pi pi-circle-fill pr-2" />
        <span>{option.Name}</span>
      </div>
    );
  }, []);

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
        options={data}
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
