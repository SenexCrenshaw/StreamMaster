import StandardHeader from '@components/StandardHeader';
import SchedulesDirectStationDataSelector from '@components/schedulesDirect/SchedulesDirectStationDataSelector';
import { SDIcon } from '@lib/common/icons';
import useSettings from '@lib/useSettings';
import { BlockUI } from 'primereact/blockui';
import { memo, useMemo } from 'react';

const SDEditorChannels = () => {
  const settings = useSettings();

  const isSDReady = useMemo((): boolean => {
    return settings.data?.sdSettings?.sdEnabled ?? false;
  }, [settings.data?.sdSettings?.sdEnabled]);

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
