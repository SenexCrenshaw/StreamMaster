import SMChannelGroupEditor from '@components/columns/SMChannel/SMChannelGroupEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { isEmptyObject } from '@lib/common/common';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { ChannelGroupDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
import { ReactNode, useCallback, useRef } from 'react';

export const useSMChannelGroupColumnConfig = () => {
  const { data } = useGetChannelGroups();

  const multiSelectRef = useRef<MultiSelect>(null);

  const itemTemplate = useCallback((option: ChannelGroupDto) => {
    if (option === undefined) {
      return null;
    }

    return <span>{option.Name}</span>;
  }, []);

  function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    return (
      <MultiSelect
        className="w-full input-height-with-no-borders"
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
        placeholder="Group"
        value={options.value}
        selectedItemTemplate={itemTemplate}
      />
    );
  }
  const bodyTemplate = (bodyData: SMChannelDto) => {
    return <SMChannelGroupEditor data={bodyData} />;
  };

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate: bodyTemplate,
    field: 'Group',
    filter: true,
    filterElement: filterTemplate,
    header: 'Group',
    maxWidth: '10rem',
    sortable: true
  };

  return columnConfig;
};