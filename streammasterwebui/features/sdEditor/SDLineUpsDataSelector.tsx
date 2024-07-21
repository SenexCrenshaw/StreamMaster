import StandardHeader from '@components/StandardHeader';
import SchedulesDirectHeadendDataSelector from '@components/schedulesDirect/SchedulesDirectHeadendDataSelector';
import SchedulesDirectLineUpsDataSelector from '@components/schedulesDirect/SchedulesDirectLineUpsDataSelector';
import SchedulesDirectStationDataSelector from '@components/schedulesDirect/SchedulesDirectStationDataSelector';
import { SDIcon } from '@lib/common/icons';
import { useSMContext } from '@lib/context/SMProvider';

import { BlockUI } from 'primereact/blockui';

import { memo, useMemo } from 'react';

const SDLineUpsDataSelector = () => {
  const { settings } = useSMContext();

  const isSDReady = useMemo((): boolean => {
    return settings.SDSettings?.SDEnabled ?? false;
  }, [settings]);

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
        <div className="sm-w-12 flex flex-row">
          <div className="sm-w-6 flex flex-column justify-content-between pr-1">
            <SchedulesDirectLineUpsDataSelector id={'SDEditor'} />

            <SchedulesDirectHeadendDataSelector />
          </div>
          <SchedulesDirectStationDataSelector />
        </div>
      </StandardHeader>
    </BlockUI>
  );
};

export default memo(SDLineUpsDataSelector);
