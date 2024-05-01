import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { isEmptyObject } from '@lib/common/common';
import useGetM3UFileNames from '@lib/smAPI/M3UFiles/useGetM3UFileNames';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
import { ReactNode, useCallback, useRef } from 'react';

export const useSMStreamM3UColumnConfig = () => {
  const { data } = useGetM3UFileNames();

  const multiSelectRef = useRef<MultiSelect>(null);

  const itemTemplate = useCallback((option: string) => {
    if (option === undefined) {
      return null;
    }

    return <span>{option}</span>;
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
  const bodyTemplate = (bodyData: SMStreamDto) => {
    return <div>{bodyData.M3UFileName}</div>;
  };

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate: bodyTemplate,
    field: 'M3UFileName',
    filter: true,
    filterElement: filterTemplate,
    header: 'M3UFileName',
    maxWidth: '10rem',
    sortable: true
  };

  return columnConfig;
};
