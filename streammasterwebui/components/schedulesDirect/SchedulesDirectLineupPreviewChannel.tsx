import SMPopUp from '@components/sm/SMPopUp';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { GetLineupPreviewChannel } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { LineupPreviewChannel } from '@lib/smAPI/smapiTypes';
import { memo, useEffect, useMemo, useState } from 'react';

type SchedulesDirectLineupPreviewChannelProps = {
  lineup: string | undefined;
};

const SchedulesDirectLineupPreviewChannel = ({ lineup }: SchedulesDirectLineupPreviewChannelProps) => {
  const dataKey = 'LineupPreviewChannel';
  const [isOpen, setIsOpen] = useState(false);
  const [dataSource, setDataSource] = useState<LineupPreviewChannel[]>([]);

  useEffect(() => {
    if (!lineup || isOpen !== true) {
      return;
    }

    GetLineupPreviewChannel({ Lineup: lineup })
      .then((data) => {
        setDataSource(data ?? []);
      })
      .catch((error) => {
        setDataSource([]);
      })
      .finally(() => {});
  }, [lineup, isOpen]);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Channel', filter: true, sortable: true },
      { field: 'Name', filter: true, sortable: true },
      { field: 'Callsign', filter: true, sortable: true },
      { field: 'Affiliate', filter: true, sortable: true }
    ],
    []
  );

  if (!lineup) {
    return <div></div>;
  }

  return (
    <SMPopUp
      title={
        <div className="flex flex-row align-items-center">
          Line Up Preview: <div className="pl-3 text-sm text-color">{lineup}</div>
        </div>
      }
      contentWidthSize="5"
      icon="pi-id-card"
      modal
      modalCentered
      onCloseClick={() => console.log('close')}
      onOpen={setIsOpen}
      tooltip="Line Up Preview"
    >
      <SMDataTable
        enablePaginator
        columns={columns}
        dataSource={dataSource}
        defaultSortField="name"
        emptyMessage="No Line Ups"
        noSourceHeader
        // headerName={
        //   <div className="flex flex-row align-items-center">
        //     Line Up Preview: <div className="pl-3 text-sm text-color">{lineup}</div>
        //   </div>
        // }
        id={dataKey}
        isLoading={!dataSource}
        style={{ height: 'calc(40vh)' }}
      />
      {/* )} */}
    </SMPopUp>
  );
};

export default memo(SchedulesDirectLineupPreviewChannel);
