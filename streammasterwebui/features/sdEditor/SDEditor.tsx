import SchedulesDirectStationPreviewDataSelector from '@/components/schedulesDirect/SchedulesDirectStationPreviewDataSelector';
import { useSchedulesDirectGetStatusQuery } from '@lib/iptvApi';

import useSettings from '@lib/useSettings';
import { BlockUI } from 'primereact/blockui';
import { memo, useMemo } from 'react';

// const SchedulesDirectStationPreviewDataSelector = React.lazy(() => import('@components/schedulesDirect/SchedulesDirectStationPreviewDataSelector'));

const SDEditor = () => {
  const getStatusQuery = useSchedulesDirectGetStatusQuery();
  const settings = useSettings();

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
      <BlockUI blocked={getStatusQuery.data?.systemStatus?.[0].status?.toLocaleLowerCase() !== 'online' || settings.data?.sdEnabled !== true}>
        <SchedulesDirectStationPreviewDataSelector />
      </BlockUI>
    </>
  );
};

export default memo(SDEditor);
