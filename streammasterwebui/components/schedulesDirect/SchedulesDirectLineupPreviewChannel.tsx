import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { useSchedulesDirectGetLineupPreviewChannelQuery } from '@lib/iptvApi';
import { skipToken } from '@reduxjs/toolkit/query';
import { Dialog } from 'primereact/dialog';
import { memo, useMemo, useState } from 'react';

type SchedulesDirectLineupPreviewChannelProps = {
  lineup: string | undefined;
  readonly children?: React.ReactNode;
  onHide(): void;
};

const SchedulesDirectLineupPreviewChannel = ({ children, lineup, onHide }: SchedulesDirectLineupPreviewChannelProps) => {
  const getPreviewQuery = useSchedulesDirectGetLineupPreviewChannelQuery(lineup ? lineup : skipToken);

  const [reset, setReset] = useState<boolean>(false);

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
    <div>
      <Dialog
        header={lineup ? lineup : ''}
        visible={lineup !== undefined}
        style={{ width: '50vw' }}
        onHide={() => {
          setReset(true);
          onHide();
        }}
      >
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
            OnReset={() => setReset(false)}
            reset={reset}
            selectedItemsKey="queustatus"
            style={{ height: 'calc(50vh)' }}
          />
        </div>
      </Dialog>
      {children}
    </div>
  );
};

export default memo(SchedulesDirectLineupPreviewChannel);
