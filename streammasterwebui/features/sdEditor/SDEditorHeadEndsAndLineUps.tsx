import StandardHeader from '@components/StandardHeader';
import SchedulesDirectHeadendDataSelector from '@components/schedulesDirect/SchedulesDirectHeadendDataSelector';

import SchedulesDirectLineUpsDataSelector from '@components/schedulesDirect/SchedulesDirectLineUpsDataSelector';
import { SDIcon } from '@lib/common/icons';
import { useSchedulesDirectGetStatusQuery } from '@lib/iptvApi';
import useSettings from '@lib/useSettings';
import { BlockUI } from 'primereact/blockui';

import { memo, useMemo } from 'react';

const SDEditorHeadEndsAndLineUps = () => {
  const getStatusQuery = useSchedulesDirectGetStatusQuery();
  const settings = useSettings();

  const isSDReady = useMemo((): boolean => {
    return getStatusQuery.data?.systemStatus?.[0].status?.toLocaleLowerCase() === 'online' && settings.data?.sdEnabled === true;
  }, [getStatusQuery.data?.systemStatus, settings.data?.sdEnabled]);

  const status = useMemo(() => {
    if (getStatusQuery.data?.systemStatus?.[0].status?.toLocaleLowerCase() === 'online') {
      console.log(getStatusQuery.data);
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
  }, [getStatusQuery.data]);
  console.log(status);
  return (
    <BlockUI blocked={!isSDReady}>
      <StandardHeader displayName={status} icon={<SDIcon />}>
        <div className="col-6 m-0 p-0 pr-1">
          <SchedulesDirectHeadendDataSelector />
        </div>
        <div className="col-6 m-0 p-0 border-2 border-round surface-border">
          <div className="flex grid col-12 pl-1 justify-content-start align-items-center m-0 w-full smallpt"></div>
          <SchedulesDirectLineUpsDataSelector id={'SDEditor'} />
        </div>
      </StandardHeader>
    </BlockUI>
  );
};

export default memo(SDEditorHeadEndsAndLineUps);
