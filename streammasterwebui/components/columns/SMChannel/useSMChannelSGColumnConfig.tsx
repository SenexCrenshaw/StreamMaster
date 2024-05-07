import EPGSGEditor from '@components/epg/EPGSGEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { SMChannelDto } from '@lib/smAPI/smapiTypes';

export const useSMChannelSGColumnConfig = () => {
  const bodyTemplate = (bodyData: SMChannelDto) => {
    return <EPGSGEditor data={bodyData} />;
  };

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate: bodyTemplate,
    field: 'sg',
    fieldType: 'sg',
    filter: false,
    header: '',
    maxWidth: '2rem',
    minWidth: '2rem',
    sortable: false,
    width: '2rem'
  };

  return columnConfig;
};
