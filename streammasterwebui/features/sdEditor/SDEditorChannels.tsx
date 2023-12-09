import StandardHeader from '@components/StandardHeader';
import SchedulesDirectStationDataSelector from '@components/schedulesDirect/SchedulesDirectStationDataSelector';
import { SDIcon } from '@lib/common/icons';
import { useSchedulesDirectGetUserStatusQuery } from '@lib/iptvApi';
import useSettings from '@lib/useSettings';
import { BlockUI } from 'primereact/blockui';
import { memo, useMemo } from 'react';

const SDEditorChannels = () => {
  const getStatusQuery = useSchedulesDirectGetUserStatusQuery();
  const settings = useSettings();

  const isSDReady = useMemo((): boolean => {
    if (!getStatusQuery.data?.systemStatus || getStatusQuery.data?.systemStatus.length === 0 || settings.data?.sdSettings?.sdEnabled !== true) {
      return false;
    }

    return getStatusQuery.data.systemStatus[0].status?.toLocaleLowerCase() === 'online';
  }, [getStatusQuery.data?.systemStatus, settings.data?.sdSettings?.sdEnabled]);

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

  return (
    <BlockUI blocked={!isSDReady}>
      <StandardHeader displayName={status} icon={<SDIcon />}>
        <SchedulesDirectStationDataSelector />
      </StandardHeader>
    </BlockUI>
  );
};

export default memo(SDEditorChannels);
