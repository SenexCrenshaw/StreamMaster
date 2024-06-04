import ChannelGroupSelector from '@components/channelGroups/ChannelGroupSelector';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { ReactNode } from 'react';

export const useSMStreamGroupColumnConfig = () => {
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
    maxWidth: '10rem',
    sortable: true
  };

  return columnConfig;
};
