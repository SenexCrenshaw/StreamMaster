import SchedulesDirectStationPreviewDataSelector from '@/components/schedulesDirect/SchedulesDirectStationPreviewDataSelector';
import { useSchedulesDirectGetStatusQuery } from '@/lib/iptvApi';
import { BlockUI } from 'primereact/blockui';
import { memo, useMemo } from 'react';

const SDEditor = () => {
  const getStatusQuery = useSchedulesDirectGetStatusQuery();

  const status = useMemo(() => {
    if (getStatusQuery.data?.systemStatus?.[0].status?.toLocaleLowerCase() === 'online') {
      return (
        <div>
          Schedules Direct System Status: <span className="text-green-500">Online</span>
        </div>
      );
    }

    return (
      <div>
        Schedules Direct System Status: <span className="text-red-500">Offline</span>
      </div>
    );
  }, [getStatusQuery.data]);

  return (
    <>
      {status}
      <BlockUI blocked={getStatusQuery.data?.systemStatus?.[0].status?.toLocaleLowerCase() !== 'online'}>
        <SchedulesDirectStationPreviewDataSelector />
      </BlockUI>
    </>
  );
};

export default memo(SDEditor);
