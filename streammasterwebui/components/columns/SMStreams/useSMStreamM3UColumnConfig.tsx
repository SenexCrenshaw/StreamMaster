import SMDropDown from '@components/sm/SMDropDown';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { isEmptyObject } from '@lib/common/common';
import useGetM3UFileNames from '@lib/smAPI/M3UFiles/useGetM3UFileNames';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { ReactNode, useCallback } from 'react';

export const useSMStreamM3UColumnConfig = () => {
  const { data } = useGetM3UFileNames();

  const dataKey = 'm3uColumn-selections';

  const itemTemplate = useCallback((option: string) => {
    if (option === undefined) {
      return null;
    }

    return <span>{option}</span>;
  }, []);

  const buttonTemplate = useCallback((options: any): ReactNode => {
    if (Array.isArray(options.value)) {
      if (options.value.length > 0) {
        const names = options.value;
        const sortedInput = [...names].sort();
        return <div className="text-container">{sortedInput.join(', ')}</div>;
      }
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">M3U</div>
      </div>
    );
  }, []);

  function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    return (
      <div className="w-full">
        <SMDropDown
          buttonDarkBackground
          buttonTemplate={buttonTemplate(options)}
          data={data}
          itemTemplate={itemTemplate}
          filter
          filterBy="Name"
          onChange={async (e: any) => {
            if (isEmptyObject(e) || !Array.isArray(e)) {
              options.filterApplyCallback();
            } else {
              options.filterApplyCallback(e);
            }
          }}
          select
          selectedItemsKey={dataKey}
          title="M3U"
          contentWidthSize="2"
        />
      </div>
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
    sortable: true,
    minWidth: '6'
  };

  return columnConfig;
};
