import SMDropDown from '@components/sm/SMDropDown';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { arraysEqual, isEmptyObject } from '@lib/common/common';
import useSelectedAndQ from '@lib/hooks/useSelectedAndQ';
import { useFilters } from '@lib/redux/hooks/filters';

import useGetM3UFiles from '@lib/smAPI/M3UFiles/useGetM3UFiles';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { DataTableFilterMetaData } from 'primereact/datatable';
import { ReactNode, useCallback } from 'react';

interface SMStreamM3UColumnConfigProperties {
  readonly className?: string;
  readonly dataKey: string;
  readonly width?: string;
}

export const useSMStreamM3UColumnConfig = ({ className = 'sm-w-7rem sm-scroller-items', dataKey }: SMStreamM3UColumnConfigProperties) => {
  const { data } = useGetM3UFiles();

  const { filters, setFilters } = useFilters(dataKey);
  const { selectedItems } = useSelectedAndQ('useSMStreamM3UColumnConfig');

  const updateFilters = () => {
    if (selectedItems.length === 0) {
      const newFilter = { ...filters };
      const a = newFilter['M3UFileName'] as DataTableFilterMetaData;
      if (a && a.value !== undefined) {
        newFilter['M3UFileName'] = { matchMode: 'contains', value: undefined };
        setFilters(newFilter);
      }
    } else {
      const names = selectedItems.map((x) => x.Name);
      const newFilter = { ...filters };
      const a = newFilter['M3UFileName'] as DataTableFilterMetaData;
      if (!filters['M3UFileName'] || !arraysEqual(a.value, names)) {
        newFilter['M3UFileName'] = { matchMode: 'contains', value: names };
        setFilters(newFilter);
      }
    }
  };

  updateFilters();

  const itemTemplate = useCallback((option: M3UFileDto) => {
    if (option?.Name === undefined) {
      return null;
    }

    return (
      <div className="sm-channelgroup-selector sm-w-12rem ">
        <div className="text-container">{option.Name}</div>
      </div>
    );
  }, []);

  const buttonTemplate = useCallback((options: any): ReactNode => {
    if (Array.isArray(options.value)) {
      if (options.value.length > 0) {
        const names = options.value.map((x: any) => x.Name);
        const sortedInput = [...names].sort();
        return (
          <div className="sm-channelgroup-selector">
            <div className="text-container">{sortedInput.join(', ')}</div>
          </div>
        );
      }
    }

    return (
      <div className="sm-channelgroup-selector">
        <div className="text-container pl-1">M3U</div>
      </div>
    );
  }, []);

  function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    return (
      <div className={className}>
        <SMDropDown
          buttonDarkBackground
          buttonTemplate={buttonTemplate(options)}
          data={data}
          itemTemplate={itemTemplate}
          filter
          filterBy="Name"
          fixed
          onChange={async (e: any) => {
            if (isEmptyObject(e) || !Array.isArray(e)) {
              options.filterApplyCallback();
            }
          }}
          select
          selectedItemsKey="useSMStreamM3UColumnConfig"
          title="M3U"
          contentWidthSize="2"
        />
      </div>
    );
  }

  const columnConfig: ColumnMeta = {
    align: 'left',
    field: 'M3UFileName',
    filter: true,
    filterElement: filterTemplate,
    header: 'M3UFileName',
    sortable: true,
    width: '8rem'
  };

  return columnConfig;
};
