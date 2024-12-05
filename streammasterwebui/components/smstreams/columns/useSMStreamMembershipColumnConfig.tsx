import { getIconUrl } from '@components/icons/iconUtil';
import SMDropDown from '@components/sm/SMDropDown';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { ReactNode, useCallback, useMemo } from 'react';

interface SMStreamGroupMembershipProperties {
  readonly dataKey: string;
  readonly width?: string;
}
export const useSMStreamMembershipColumnConfig = ({ dataKey }: SMStreamGroupMembershipProperties) => {
  const itemTemplate = useCallback((option: SMStreamDto) => {
    if (option === undefined) {
      return null;
    }

    const iconUrl = getIconUrl(option.Logo);

    return (
      <div className="flex sm-start-stuff sm-w-10">
        <div className="icon-button-template">
          <img className="icon-button-template" alt="Icon logo" src={iconUrl} />
        </div>
        <div className="pl-1 text-xs text-container"> {option.Name}</div>
      </div>
    );
  }, []);

  const buttonTemplate = useCallback(
    (row: SMStreamDto): ReactNode => {
      if (row === undefined || row.ChannelMembership === undefined || row.ChannelMembership.length === 0) {
        return null;
      }
      return itemTemplate(row);
    },
    [itemTemplate]
  );

  const bodyTemplate = useCallback(
    (rowData: any) => {
      var data = rowData as unknown as SMStreamDto;

      return (
        <SMDropDown
          buttonTemplate={buttonTemplate(data)}
          contentWidthSize="2"
          data={data.ChannelMembership}
          info=""
          itemTemplate={itemTemplate}
          scrollHeight="20vh"
          title="Channel Membership"
        />
      );
    },
    [buttonTemplate, itemTemplate]
  );

  const columnConfig: ColumnMeta = useMemo(() => {
    // function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    //   return <SMChannelSelector onChange={(e) => {}} />;
    // }

    const columnConfig: ColumnMeta = {
      align: 'right',
      bodyTemplate: bodyTemplate,
      field: 'ChannelMembership',
      // filter: true,
      // filterElement: filterTemplate,
      header: 'Membership',
      // sortable: true,
      width: 125
    };

    return columnConfig;
  }, [bodyTemplate]);

  return columnConfig;
};
