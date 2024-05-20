import ChannelGroupSelector from '@components/channelGroups/ChannelGroupSelector';
import SMChannelGroupEditor from '@components/columns/SMChannel/SMChannelGroupEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { arraysEqual } from '@lib/common/common';
import useSelectedAndQ from '@lib/hooks/useSelectedAndQ';
import { useFilters } from '@lib/redux/hooks/filters';
import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { DataTableFilterMetaData } from 'primereact/datatable';
import { ReactNode, useEffect } from 'react';

export const useSMChannelGroupColumnConfig = (tableDataKey: string) => {
  const { filters, setFilters } = useFilters(tableDataKey);
  const { selectedItems } = useSelectedAndQ('useSMChannelGroupColumnConfig');

  useEffect(() => {
    const newFilter = { ...filters };
    if (selectedItems.length === 0) {
      const a = newFilter['Group'] as DataTableFilterMetaData;
      if (a && a.value !== undefined) {
        newFilter['Group'] = { matchMode: 'contains', value: undefined };
        setFilters(newFilter);
      }

      return;
    }

    const names = selectedItems.map((x) => x.Name);

    if (newFilter['Group']) {
      const a = newFilter['Group'] as DataTableFilterMetaData;
      if (!arraysEqual(a.value, names)) {
        newFilter['Group'] = { matchMode: 'contains', value: names };
        setFilters(newFilter);
      }
    }
  }, [filters, selectedItems, setFilters]);

  function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    return (
      <ChannelGroupSelector
        darkBackGround
        dataKey="useSMChannelGroupColumnConfig"
        value={options.value}
        onChange={(e) => {
          if (e) {
            options.filterApplyCallback();
          }
        }}
      />
    );
  }
  const bodyTemplate = (bodyData: SMChannelDto) => {
    return <SMChannelGroupEditor tableDataKey={tableDataKey} data={bodyData} useSelectedItemsFilter />;
  };

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate: bodyTemplate,
    field: 'Group',
    filter: true,
    filterElement: filterTemplate,
    header: 'Group',
    maxWidth: '10rem',
    minWidth: '10rem',
    sortable: true,
    width: '10rem'
  };

  return columnConfig;
};
