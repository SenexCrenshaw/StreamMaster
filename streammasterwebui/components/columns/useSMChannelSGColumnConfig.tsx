import EPGSGEditor from '@components/epg/EPGSGEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { EPGFileDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { useCallback } from 'react';

export const useSMChannelSGColumnConfig = () => {
  const itemTemplate = useCallback((option: EPGFileDto) => {
    return (
      <div className="flex align-items-center gap-2">
        <span>{option.Name}</span>
      </div>
    );
  }, []);

  const selectedItemTemplate = useCallback((option: EPGFileDto) => {
    if (option === undefined) {
      return null;
    }

    return (
      <div className="flex align-items-center gap-2">
        <span>{option.Name}</span>
      </div>
    );
  }, []);

  const bodyTemplate = (bodyData: SMChannelDto) => {
    return <EPGSGEditor data={bodyData} />;
  };

  const columnConfig: ColumnMeta = {
    align: 'left',

    bodyTemplate: bodyTemplate,
    field: 'sg',
    filter: false,
    header: 'SG',
    maxWidth: '4rem',
    minWidth: '4rem',
    sortable: false,
    width: '4rem'
  };

  return columnConfig;
};
