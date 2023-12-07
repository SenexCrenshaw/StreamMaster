import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { useSchedulesDirectGetLineupPreviewChannelQuery } from '@lib/iptvApi';
import { skipToken } from '@reduxjs/toolkit/query';
import { memo, useMemo } from 'react';

type SchedulesDirectLineupPreviewChannelProps = {
  lineup: string | undefined;
};

const SchedulesDirectLineupPreviewChannel = ({ lineup }: SchedulesDirectLineupPreviewChannelProps) => {
  const getPreviewQuery = useSchedulesDirectGetLineupPreviewChannelQuery(lineup ? lineup : skipToken);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'channel', filter: true, sortable: true },
      { field: 'name', filter: true, sortable: true },
      { field: 'callsign', filter: true, sortable: true },
      { field: 'affiliate', filter: true, sortable: true }
    ],
    []
  );

  if (!lineup) {
    return <div></div>;
  }

  return (
    <div className="flex grid flex-col">
      <DataSelector
        isLoading={getPreviewQuery.isLoading}
        columns={columns}
        dataSource={getPreviewQuery.data}
        defaultSortField="name"
        disableSelectAll
        emptyMessage="No Line Ups"
        headerName="Line Up Preview"
        id="queustatus"
        selectedItemsKey="queustatus"
        style={{ height: 'calc(50vh)' }}
      />
    </div>
  );
};

export default memo(SchedulesDirectLineupPreviewChannel);
