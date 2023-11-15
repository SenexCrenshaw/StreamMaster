import StandardHeader from '@components/StandardHeader';
import SchedulesDirectHeadendDataSelector from '@components/schedulesDirect/SchedulesDirectHeadendDataSelector';

import SchedulesDirectLineUpsDataSelector from '@components/schedulesDirect/SchedulesDirectLineUpsDataSelector';
import { SDIcon } from '@lib/common/icons';
import { useSchedulesDirectGetStatusQuery } from '@lib/iptvApi';

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
    <StandardHeader displayName={`Schedules Direct - ${status}`} icon={<SDIcon />}>
      {/* <BlockUI blocked={getStatusQuery.data?.systemStatus?.[0].status?.toLocaleLowerCase() !== 'online' || settings.data?.sdEnabled !== true}> */}

      <div className="col-6 m-0 p-0 pr-1">
        {/* <SchedulesDirectCountrySelector /> */}
        <SchedulesDirectHeadendDataSelector />
      </div>
      <div className="col-6 m-0 p-0 border-2 border-round surface-border">
        <div className="flex grid col-12 pl-1 justify-content-start align-items-center m-0 w-full smallpt"></div>
        <SchedulesDirectLineUpsDataSelector id={'SDEditor'} />
      </div>
      {/* </BlockUI> */}
    </StandardHeader>
  );
};

export default memo(SDEditor);
