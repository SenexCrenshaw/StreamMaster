import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import { useCallback, useMemo } from 'react';
import SMChannelNameEditor from '../SMChannelNameEditor';

interface SMChannelNameColumnConfigProperties {
  readonly width: number;
}

export const useSMChannelNameColumnConfig = ({ width }: SMChannelNameColumnConfigProperties) => {
  const bodyTemplate = useCallback((smChannelDto: SMChannelDto) => {
    // if (smChannelDto.SMChannelType === 1) {
    //   return <div className="text-container pl-1">{smChannelDto.Name}</div>;
    // }

    return <SMChannelNameEditor smChannelDto={smChannelDto} />;
  }, []);

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
  }, [bodyTemplate, width]);

  return columnConfig;
};
