import ChannelGroupEditor from '@components/channelGroups/ChannelGroupEditor';
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

  const itemTemplate = useCallback((option: EPGFileDto) => {
    if (option === undefined) {
      return null;
    }

    return <span>{option.Name}</span>;
  }, []);

  function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    return (
      <MultiSelect
        className="w-full"
        filter
        ref={multiSelectRef}
        itemTemplate={itemTemplate}
        maxSelectedLabels={1}
        showClear
        clearIcon="pi pi-filter-slash"
        filterBy="Name"
        onChange={(e: MultiSelectChangeEvent) => {
          if (isEmptyObject(e.value)) {
            options.filterApplyCallback();
          } else {
            options.filterApplyCallback(e.value);
          }
        }}
        options={data}
        placeholder="All"
        value={options.value}
        selectedItemTemplate={itemTemplate}
      />
    );
  }
  const bodyTemplate = (bodyData: SMChannelDto) => {
    return <ChannelGroupEditor data={bodyData} />;
  };

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate: bodyTemplate,
    field: 'Group',
    filter: true,
    filterElement: filterTemplate,
    header: 'Group',
    maxWidth: '14rem',
    minWidth: '14rem',
    sortable: true,
    width: '14rem'
  };

  return columnConfig;
};
