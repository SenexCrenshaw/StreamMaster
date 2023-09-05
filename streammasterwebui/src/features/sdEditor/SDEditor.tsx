

import { useMemo, memo } from "react";
import { useSchedulesDirectGetStatusQuery } from "../../store/iptvApi";
import SchedulesDirectSchedulesDataSelector from "../../components/schedulesDirect/SchedulesDirectSchedulesDataSelector";

const SDEditor = () => {
  const getStatusQuery = useSchedulesDirectGetStatusQuery();

  const status = useMemo(() => {
    if (getStatusQuery.data?.systemStatus?.[0].status?.toLocaleLowerCase() === 'online') {
      return (<div>Schedules Direct System Status: <span className='text-green-500'>Online</span></div>);
    }

    return (<div>Schedules Direct System Status: <span className='text-red-500'>Offline</span></div>);

  }, [getStatusQuery.data]);


  return (
    <>
      {status}
      {/* <SchedulesDirectCountrySelector onChange={(e) => setCountry(e)} value={country} />
      <StringEditorBodyTemplate
        onChange={async (e) => {
          console.debug(e);
          setPostalCode(e.toString());
        }}
        value={postalCode}
      />
      <SchedulesDirectHeadendDataSelector country={country} postalCode={postalCode} /> */}
      {/* <SchedulesDirectLineUpsDataSelector /> */}
      <SchedulesDirectSchedulesDataSelector stationIds={[]} />

    </>
  );
}

SDEditor.displayName = 'SDEditor';
SDEditor.defaultProps = {
  onChange: null,
  value: null,
};

// type SDEditorProps = {
//   readonly data?: ChannelGroupDto | undefined;
//   readonly onChange?: ((value: string) => void) | null;
//   readonly value?: string | null;
// };

export default memo(SDEditor);
