import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';

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

    GetLineupPreviewChannel({ lineup: lineup })
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
          <DataSelector
            // isLoading={getPreviewQuery.isLoading}
            columns={columns}
            dataSource={dataSource}
            defaultSortField="name"
            disableSelectAll
            emptyMessage="No Line Ups"
            enableState={false}
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
