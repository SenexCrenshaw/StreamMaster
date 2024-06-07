import ChannelGroupSelector from '@components/channelGroups/ChannelGroupSelector';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { arraysEqual } from '@lib/common/common';
import useSelectedAndQ from '@lib/hooks/useSelectedAndQ';
import { useFilters } from '@lib/redux/hooks/filters';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { DataTableFilterMetaData } from 'primereact/datatable';
import { ReactNode, useMemo } from 'react';

interface SMStreamGroupColumnConfigProperties {
  readonly dataKey: string;
  readonly width?: string;
}
export const useSMStreamGroupColumnConfig = ({ dataKey, width = '10rem' }: SMStreamGroupColumnConfigProperties) => {
  const { filters, setFilters } = useFilters(dataKey);
  const { selectedItems } = useSelectedAndQ('useSMStreamGroupColumnConfig');

  const columnConfig: ColumnMeta = useMemo(() => {
    const updateFilters = () => {
      if (selectedItems.length === 0) {
        const newFilter = { ...filters };
        const a = newFilter['Group'] as DataTableFilterMetaData;
        if (a && a.value !== undefined) {
          newFilter['Group'] = { matchMode: 'contains', value: undefined };
          setFilters(newFilter);
        }
      } else {
        const names = selectedItems.map((x) => x.Name);

        if (filters['Group']) {
          const newFilter = { ...filters };
          const a = newFilter['Group'] as DataTableFilterMetaData;
          if (!arraysEqual(a.value, names)) {
            newFilter['Group'] = { matchMode: 'contains', value: names };
            setFilters(newFilter);
          }
        }
      }
    };

    updateFilters();

    function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
      return (
        <ChannelGroupSelector
          dataKey="useSMStreamGroupColumnConfig"
          onChange={(e) => {
            if (e) {
              options.filterApplyCallback();
            }
          }}
        />
      );
    }

    const bodyTemplate = (bodyData: SMStreamDto) => {
      return <div className="text-container">{bodyData.Group}</div>;
    };

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate: bodyTemplate,
    field: 'Group',
    filter: true,
    filterElement: filterTemplate,
    header: 'Group',
    sortable: true,
    width: '16'
  };

  return columnConfig;
};
