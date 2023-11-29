import StandardHeader from '@components/StandardHeader';
import SchedulesDirectStationPreviewDataSelector from '@components/schedulesDirect/SchedulesDirectStationPreviewDataSelector';
import { SDIcon } from '@lib/common/icons';
import { useSchedulesDirectGetStatusQuery } from '@lib/iptvApi';
import useSettings from '@lib/useSettings';
import { BlockUI } from 'primereact/blockui';
import { memo, useMemo } from 'react';

const SDEditorChannels = () => {
  const getStatusQuery = useSchedulesDirectGetStatusQuery();
  const settings = useSettings();

  const isSDReady = useMemo((): boolean => {
    if (!getStatusQuery.data?.systemStatus || getStatusQuery.data?.systemStatus.length === 0) {
      return false;
    }
    console.log(getStatusQuery.data.systemStatus);
    return getStatusQuery.data.systemStatus[0].status?.toLocaleLowerCase() === 'online' && settings.data?.sdEnabled === true;
  }, [getStatusQuery.data?.systemStatus, settings.data?.sdEnabled]);

  const status = useMemo(() => {
    if (isSDReady) {
      return (
        <span>
          Schedules Direct System Status: <span className="text-green-500">Online</span>
        </span>
      );
    }

    return (
      <div>
        Schedules Direct System Status: <span className="text-red-500">Offline</span>
      </div>
    );
  }, [isSDReady]);
  console.log(status);
  return (
    <BlockUI blocked={!isSDReady}>
      <StandardHeader displayName={status} icon={<SDIcon />}>
        <SchedulesDirectStationPreviewDataSelector />
      </StandardHeader>
    </BlockUI>
  );
};

export default memo(SDEditorChannels);
