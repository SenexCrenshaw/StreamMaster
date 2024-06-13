import SMChannelNameEditor from '@components/smchannels/SMChannelNameEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import { useMemo } from 'react';

interface SMChannelNameColumnConfigProperties {
  readonly width: number;
}

export const useSMChannelNameColumnConfig = ({ width }: SMChannelNameColumnConfigProperties) => {
  const bodyTemplate = (smChannelDto: SMChannelDto) => {
    return <SMChannelNameEditor smChannelDto={smChannelDto} />;
  };

  const columnConfig: ColumnMeta = useMemo(() => {
    return {
      align: 'left',
      bodyTemplate: bodyTemplate,
      field: 'Name',
      filter: true,
      header: 'Name',
      sortable: true,
      width: width
    } as ColumnMeta;
  }, [width]);

  return columnConfig;
};
