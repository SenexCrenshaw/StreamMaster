/* eslint-disable react/no-unused-prop-types */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/consistent-type-imports */

import React from "react";
import * as StreamMasterApi from '../../store/iptvApi';
import * as Hub from '../../store/signlar_functions';
import { Toast } from 'primereact/toast';
import SchedulesDirectCountrySelector from "../../components/SchedulesDirectCountrySelector";
import StringEditorBodyTemplate from "../../components/StringEditorBodyTemplate";
import SchedulesDirectHeadendDataSelector from "../../components/SchedulesDirectHeadendDataSelector";

const SDEditor = (props: SDEditorProps) => {
  const toast = React.useRef<Toast>(null);
  const getStatusQuery = StreamMasterApi.useSchedulesDirectGetStatusQuery();
  const [country, setCountry] = React.useState<string>('USA');
  const [postalCode, setPostalCode] = React.useState<string>('19087');
  const status = React.useMemo(() => {
    if (getStatusQuery.data?.systemStatus?.[0].status?.toLocaleLowerCase() === 'online') {
      return (<div>Schedules Direct System Status: <span className='text-green-500'>Online</span></div>);
    }

    return (<div>Schedules Direct System Status: <span className='text-red-500'>Offline</span></div>);

  }, [getStatusQuery.data]);
  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      {status}
      <SchedulesDirectCountrySelector onChange={(e) => setCountry(e)} value={country} />
      <StringEditorBodyTemplate
        onChange={async (e) => {
          console.debug(e);
          setPostalCode(e.toString());
        }}
        value={postalCode}
      />

      <SchedulesDirectHeadendDataSelector country={country} postalCode={postalCode} />
    </>
  );
}

SDEditor.displayName = 'SDEditor';
SDEditor.defaultProps = {
  onChange: null,
  value: null,
};

type SDEditorProps = {
  data?: StreamMasterApi.ChannelGroupDto | undefined;
  onChange?: ((value: string) => void) | null;
  value?: string | null;
};

export default React.memo(SDEditor);
