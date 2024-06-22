import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

import { GetLineupPreviewChannel } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { LineupPreviewChannel } from '@lib/smAPI/smapiTypes';

import { Dialog } from 'primereact/dialog';
import { memo, useEffect, useMemo, useState } from 'react';

type SchedulesDirectLineupPreviewChannelProps = {
  lineup: string | undefined;
  readonly children?: React.ReactNode;
  onHide(): void;
};

const SchedulesDirectLineupPreviewChannel = ({ children, lineup, onHide }: SchedulesDirectLineupPreviewChannelProps) => {
  const [dataSource, setDataSource] = useState<LineupPreviewChannel[]>([]);

  useEffect(() => {
    if (!lineup) {
      return;
    }

    GetLineupPreviewChannel({ Lineup: lineup })
      .then((data) => {
        setDataSource(data ?? []);
      })
      .catch((error) => {
        setDataSource([]);
      });
  }, [lineup]);

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
          onHide();
        }}
      >
        <div className="flex grid flex-col">
          <SMDataTable
            // isLoading={getPreviewQuery.isLoading}
            columns={columns}
            dataSource={dataSource}
            defaultSortField="name"
            emptyMessage="No Line Ups"
            headerName="Line Up Preview"
            id="queustatus"
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
