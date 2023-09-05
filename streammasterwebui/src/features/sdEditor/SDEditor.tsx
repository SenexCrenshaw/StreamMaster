

import { useState, useMemo, useCallback, memo } from "react";
import { type ChannelGroupDto } from "../../store/iptvApi";
import { useSchedulesDirectGetStatusQuery, useSchedulesDirectGetLineupsQuery } from "../../store/iptvApi";
import SchedulesDirectSchedulesDataSelector from "../../components/schedulesDirect/SchedulesDirectSchedulesDataSelector";

const SDEditor = (props: SDEditorProps) => {
  const getStatusQuery = useSchedulesDirectGetStatusQuery();
  const [country, setCountry] = useState<string>('USA');
  const [postalCode, setPostalCode] = useState<string>('19087');
  const getLineUpsQuery = useSchedulesDirectGetLineupsQuery();


  const status = useMemo(() => {
    if (getStatusQuery.data?.systemStatus?.[0].status?.toLocaleLowerCase() === 'online') {
      return (<div>Schedules Direct System Status: <span className='text-green-500'>Online</span></div>);
    }

    return (<div>Schedules Direct System Status: <span className='text-red-500'>Offline</span></div>);

  }, [getStatusQuery.data]);

  const onGetHeadends = useCallback(async () => {

  }, []);

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

type SDEditorProps = {
  data?: ChannelGroupDto | undefined;
  onChange?: ((value: string) => void) | null;
  value?: string | null;
};

export default memo(SDEditor);
